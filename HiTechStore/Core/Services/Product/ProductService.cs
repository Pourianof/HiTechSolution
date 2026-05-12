

using AutoMapper;

using HiTechStore.Core.Auth;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Helpers;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Mapping;
using HiTechStore.Data.Queries;
using HiTechStore.Data.Storage;
using HiTechStore.DTOs.Product;
using HiTechStore.Helpers.Types;
using HiTechStore.Models;

namespace HiTechStore.Core.Services.Product;

public class ProductService(
    IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider,
    IDiscountConditionScriptParser scriptParser,
    IMapper mapper,
    IConditionComponentTreeToLambdaExpression conditionTreeToLambdaExprMapper,
    ProductServiceHelper productServiceHelper
) : IProductService
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

        var currentUser = currentUserProvider.UserId is null ?
                            default :
                            await unitOfWork.UserRepository.GetUserByIdAsync(currentUserProvider.UserId);
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

        return products;
    }

    public async Task<Models.Product?> DeleteProduct(int id)
    {
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

                return (await unitOfWork.Products.GetByIdProjectedAsync(createdProduct.ProductId))!;

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

    private async Task<Models.Product> _CreateProduct(ProductCreationDto product, string userId)
    {
        var createdProduct = mapper.Map<Models.Product>(product);

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

            // setting product category
            var categoryId = product.CategoryValues.CategoryId!.Value;
            createdProduct.CategoryId = categoryId;

            var productCategory = unitOfWork.Categories.GetByIdProjectedAsync(categoryId).Result;

            if (productCategory is null)
            {
                throw new ModelException(
                    title: "Category not found",
                    description: $"Category with id {categoryId} not exist",
                    fieldName: $"{nameof(ProductCreationDto.CategoryValues)}.{nameof(ProductCreationDto.CategoryValues.CategoryId)}"
                );
            }


            if (product.CategoryValues.ComponentModels != null && product.CategoryValues.ComponentModels.Any())
            {
                var componentModelIds = product.CategoryValues.ComponentModels;

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

                    createdProduct.ComponentModels.Add(componentModel);
                }


            }

            var categoryProperties = productCategory.Properties;

            createdProduct.Properties = new List<ProductPropertyValue>();

            for (int index = 0; index < (product.CategoryValues.Properties?.Count() ?? 0); index++)
            {
                var prop = product.CategoryValues.Properties!.ElementAt(index);

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
                    PropertyId = prop.PropertyId!.Value,

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
                createdProduct.Properties.Append(ppv);
            }
        }


        createdProduct.AuthorId = userId;

        await unitOfWork.Products.AddAsync(createdProduct);
        await unitOfWork.Complete();

        return createdProduct;
    }

    public async Task<PagedResultDto<ProductDto>> GetOnSaleProducts()
    {

        var activeDiscounts = await GetAppliableActiveDiscount();

        var products = await unitOfWork.DiscountedProductsRepository.GetDiscountedProducts(activeDiscounts);

        ApplyRulesToProducts(products.Items, activeDiscounts);

        return products;
    }

    public async Task<ProductDto?> GetProductById(int productId, ProductAccessAdditionalProcessing? discountCalculation = default)
    {
        var activeDiscounts = await GetAppliableActiveDiscount();

        var product = await unitOfWork.Products.GetByIdAsync(
            productId,
            discountCalculation?.UsersScore == true ? currentUserProvider.UserId : default
        );

        if (discountCalculation?.DiscountCalculation == true && product is not null)
        {
            ApplyRulesToProducts([product], activeDiscounts);
        }

        return product;
    }

    public async Task<IEnumerable<ProductDto>> GetSimilarProductsOf(int productId)
    {
        var isProductExist = await unitOfWork.Products.IsExistsAsync(productId);

        if (!isProductExist)
        {
            throw new NotFoundException($"No product with id {productId} found");
        }

        var similarProducts = await unitOfWork.Products.GetSimilarProductsOf(productId);

        return similarProducts;
    }
}