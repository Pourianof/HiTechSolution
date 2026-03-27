using HiTechStore.Core;
using HiTechStore.Core.Services.Discount;

namespace HiTechStore.Data.Seeders;


public class DiscountEntitySeeder
{
    public static async Task SeedAsync(IUnitOfWork unitOfWork)
    {
        await unitOfWork.DiscountEntityRepository.AddAllSafeAsync(
            [
                new()
                {
                    Name = "user",
                    Description = "Targeting user",
                    Properties= [
                        new (){
                            Name = "Total orders",
                            Description = "Total orders which the user has purchased",
                            Type = Models.DiscountEntityPropertyType.Int,
                            Path = UserDiscountEntity.TotalOrders.Path
                        },
                        new (){
                            Name = "Last order",
                            Description = "Last orders which the user has purchased",
                            Type = Models.DiscountEntityPropertyType.Object,
                            Path = UserDiscountEntity.LastOrder.Path,
                            SubEntity = new (){
                                Name = "Order",
                                Description = "User order item",
                                Properties = [
                                    new (){
                                        Name = "Purchase date",
                                        Description= "The date user purchased the order",
                                        Type = Models.DiscountEntityPropertyType.Date,
                                        Path = UserDiscountEntity.LastOrder.OrderDiscountEntity.PurchaseDate.Path,
                                    },
                                    new (){
                                        Name = "Items counts",
                                        Description= "The total items in the orders cart",
                                        Type = Models.DiscountEntityPropertyType.Int,
                                        Path = UserDiscountEntity.LastOrder.OrderDiscountEntity.ItemsCount.Path,
                                    },
                                    new (){
                                        Name = "Price",
                                        Description= "Total price of order",
                                        Type = Models.DiscountEntityPropertyType.Float,
                                        Path = UserDiscountEntity.LastOrder.OrderDiscountEntity.Price.Path,
                                    },
                                ]
                            }
                        }
                    ]
                },
                new()
                {
                    Name = "cart",
                    Description = "Targeting active cart of user",
                    Properties= [
                        new (){
                            Name = "Price",
                            Description = "Price of the cart",
                            Path = CartDiscountEntity.Price.Path,
                        }
                    ]
                },
                new (){
                    Name = "Product",
                    Description = "Discount based on product entities",
                    Properties=[
                        new Models.DiscountEntityProperty{
                            Name = "Price",
                            Description ="Product selling price",
                            Path = ProductDiscountEntity.Price.Path,
                            Type = Models.DiscountEntityPropertyType.Float,
                        },
                        new (){
                            Name = "Category",
                            Description ="Category of product",
                            Path = ProductDiscountEntity.Category.Path,
                            Type = Models.DiscountEntityPropertyType.Int,
                        },
                        new (){
                            Name = "Inventory",
                            Description ="Product selling price",
                            Path = ProductDiscountEntity.Inventory.Path,
                            Type = Models.DiscountEntityPropertyType.Int,
                        }
                    ]
                }
            ]
        );
    }

}