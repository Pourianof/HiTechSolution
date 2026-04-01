
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
                IsConditionPassed = Comaprator.Compare(orderCounts, conditionValue, operation),
            };
        }
    }

    public static class LastOrder
    {
        public const string Path = $"{UserDiscountEntity.Path}/Last order";
        public static class OrderDiscountEntity
        {
            public const string Path = "Order";
            public class PurchaseDate
            {
                public const string Path = $"{OrderDiscountEntity.Path}/Purchase date";
            }

            public class ItemsCount
            {
                public const string Path = $"{OrderDiscountEntity.Path}/Items counts";

            }

            public class Price
            {
                public const string Path = $"{OrderDiscountEntity.Path}/Price";

            }
        }
    }
}


/*
    Note about interpreting product discount:
    The way that a discount(code or product-base) conditions get applied to
    some products, is somehow ambigiuos. Because each product may have mul-
    tiple variations which some of them pass the conditions but some of th-
    em not. For example the condition is Product.Price > 500, and one of t-
    he variation have 450$ price and another have 550$.
    The business rule i choosen for this problem, is the simplified one w-
    hic if any of variations pass the conditions then the whole product g-
    etting the discount. But it is more rational which some business expe-
    rt give some advice and change this rule.
*/
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

    public class Variations
    {
        public const string Name = "ProductVariation";
        public const string Path = $"{ProductDiscountEntity.Path}/Variations";

        public class Orders
        {
            public const string Name = "Orders";
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

    [DiscountEntityMap(Path)]
    public class Category : BaseProductPropertiesInterpreter
    {
        public const string Path = $"{ProductDiscountEntity.Path}/Category";

        public Category(IDiscountConditionValueComaprator comaprator) : base(comaprator)
        {
        }

        protected override Task<string> ProvideValue(ProductVariation productVariation)
        {
            return Task.FromResult(productVariation.Product!.CategoryId.ToString());
        }
    }

    [DiscountEntityMap(Path)]
    public class Inventory : BaseProductPropertiesInterpreter
    {
        public const string Path = $"{ProductDiscountEntity.Path}/Inventory";

        public Inventory(IDiscountConditionValueComaprator comaprator) : base(comaprator)
        {
        }

        protected override Task<string> ProvideValue(ProductVariation productVariation)
        {
            return Task.FromResult(productVariation.Product!.CategoryId.ToString());
        }
    }

}


public static class CartDiscountEntity
{
    public const string Path = "cart";
    public class Price
    {
        public const string Path = $"{CartDiscountEntity.Path}/Price";
    }
}

