

using HiTechStore.Core;
using HiTechStore.Models;

namespace HiTechStore.Data.Seeders
{
    public class ComponentSeeder
    {
        public static async Task SeedAsync(IUnitOfWork unitOfWork)
        {
            if (await unitOfWork.ComponentRepository.HasAnyAsync())
            {
                return;
            }

            await unitOfWork.ComponentRepository.AddAllAsync(
                [
                   new ComponentType{
                        Name = "RAM",
                        Description = "Random Access Memory",
                        Properties = [
                            new Property{
                                Name = "Capacity",
                                Description = "How much capacity in Gigabyte",
                                Unit = "Gigabyte - Gb",
                                PropertyType = PropertyType.Number
                            }
                        ]
                    },
                    new ComponentType{
                        Name = "GPU",
                        Description = "Graphics Processing Unit",
                        Properties = [
                            new Property{
                                Name = "Size",
                                Description = "In-board Memory storage size",
                                Unit = "Gigabyte - Gb",
                                PropertyType = PropertyType.Number
                            },
                            new Property{
                                Name = "Clock",
                                Description = "Speed and fastness of gpu",
                                Unit = "Giga Hertz - Gh",
                                PropertyType = PropertyType.Number
                            }
                        ],
                    },
                    new ComponentType{
                        Name = "Processor",
                        Description = "CPU or Central Processor Unit is the brain of devices to process all the device need to do",
                        Properties = [
                            new Property{
                                Name = "Clock",
                                Description = "Describe how fast the processor is",
                                Unit = "Giga Hertz - Ghz",
                                PropertyType = PropertyType.Number
                            }
                        ]
                    },
                    new ComponentType{
                        Name = "Storage Size",
                        Description = "Storage size of device for saving data",
                        Properties = [
                            new Property{
                                Name = "Size",
                                Description = "Describe the capacity of storage",
                                Unit = "Giga bytes- Bb",
                                PropertyType = PropertyType.Number
                            },
                            new Property{
                                Name = "Write Speed",
                                Description = "Speed of writing data to storage",
                                Unit = "Giga Hertz - GH",
                                PropertyType = PropertyType.Number
                            },
                            new Property{
                                Name = "Read Speed",
                                Description = "Speed of writing data to storage",
                                Unit = "Giga Hertz - GH",
                                PropertyType = PropertyType.Number
                            }
                        ]
                    }
                ]
            );
            await unitOfWork.Complete();

            await ComponentModelSeeder.SeedAsync(unitOfWork);
        }
    }
}