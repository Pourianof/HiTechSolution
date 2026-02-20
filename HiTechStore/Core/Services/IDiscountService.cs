namespace HiTechStore.Core.Services;

public interface IDiscountService
{
    string GenerateRanomCode(int length = 10);
}