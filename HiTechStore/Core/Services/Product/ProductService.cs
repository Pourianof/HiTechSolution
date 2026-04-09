
using HiTechStore.Core.Helpers;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Mapping;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

namespace HiTechStore.Core.Services.Product;

public class ProductService(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IDiscountConditionScriptParser scriptParser) : IProductService
{
    public async Task<PagedResultDto<ProductDto>> GetProducts(ProductQuery query)
    {
        var activeDiscounts = await unitOfWork.DiscountRepository.GetActiveDiscountsAsync();

        var rules = activeDiscounts.SelectMany(
            (discount) => discount.Rules!
        );

        var products = await unitOfWork.Products.GetAllProjectedAsync(query);

        foreach (var rule in rules)
        {
            var conditionTree = scriptParser.Parse(rule.RawConditionScript!);

            // need new instance for every process and remove previous state
            var conditionToExprMapper = serviceProvider.GetRequiredService<IConditionComponentTreeToLambdaExpression>();
            var filterExpr = conditionToExprMapper.Map<ProductDto>(conditionTree!, nameof(Product));

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
}