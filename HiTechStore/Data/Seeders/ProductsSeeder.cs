using HiTechStore.Core;
using HiTechStore.Models;

using Microsoft.AspNetCore.Identity;

namespace HiTechStore.Data.Seeders
{
    public class ProductsSeeder
    {
        static private IUnitOfWork uow { get; set; } = default!;
        static private UserManager<User> userManager { get; set; } = default!;

        static private async Task<BrandModel> GetBrandModel(string brandName, string modelName) => (await uow.BrandRepository.GetByNameAsync(brandName))!.Models!.First(
                            c => string.Equals(c.Name, modelName, StringComparison.OrdinalIgnoreCase));
        public static async Task SeedAsync(IUnitOfWork uow, UserManager<User> userManager)
        {
            if (await uow.Products.HasAnyAsync())
            {
                return;
            }
            ProductsSeeder.uow = uow;
            ProductsSeeder.userManager = userManager;

            await AddProduct1();
            await AddProduct2();

        }

        static private async Task AddProduct1()
        {
            var storageComponent = uow.ComponentRepository.GetByNameAsync("Storage Size").Result.First();
            var ramComponent = uow.ComponentRepository.GetByNameAsync("RAM").Result.First();

            var product = new Product
            {
                Title = "Lenovo IdeaPad  V15 G4 Intel Core i5 8GB RAM 512GB SSD 15.6 Inch Wndows 11 Laptop",
                Description = "Some test description",
                Category = uow.Categories.GetCategoriesByName("laptop").First(),
                Author = await userManager.FindByNameAsync("manager"),
                BrandModel = await GetBrandModel("lenovo", "ideapad"),
                Variations = [
                    new ProductVariation{
                        Color = await uow.ColorRepository.GetColorByNameAsync("gray"),
                        Inventory = 20,
                        Media = [
                            new ProductMedia{
                                FilePath = "ideapad_gray.jpg",
                                IsMain = true,
                                Type =MediaType.Image
                            }
                        ],
                        Price = 800
                    },
                        new ProductVariation{
                        Color = await uow.ColorRepository.GetColorByNameAsync("black"),
                        Inventory = 12,
                        Media = [
                            new ProductMedia{
                                FilePath = "ideapad_black.jpg",
                                IsMain = true,
                                Type =MediaType.Image
                            }
                        ],
                        Price = 900
                    }
                ],
            };
            await uow.Products.AddAsync(product);
            await uow.Complete();
            product.ComponentModels = [new ComponentModel
            {
                ComponentType =ramComponent,
                Description = "RAM Size of Lenovo Ideapad V15 G4",
                Properties = [
                    new ComponentPropertyValue{
                        Property = ramComponent.Properties!.First(
                            prop=>prop.Name == "Capacity"),
                        Value = new PropertyValue{
                            ValueNumber = 32
                        }
                    },
                ]
            },
            new ComponentModel
            {
                ComponentType = storageComponent,
                Description = "Internal storage size",
                Properties = [
                    new ComponentPropertyValue{
                        Property = storageComponent.Properties!.First(
                            prop=>prop.Name == "Size"),
                        Value = new PropertyValue{
                            ValueNumber = 1000
                        }
                    },
                ]
            }];
            await uow.Complete();

        }

        static private async Task AddProduct2()
        {

            var storageComponent = uow.ComponentRepository.GetByNameAsync("Storage Size").Result.First();
            var ramComponent = uow.ComponentRepository.GetByNameAsync("RAM").Result.First();


            var product = new Product
            {
                Title = "Apple iPhone 17 Pro Max, US Version, 256GB, eSIM",
                Description = "Some test description",
                Category = uow.Categories.GetCategoriesByName("smart phone").First(),
                Author = await userManager.FindByNameAsync("manager"),
                BrandModel = await GetBrandModel("apple", "iphone"),
                Variations = [
                    new ProductVariation{
                        Color = await uow.ColorRepository.GetColorByNameAsync("orange"),
                        Inventory = 10,
                        Media = [
                            new ProductMedia{
                                FilePath = "iphone_17_pro_max_orange.jpg",
                                IsMain = true,
                                Type =MediaType.Image
                            }
                        ],
                        Price = 1380
                    },
                    new ProductVariation{
                        Color = await uow.ColorRepository.GetColorByNameAsync("blue"),
                        Inventory = 15,
                        Media = [
                            new ProductMedia{
                                FilePath = "iphone_17_pro_max_blue.jpg",
                                IsMain = true,
                                Type =MediaType.Image
                            }
                        ],
                        Price = 1350
                    }
                ],
                ComponentModels = [
                    new ComponentModel
            {
                ComponentType = storageComponent,
                Description = "Internal storage of IPhone 17 pro max 256GB",
                Properties = [
                    new ComponentPropertyValue{
                        Property = uow.ComponentRepository.GetByNameAsync("Storage Size").Result.First().Properties!.First(
                            prop=>prop.Name == "Size"),
                        Value = new PropertyValue{
                            ValueNumber = 256,
                        }
                    },
                ]
            },
                    new ComponentModel
            {
                ComponentType = ramComponent,
                Description = "Ram for Iphone 17 pro max",
                Properties = [
                     new ComponentPropertyValue{
                            Property = uow.ComponentRepository.GetByNameAsync("RAM").Result.First().Properties!.First(
                                prop=>prop.Name == "Capacity"),
                            Value = new PropertyValue{
                                ValueNumber = 8
                            }
                        },
                    ]
            }
                ]
                // Foreign key constraints violation error. its seems about nullity of FK column 
                // when the propertValue is associate with component and is a category property
                // Properties = [
                //     new ProductPropertyValue{
                //         Property = uow.Categories.GetCategoriesByName("smart phone").First().Properties!.First(
                //             prop=> prop.Name == "Operating System"
                //         ),
                //         Value = new PropertyValue{
                //             ValueString = "IOS"
                //         }
                //     }
                // ],
            };
            await uow.Products.AddAsync(product);
            await uow.Complete();
        }
    }
}