using HiTechStore.Core;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.Seeders
{
    public class CategorySeeder
    {
        public static async Task SeedAsync(IUnitOfWork uow)
        {
            if (await uow.Categories.HasAnyAsync())
            {
                return;
            }

            var ramComponent = uow.ComponentRepository.GetByNameAsync("ram").Result.First();
            var gpuComponent = uow.ComponentRepository.GetByNameAsync("gpu").Result.First();
            var cpuComponent = uow.ComponentRepository.GetByNameAsync("processor").Result.First();
            var storageComponent = uow.ComponentRepository.GetByNameAsync("storage size").Result.First();

            await uow.Categories.AddAllAsync(
                [
                    new Category{
                        Name = "Laptop",
                        Description = "All laptops stay here",
                        Components = [
                            new CategoryComponent{
                                Component = ramComponent
                            },
                            new CategoryComponent{
                                Component = cpuComponent
                            },
                            new CategoryComponent{
                                Component = gpuComponent
                            },
                            new CategoryComponent {
                                Component = storageComponent
                            }
                        ],
                        Properties = [
                            new Property{
                                Name = "Operating System",
                                Description = "Laptop operating systems",
                                PropertyType = PropertyType.String,
                            }
                        ]
                    },
                    new Category{
                        Name = "Smart Phone",
                        Description = "All smart phone from all brands",
                        Components = [
                            new CategoryComponent{
                                Component = ramComponent
                            },
                            new CategoryComponent{
                                Component = cpuComponent
                            },
                            new CategoryComponent{
                                Component = storageComponent
                            }
                        ],
                        Properties = [
                            new Property{
                                Name = "Operating System",
                                Description = "Smart phone operating systems",
                                PropertyType = PropertyType.String,
                            }
                        ]
                    },
                ]
            );
            await uow.Complete();
        }
    }
}