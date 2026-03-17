
using HiTechStore.Models;

namespace HiTechStore.Core.Services.Discount;

public abstract class BaseDiscountEntityInterpreter(IDiscountConditionValueComaprator comaprator)
{
    protected IDiscountConditionValueComaprator Comaprator = comaprator;
    abstract public Task<DiscountEntityInterpretionResult> Interpret(DiscountOperation operation, string conditionValue, IDiscountEntityResolverContext discountEntityResolverContext);
}

public class DiscountEntityInterpretionResult
{
    public bool IsProductBase { get; init; }
    public bool IsConditionPassed { get; init; }
    public IReadOnlyCollection<ProductVariation>? ConditionMatchedProducts { get; init; }
}

/**
    Responsible for handling how the target Criteria values extracted
    It mean it knows how to evalute the actual data for entity's property
**/
public static class UserDiscountEntity
{
    public const string Path = "user";

    [DiscountEntityMap(Path)]
    public class TotalOrders : BaseDiscountEntityInterpreter
    {
        public const string Path = $"{UserDiscountEntity.Path}/Total Orders";

        public TotalOrders(IDiscountConditionValueComaprator comaprator) : base(comaprator)
        {
        }

        override public async Task<DiscountEntityInterpretionResult> Interpret(DiscountOperation operation, string conditionValue, IDiscountEntityResolverContext context)
        {
            var orders = await context.UnitOfWork!.OrderRepository.GetUserOrders(context.User!.Id);
            var orderCounts = orders?.Count(
                order => order.PaymentState == OrderPaymentState.Paid
            ) ?? 0;

            return new()
            {
                IsProductBase = false,
                IsConditionPassed = Comaprator.Compare(orderCounts.ToString(), conditionValue, operation),
            };
        }
    }

    public static class LastOrder
    {
        public static class OrderDiscountEntity
        {
            public class PurchaseDate { }

            public class ItemsCount { }

            public class Price { }
        }
    }
}

public static class ProductDiscountEntity
{
    public const string Path = "Product";

    public abstract class BaseProductPropertiesInterpreter : BaseDiscountEntityInterpreter
    {
        protected BaseProductPropertiesInterpreter(IDiscountConditionValueComaprator comaprator) : base(comaprator)
        { }

        protected abstract Task<string> ProvideValue(ProductVariation productVariation);

        override public async Task<DiscountEntityInterpretionResult> Interpret(DiscountOperation operation, string conditionValue, IDiscountEntityResolverContext context)
        {
            var targetProducts = context.MatchedProducts is not null ? context.MatchedProducts : context.Cart?.Items.Select((item) => item.ProductVariation);

            if (targetProducts is null || !targetProducts.Any())
            {
                return new()
                {
                    IsProductBase = true,
                    IsConditionPassed = false,
                    ConditionMatchedProducts = new List<ProductVariation>()
                };
            }

            var productsWhichPassedCondition = new List<ProductVariation>();

            foreach (var prod in targetProducts)
            {
                var criteriaValue = await ProvideValue(prod!);
                if (Comaprator.Compare(criteriaValue, conditionValue, operation))
                {
                    productsWhichPassedCondition.Add(prod!);
                }
            }

            var IsConditionPassed = productsWhichPassedCondition.Any();

            return new()
            {
                IsProductBase = true,
                IsConditionPassed = IsConditionPassed,
                ConditionMatchedProducts = productsWhichPassedCondition
            };
        }
    }
    [DiscountEntityMap(Path)]
    public class Price : BaseProductPropertiesInterpreter
    {
        public const string Path = $"{ProductDiscountEntity.Path}/Price";

        public Price(IDiscountConditionValueComaprator comaprator) : base(comaprator)
        {
        }

        protected override Task<string> ProvideValue(ProductVariation productVariation)
        {
            return Task.FromResult(productVariation.Price.ToString());
        }
    }

}


public static class CartDiscountEntity
{
    public class Price
    {

    }
}