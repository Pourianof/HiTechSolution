

using HiTechStore.Core;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.Seeders
{
    public class ComponentModelSeeder
    {
        public static async Task SeedAsync(IUnitOfWork unitOfWork)
        {
            var gpuComponent = unitOfWork.ComponentRepository.GetByNameAsync("GPU").Result.First();

            gpuComponent.ComponentModels = [
                new ComponentModel{
                    BrandModel = unitOfWork.BrandRepository.GetByNameAsync("nvidia").Result!.Models!.First(
                        model => model.Name!.Contains("4060")
                    ),
                    Properties = [
                        new ComponentPropertyValue{
                            Property = gpuComponent.Properties!.First(
                                prop=> prop.Name == "Size"
                            ),
                            Value = new PropertyValue{
                                ValueNumber = 8
                            }
                        }
                    ]
                }

            ];

            await unitOfWork.Complete();
        }
    }
}