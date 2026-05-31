using HiTechStore.Core;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.Seeders
{
    public class BrandSeeder
    {
        public static async Task SeedAsync(IUnitOfWork context)
        {
            if (await context.BrandRepository.HasAnyAsync())
            {
                return;
            }

            await context.BrandRepository.AddAllAsync(
                [
                    new Brand{
                        Name = "Apple",
                        Models = [
                            new BrandModel{
                                Name = "Macbook",
                                Description = "Apples laptop series",
                            },
                            new BrandModel {
                                Name = "IPhone",
                                Description ="Apple's smart-phone series"
                            },
                            new BrandModel {
                                Name = "IPad",
                                Description ="Apple's tablet series"
                            }
                        ]
                    },
                    new Brand{
                        Name = "Lenovo",
                        Models = [
                            new BrandModel{
                                Name ="Ideapad",
                                Description = "Economist series"
                            }
                        ]
                    },
                    new Brand {
                        Name = "NVidia",
                        Models = [
                            new BrandModel{
                                Name = "GeForce RTX™ 4090",
                            },
                            new BrandModel{
                                Name = "GeForce RTX™ 4080"
                            },
                            new BrandModel{
                                Name = "GeForce RTX™ 4070"
                            },
                            new BrandModel{
                                Name = "GeForce RTX™ 4060"
                            }
                        ]
                    }
                ]
            );
            await context.Complete();
        }
    }
}