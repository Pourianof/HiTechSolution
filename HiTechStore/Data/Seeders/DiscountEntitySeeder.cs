using HiTechStore.Core;

namespace HiTechStore.Data.Seeders;


public class DiscountEntitySeeder
{
    public static async Task SeedAsync(IUnitOfWork unitOfWork)
    {
        if (await unitOfWork.DiscountEntityRepository.HasAnyAsync())
        {
            return;
        }

        await unitOfWork.DiscountEntityRepository.AddAllAsync(
            [
                new()
                {
                    Name = "user",
                    Description = "Targeting user",
                    Properties= [
                        new (){
                            Name = "Total orders",
                            Description = "Total orders which the user has purchased",
                            Type = Models.DiscountEntityPropertyType.Int
                        },
                        new (){
                            Name = "Last order",
                            Description = "Last orders which the user has purchased",
                            Type = Models.DiscountEntityPropertyType.Object,
                            SubEntity = new (){
                                Name = "Order",
                                Description = "User order item",
                                Properties = [
                                    new (){
                                        Name = "Purchase date",
                                        Description= "The date user purchased the order",
                                        Type = Models.DiscountEntityPropertyType.Date
                                    },
                                    new (){
                                        Name = "Items counts",
                                        Description= "The total items in the orders cart",
                                        Type = Models.DiscountEntityPropertyType.Int
                                    },
                                    new (){
                                        Name = "Price",
                                        Description= "Total price of order",
                                        Type = Models.DiscountEntityPropertyType.Float
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
                        }
                    ]
                }
            ]
        );

        await unitOfWork.Complete();

    }

}