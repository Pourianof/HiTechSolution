

using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Dto.Product;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Helpers;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Product;
using HiTechStore.Infrastructure.Data.Mapping;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Helpers.Types;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Services.Product;

public class ProductService(
    IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider,
    IDiscountConditionScriptParser scriptParser,
    IMapper mapper,
    IConditionComponentTreeToLambdaExpression conditionTreeToLambdaExprMapper,
    ProductServiceHelper productServiceHelper,
    IAuthorizationService authorizationService,
    IPublicAssetRegisterer assetRegisterer,
    ProductPermissionHelper productPermissionHelper
) : ServiceBase(authorizationService, currentUserProvider), IProductService
{
    private void ApplyRulesToProducts(IEnumerable<ProductDto> products, IEnumerable<DiscountRule> rules)
    {
        foreach (var rule in rules)
        {
            var productConditionTree = rule.ProductRawConditionScript is null ? default : scriptParser.Parse(rule.ProductRawConditionScript);

            var items = products;
            if (productConditionTree is not null)
            {
                // need new instance for every process and remove previous state
                var filterExpr = conditionTreeToLambdaExprMapper.Map<ProductDto>(productConditionTree, nameof(Product));

                items = products.Where(filterExpr.Compile());
            }


            foreach (var variation in items.SelectMany(i => i.Variations))
            {
                variation.Discount += rule.DiscountAction!.Type == DiscountActionType.Percent ?
                    variation.Price * (double)rule.DiscountAction.Value! / 100 :
                    (double)rule.DiscountAction.Value;
            }
        }
    }

    private async Task<IEnumerable<DiscountRule>> GetAppliableActiveDiscount()
    {
        var activeDiscounts = await unitOfWork.DiscountRepository.GetActiveDiscountsOfTypeAsync(DiscountType.Products);

        var rules = activeDiscounts.SelectMany(
            (discount) => discount.Rules!
        );

        var currentUser = await GetUserOrDefault();

        var isAuthorized = currentUser is not null;

        List<DiscountRule> applyableRules = [];
        foreach (var rule in rules)
        {
            var productConditionTree = rule.ProductRawConditionScript is null ? default : scriptParser.Parse(rule.ProductRawConditionScript);
            var userConditionTree = rule.UserRawConditionScript is null ? default : scriptParser.Parse(rule.UserRawConditionScript);

            if (productConditionTree is null && userConditionTree is null)
            {
                continue;
            }

            if (userConditionTree is not null)
            {
                if (!isAuthorized)
                {
                    // if there is user-specific condition but client not authorized as user
                    // so the whole discount not associate to him
                    continue;
                }

                var userEvaluator = conditionTreeToLambdaExprMapper.Map<User>(userConditionTree);
                var isUserAuthorized = userEvaluator.Compile().Invoke(currentUser!);

                if (!isUserAuthorized)
                {
                    // if user condition not passed, then this discount not associate to user
                    continue;
                }
            }

            if (productConditionTree is null)
            {
                continue;
            }

            rule.ProductConditionTree = productConditionTree;
            applyableRules.Add(rule);
        }

        return applyableRules;
    }

    public async Task<PagedResultDto<ProductDto>> GetProducts(ProductQuery query)
    {
        var activeDiscounts = await unitOfWork.DiscountRepository.GetActiveDiscountsOfTypeAsync(DiscountType.Products);

        var rules = activeDiscounts.SelectMany(
            (discount) => discount.Rules!
        );

        var products = await unitOfWork.Products.GetAllProjectedAsync(query);

        ApplyRulesToProducts(products.Items, rules);

        foreach (var product in products.Items)
        {
            ResolveProductMediaUrl(product);
        }

        return products;
    }

    public async Task<Models.Product?> DeleteProduct(int id)
    {
        if (!await productPermissionHelper.HasProductDeletePermission(UserIdOrThrow))
        {
            throw new NotAllowedException("Not authorized", "You have not access to delete product");
        }

        var product = await unitOfWork.Products.GetModelByIdAsync(id);
        if (product == null)
        {
            return null;
        }

        await unitOfWork.Products.Delete(product);
        await unitOfWork.Complete();

        return product;
    }

    public async Task<ProductDto> CreateProduct(ProductCreationDto product, string userId)
    {
        if (!await productPermissionHelper.HasProductCreatePermission(userId))
        {
            throw new NotAllowedException("Not authorized", "You are not authorized to create a product");
        }

        int? createdProductId = default;
        using (var trx = await unitOfWork.StartTransaction())
        {
            try
            {
                var createdProduct = await _CreateProduct(product, userId);

                await trx.Commit();

                createdProductId = createdProduct.ProductId;

                var variationData = await productServiceHelper.RegisterCreatedProductMedia(createdProduct.ProductId, product);

                foreach (var pvData in variationData)
                {
                    var variation = createdProduct.Variations.ElementAt(pvData.VariationIndex);

                    if (variation is not null && pvData.VariationMedia is not null)
                    {
                        variation.Media.Add(pvData.VariationMedia);
                    }
                }

                await unitOfWork.Complete();

                return (await GetProductById(
                    createdProduct.ProductId,
                    discountCalculation: new()
                    {
                        DiscountCalculation = false,
                        UsersScore = false
                    },
                    query: ProductsDefaultQuery.Query.CopyWith(
                        new ProductQuery
                        {
                            Include = "variations,components"
                        }
                     )
                ))!;

            }
            catch
            {
                if (createdProductId is not null)
                {
                    return (await unitOfWork.Products.GetByIdProjectedAsync(createdProductId.Value))!;
                }

                await trx.Rollback();
                throw;
            }
        }
    }

    private async Task SetProductCategoryValues(Models.Product product, ProductCategoryValuesDto categoryValuesDto)
    {
        var categoryId = categoryValuesDto.CategoryId;
        product.CategoryId = categoryId;

        var productCategory = unitOfWork.Categories.GetByIdProjectedAsync(categoryId).Result;

        if (productCategory is null)
        {
            throw new ModelException(
                title: "Category not found",
                description: $"Category with id {categoryId} not exist",
                fieldName: $"{nameof(ProductCreationDto.CategoryValues)}.{nameof(ProductCreationDto.CategoryValues.CategoryId)}"
            );
        }


        if (categoryValuesDto.ComponentModels != null && categoryValuesDto.ComponentModels.Any())
        {
            var componentModelIds = categoryValuesDto.ComponentModels;

            var categoryValidModels = unitOfWork.Categories.GetModelsOfCategory(categoryId, componentModelIds).Result;

            for (var index = 0; index < componentModelIds.Count(); index++)
            {
                var modelId = componentModelIds.ElementAt(index);
                var componentModel = categoryValidModels.FirstOrDefault((cvm) => cvm.ComponentModelId == modelId);

                if (componentModel is null)
                {
                    throw new ModelException(
                        title: "Owning problem",
                        description: $"specified component model-id is not belong to a component of the '{productCategory.Name}' category",
                        fieldName: $"{nameof(ProductCreationDto.CategoryValues)}.{nameof(ProductCreationDto.CategoryValues.ComponentModels)}.{index}"
                    );
                }

                product.ComponentModels.Add(componentModel);
            }

        }

        var categoryProperties = productCategory.Properties;

        product.Properties = new List<ProductPropertyValue>();

        for (int index = 0; index < (categoryValuesDto.Properties?.Count() ?? 0); index++)
        {
            var prop = categoryValuesDto.Properties!.ElementAt(index);

            // some value must specified for property
            if (prop.PropertyValue is null)
            {
                throw new ModelException(
                    title: "No value",
                    description: "PropertyValue is required",
                    fieldName: $"{nameof(ProductCreationDto.CategoryValues)}.{nameof(ProductCreationDto.CategoryValues.Properties)}.{index}.{nameof(PropertyValueEntryCreationDto.PropertyValue)}"
                );
            }

            var ppv = new ProductPropertyValue
            {
                PropertyId = prop.PropertyId,

            };

            // get the property which this item target to
            var actualProp = categoryProperties?.FirstOrDefault((p) => p.PropertyId == prop.PropertyId);

            if (actualProp is null)
            {
                throw new ModelException(
                    title: "Not found",
                    description: $"No PropertyId with id {prop.PropertyId} exist for specified category.",
                    fieldName: $"{nameof(ProductCreationDto.CategoryValues)}.{nameof(ProductCreationDto.CategoryValues.Properties)}.{index}.{nameof(PropertyValueEntryCreationDto.PropertyId)}"
                );
            }

            var pv = new PropertyValue();

            try
            {
                pv.PopulateValue(actualProp.PropertyType!.Value, prop.PropertyValue);
            }
            catch (PropertyValueTypeDismatchException ex)
            {
                throw new ModelException(
                    title: "Type dismatch",
                    description: ex.Message,
                    fieldName: $"{nameof(ProductCreationDto.CategoryValues)}.{nameof(ProductCreationDto.CategoryValues.Properties)}.{index}.{nameof(PropertyValueEntryCreationDto.PropertyValue)}"
                );
            }
            ppv.Value = pv;
            product.Properties.Add(ppv);
        }
    }

    private async Task<Core.Models.Product> _CreateProduct(ProductCreationDto product, string userId)
    {
        var createdProduct = mapper.Map<Core.Models.Product>(product);

        if (product.BrandModel is not null)
        {
            var brandModel = unitOfWork.BrandModelRepository.GetModelByIdAsync(product.BrandModel.Value).Result;
            if (brandModel is null)
            {
                throw new ModelException(
                    title: "Bad request",
                    description: $"Specified brandModel with id '{product.BrandModel}' not exist",
                    fieldName: $"{nameof(ProductCreationDto.BrandModel)}.{nameof(ProductCreationDto.BrandModel.Value)}"
                );
            }

            createdProduct.BrandModel = brandModel;
        }

        // register product properties
        if (product.CategoryValues is not null)
        {
            await SetProductCategoryValues(createdProduct, new()
            {
                CategoryId = product.CategoryValues.CategoryId,
                ComponentModels = product.CategoryValues.ComponentModels,
                Properties = product.CategoryValues.Properties
            });
        }


        createdProduct.AuthorId = userId;

        await unitOfWork.Products.AddAsync(createdProduct);
        await unitOfWork.Complete();

        return createdProduct;
    }

    public async Task<PagedResultDto<ProductDto>> GetOnSaleProducts(ProductQuery? productQuery)
    {

        var activeDiscounts = await GetAppliableActiveDiscount();

        var products = await unitOfWork.DiscountedProductsRepository.GetDiscountedProducts(activeDiscounts, productQuery);

        ApplyRulesToProducts(products.Items, activeDiscounts);

        return products;
    }

    private void ResolveProductMediaUrl(ProductDto product)
    {
        foreach (var variation in product.Variations)
        {
            foreach (var media in variation.Media)
            {
                media.Url = media.Url is not null ? assetRegisterer.GetPublicUrl(media.Url) : null;
                media.ThumbnailUrl = media.ThumbnailUrl is not null ? assetRegisterer.GetPublicUrl(media.ThumbnailUrl) : null;
            }
        }
    }

    public async Task<ProductDto?> GetProductById(int productId, ProductAccessAdditionalProcessing? discountCalculation = default, ProductQuery? query = default)
    {
        var activeDiscounts = await GetAppliableActiveDiscount();

        var product = await unitOfWork.Products.GetByIdAsync(
            productId,
            discountCalculation?.UsersScore == true ? UserId : default,
            query
        );

        if (discountCalculation?.DiscountCalculation == true && product is not null)
        {
            ApplyRulesToProducts([product], activeDiscounts);
        }

        if (product is not null)
        {
            ResolveProductMediaUrl(product);
        }

        return product;
    }

    public async Task<IEnumerable<ProductDto>> GetSimilarProductsOf(int productId, ProductQuery? productQuery = default)
    {
        var isProductExist = await unitOfWork.Products.IsExistsAsync(productId);

        if (!isProductExist)
        {
            throw new NotFoundException($"No product with id {productId} found");
        }

        var similarProducts = await unitOfWork.Products.GetSimilarProductsOf(productId, productQuery);

        return similarProducts;
    }

    public async Task<PagedResultDto<ProductDto>> GetUsersProducts(ProductQuery? productQuery = default)
    {
        var user = await GetUser();

        var products = await unitOfWork.Products.GetPoductsOfUser(user.Id, ProductsDefaultQuery.Query.CopyWith(productQuery));

        return products;
    }


    private async Task<Core.Models.Product> GetAuthorizedProduct(int productId)
    {
        var user = await GetUser();

        var product = await unitOfWork.Products.GetModelByIdAsync(productId);

        if (product is null)
        {
            throw new NotFoundException($"product with id {productId} not found");
        }

        if (product.AuthorId != user.Id)
        {
            throw new NotAllowedException("you have not authorized to update this product");
        }

        return product;
    }

    public async Task<ProductBasicInfoDto> UpdateProduct(int productId, UpdateProductDto? updateDto)
    {
        if (!await productPermissionHelper.HasProductEditPermission(UserIdOrThrow))
        {
            throw new NotAllowedException("Not authorized", "You have not access to edit a product");
        }

        var product = await GetAuthorizedProduct(productId);

        if (updateDto is not null)
        {
            product.Title = updateDto.Title ?? product.Title;
            product.Description = updateDto.Description ?? product.Description;

            if (updateDto.BrandModelId is not null)
            {

                var brandModelId = updateDto.BrandModelId.Value;

                if (brandModelId != product.BrandModelId)
                {
                    var brandModel = await unitOfWork.BrandModelRepository.GetModelByIdAsync(brandModelId);

                    if (brandModel is null)
                    {
                        throw new NotFoundException("brand model not found", $"no brand model with id {brandModelId} exists");
                    }

                    product.BrandModel = brandModel;
                }

                await unitOfWork.Complete();
            }
        }

        return new()
        {
            ProductId = product.ProductId,
            Title = product.Title,
            Description = product.Description,
            AuthorId = product.AuthorId,
            BrandModel = product.BrandModelId is null ? default : await unitOfWork.BrandModelRepository.GetByIdProjectedAsync(product.BrandModelId.Value)
        };
    }

    public async Task<ProductDto> UpdateProductsCategory(int productId, ProductCategoryValuesDto replaceDto)
    {
        if (!await productPermissionHelper.HasProductEditPermission(UserIdOrThrow))
        {
            throw new NotAllowedException("Not authorized", "You are not authorized to edit product");
        }

        var product = await GetAuthorizedProduct(productId);

        if (replaceDto is not null)
        {
            using var trx = await unitOfWork.StartTransaction();

            try
            {
                // remove old data
                product.Properties.Clear();
                product.ComponentModels.Clear();

                await unitOfWork.Complete();


                // set new data
                await SetProductCategoryValues(product, replaceDto);

                await unitOfWork.Complete();

                await trx.Commit();
            }
            catch
            {
                await trx.Rollback();
                throw;
            }
        }

        return (await unitOfWork.Products.GetByIdProjectedAsync(productId))!;
    }
}