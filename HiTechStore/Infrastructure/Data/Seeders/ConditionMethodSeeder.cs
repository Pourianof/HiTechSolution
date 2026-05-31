using HiTechStore.Core;

namespace HiTechStore.Infrastructure.Data.Seeders;


public static class ConditionMethodSeeder
{
    public static async Task<IUnitOfWork> SeedConditionMethodssAsync(this IUnitOfWork unitOfWork)
    {
        await unitOfWork.ConditionMethodRepository.AddAllSafe([
            new (){
                Name = "Any",
                ReturnType = Core.Models.DiscountEntityPropertyType.Boolean
            },
            new (){
                Name = "Count",
                ReturnType = Core.Models.DiscountEntityPropertyType.Int
            },
        ]);

        return unitOfWork;
    }

}