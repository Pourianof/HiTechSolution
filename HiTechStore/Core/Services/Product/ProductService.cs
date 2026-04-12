
using AutoMapper;

using HiTechStore.Core.Auth;
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
    IServiceProvider serviceProvider,
    ICurrentUserProvider currentUserProvider,
    IDiscountConditionScriptParser scriptParser,
    IMapper mapper,
    IPublicAssetRegisterer assetRegisterer
) : IProductService
{
    public async Task<PagedResultDto<ProductDto>> GetProducts(ProductQuery query)
    {
        var activeDiscounts = await unitOfWork.DiscountRepository.GetActiveDiscountsAsync();

        var rules = activeDiscounts.SelectMany(
            (discount) => discount.Rules!
        );

        var products = await unitOfWork.Products.GetAllProjectedAsync(query);

        var currentUser = currentUserProvider.UserId is null ?
                            default :
                            await unitOfWork.UserRepository.GetUserByIdAsync(currentUserProvider.UserId);
        var isAuthorized = currentUser is not null;

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

                var userToExprMapper = serviceProvider.GetRequiredService<IConditionComponentTreeToLambdaExpression>();
                var userEvaluator = userToExprMapper.Map<User>(userConditionTree);
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

            // need new instance for every process and remove previous state
            var conditionToExprMapper = serviceProvider.GetRequiredService<IConditionComponentTreeToLambdaExpression>();
            var filterExpr = conditionToExprMapper.Map<ProductDto>(productConditionTree, nameof(Product));

            var items = products.Items.Where(filterExpr.Compile());

            foreach (var variation in items.SelectMany(i => i.Variations))
            {
                variation.Discount += rule.DiscountAction!.Type == DiscountActionType.Percent ?
                    variation.Price * (double)rule.DiscountAction.Value! / 100 :
                    (double)rule.DiscountAction.Value;
            }
        }

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

    public async Task<ProductScore> ScoreProduct(int productId, ProductScoreDto score, string userId)
    {
        // check if is any score registered by this user for this product before
        var existingScore = await unitOfWork.ProductScores.GetUserScoreForProductAsync(userId, productId);
        if (existingScore != null)
        {
            // if exist delete it
            await unitOfWork.ProductScores.Delete(existingScore);
        }

        // register new one
        var newScore = new ProductScore
        {
            UserId = userId,
            ProductId = productId,
            Score = score.Score // default score
        };
        await unitOfWork.ProductScores.AddAsync(newScore);
        await unitOfWork.Complete();

        return newScore;
    }

    public async Task<ProductDto> CreateProduct(ProductCreationDto product, string userId)
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

        try
        {
            foreach (var variation in product.Variations!)
            {
                var variationMedia = variation.MediaMetaData!.Select(
                    (meta) => new
                    {
                        File = product.Media!.ElementAt(meta.Index),
                        meta.IsMain
                    }
                );

                for (int index = 0; index < variationMedia.Count(); index++)
                {
                    var media = variationMedia.ElementAt(index);
                    var isImage = MediaTypeHelper.IsImage(media.File.FileName);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(media.File.FileName);
                    string fileRelativePath = Path.Combine("images", "products", createdProduct.ProductId.ToString(), fileName);
                    await assetRegisterer.WriteIFormFile(media.File, fileRelativePath);

                    var createdVariation = createdProduct.Variations.First(
                        v => v.ColorId == variation.Color
                    );
                    createdVariation.Media.Add(new ProductMedia { FilePath = $"/{fileRelativePath}", IsMain = media.IsMain, Type = MediaTypeHelper.GetMediaType(fileRelativePath) });

                }
            }

        }
        catch
        {
            await unitOfWork.Products.Delete(createdProduct);
            throw;
        }

        // For updating the product media
        await unitOfWork.Complete();


        return (await unitOfWork.Products.GetByIdProjectedAsync(createdProduct.ProductId))!;
    }
}