using HiTechStore.Core;
using HiTechStore.Models;

namespace HiTechStore.Data.Seeders;


public static class DiscountEntitySeeder
{
    public static async Task<IUnitOfWork> SeedDiscountEntitiesAsync(this IUnitOfWork unitOfWork)
    {
        var orderItemEntity = new DiscountEntity
        {
            Name = nameof(OrderItem),
            Description = "Items of an order",
            Properties = [
                new(){
                    Name = nameof(OrderItem.Count),
                    Description = "The amount of an item ordered",
                    Type = DiscountEntityPropertyType.Int,
                },
                new(){
                    Name = nameof(OrderItem.OrderPayTimePrice),
                    Description = "Final price which paid for single item",
                    Type= DiscountEntityPropertyType.Float
                },
            ]
        };

        var orderEntity = new DiscountEntity()
        {
            Name = nameof(Order),
            Description = "User order item",
            Properties = [
                new (){
                    Name = nameof(Order.CreatedAt),
                    Description= "The date user purchased the order",
                    Type = DiscountEntityPropertyType.Date,
                },
                new (){
                    Name = nameof(Order.Items),
                    Description= "Items of order",
                    Type = DiscountEntityPropertyType.Array,
                    SubEntity = orderItemEntity
                },
            ]

        };

        var productVariationEntity = new DiscountEntity()
        {
            Name = nameof(ProductVariation),
            Description = "Defining variations of products",
            Properties = [
                new (){
                    Name = nameof(ProductVariation.Price),
                    Description ="Product selling price",
                    Type = DiscountEntityPropertyType.Float,
                },
                new (){
                    Name = nameof(ProductVariation.Inventory),
                    Description ="Product variation selling price",
                    Type = DiscountEntityPropertyType.Int,
                },
                new (){
                    Name = nameof(ProductVariation.Orders),
                    Description = "Order items which targeting this variation",
                    Type = DiscountEntityPropertyType.Array,
                    SubEntity = orderItemEntity
                }
            ]
        };

        var productEntity = new DiscountEntity()
        {
            Name = nameof(Product),
            Description = "Discount based on product entities",
            Properties = [
                new DiscountEntityProperty{
                    Name = nameof(Product.Variations),
                    Description ="Variations of a product",
                    Type = DiscountEntityPropertyType.Array,
                    SubEntity = productVariationEntity
                },
                new (){
                    Name = nameof(Product.CategoryId),
                    Description ="Category of product",
                    Type = DiscountEntityPropertyType.Int,
                },

            ]
        };

        productVariationEntity.Properties.Add(
            new()
            {
                Name = nameof(ProductVariation.Product),
                Description = "The product which this variation belongs to",
                Type = DiscountEntityPropertyType.Object,
                SubEntity = productEntity
            }
        );
        orderItemEntity.Properties.Add(
            new()
            {
                Name = nameof(OrderItem.ProductVariation),
                Description = "Product variation which order item targetting",
                Type = DiscountEntityPropertyType.Object,
                SubEntity = productVariationEntity
            }
        );


        var cartEntity = new DiscountEntity
        {
            Name = nameof(Cart),
            Description = "Active cart of user",
            Properties = [
                new (){
                    Name = nameof(Cart.Items),
                    Description = "Items in cart",
                    Type = DiscountEntityPropertyType.Array,
                    SubEntity = new(){
                        Name = nameof(CartItem),
                        Description = "Item in a cart",
                        Properties= [
                            new(){
                                Name = nameof(CartItem.Amount),
                                Description = "Amount of items user added to its cart",
                                Type =DiscountEntityPropertyType.Int
                            },
                            new(){
                                Name = nameof(CartItem.ProductVariation),
                                Description = "The product variation user selected",
                                Type =DiscountEntityPropertyType.Object,
                                SubEntity = productVariationEntity
                            }
                        ]
                    }
                }
            ]
        };

        var userEntity = new DiscountEntity
        {
            Name = nameof(User),
            Description = "Targeting user",
            Properties = [
                new (){
                    Name = nameof(User.Orders),
                    Description = "All orders which belongs to user",
                    Type = DiscountEntityPropertyType.Array,
                    SubEntity = orderEntity,
                },
                new (){
                    Name = nameof(User.ActiveCart),
                    Description = "User's active cart",
                    Type = DiscountEntityPropertyType.Object,
                    SubEntity = cartEntity,
                },
                 new (){
                    Name = nameof(User.RegisteredAt),
                    Description = "User's registration time",
                    Type = DiscountEntityPropertyType.Date,
                    SubEntity = cartEntity,
                }
            ]
        };

        await unitOfWork.DiscountEntityRepository.AddAllSafeAsync(
            [
                orderItemEntity,
                orderEntity,
                productVariationEntity,
                productEntity,
                cartEntity,
                userEntity,
            ]
        );

        return unitOfWork;
    }

}