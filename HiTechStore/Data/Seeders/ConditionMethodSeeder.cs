using HiTechStore.Core;

namespace HiTechStore.Data.Seeders;


public static class ConditionMethodSeeder
{
    public static async Task<IUnitOfWork> SeedConditionMethodssAsync(this IUnitOfWork unitOfWork)
    {
        await unitOfWork.ConditionMethodRepository.AddAllSafe([
            new (){
                Name = "Any",
                ReturnType = Models.DiscountEntityPropertyType.Boolean
            },
            new (){
                Name = "Count",
                ReturnType = Models.DiscountEntityPropertyType.Int
            },
        ]);

        return unitOfWork;
    }

}