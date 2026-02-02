

using HiTechStore.Core;
using HiTechStore.Models;

namespace HiTechStore.Data.Seeders
{
    public class ColorSeeder
    {
        public static async Task SeedAsync(IUnitOfWork unitOfWork)
        {
            if (await unitOfWork.ColorRepository.HasAnyAsync())
            {
                return;
            }
            await unitOfWork.ColorRepository.AddAllAsync(
                [
                    new Color{
                        Name = "Gold",
                        Code = "FFD41D"
                    },
                    new Color{
                        Name = "Gray",
                        Code = "BFC9D1"
                    },
                    new Color{
                        Name = "Red",
                        Code = "C40C0C"
                    },
                    new Color{
                        Name = "Blue",
                        Code = "007FFF"
                    },
                    new Color{
                        Name = "Green",
                        Code= "568203"
                    },
                    new Color{
                        Name = "Pink",
                        Code= "FF91AF"
                    },
                    new Color{
                        Name = "Black",
                        Code = "000000"
                    },
                    new Color {
                        Name = "Brown",
                        Code = "3D2B1F"
                    },
                    new Color{
                        Name = "Orange",
                        Code = "BF5700"
                    },
                    new Color {
                        Name= "White",
                        Code= "F2F3F4"
                    },
                    new Color {
                        Name = "Violet",
                        Code = "8A2BE2"
                    }
                ]
            );
            await unitOfWork.Complete();
        }
    }
}