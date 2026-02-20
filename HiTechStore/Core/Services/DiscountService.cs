using System.Security.Cryptography;
using System.Text;

using HiTechStore.Core.Helpers;

namespace HiTechStore.Core.Services;

public class DiscountService(IUnitOfWork unitOfWork, IDiscountCodeGenerator codeGenerator) : IDiscountService
{
    private IUnitOfWork _unitOfWork = unitOfWork;
    private IDiscountCodeGenerator _codeGenerator = codeGenerator;
    public string GenerateRanomCode(int length = 10)
    {
        var hasGenerated = false;
        string? code = default;

        while (!hasGenerated)
        {
            code = _codeGenerator.GenerateCode(length);

            var discountCode = _unitOfWork.DiscountCodeRepository.GetDiscountCodeByNameAsync(code).Result;

            hasGenerated = discountCode is null;
        }

        return code!;
    }
}
