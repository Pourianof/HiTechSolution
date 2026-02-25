using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Helpers;
using HiTechStore.Data.DTOs.DiscountCode;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.Extensions.Primitives;

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

    public Task<IEnumerable<DiscountCodeDto>> GetAllDiscountCodes(DiscountQuery? discountQuery)
    {
        var query = discountQuery ?? new();

        query.SortBy ??= new QueryFilterItem("sortBy")
            .AddOperatorValuePair(QueryOperator.Equal, new StringValues("id,endTime"));

        return _unitOfWork.DiscountCodeRepository.GetAllProjectedAsync(query);
    }

    public async Task<DiscountCode> RegisterDiscountCode(DiscountCode discountCode)
    {
        var dbDiscountCode = await _unitOfWork.DiscountCodeRepository.GetDiscountCodeByNameAsync(discountCode.Code!);

        if (dbDiscountCode is not null)
        {
            if (!(discountCode.StartTime > dbDiscountCode.EndTime
                || discountCode.EndTime < dbDiscountCode.StartTime))
            {
                // overlap state
                throw new ModelException("Overlapping date range", $"There is another discount with code \"{discountCode.Code}\" which start at {dbDiscountCode.StartTime} and ends at \"{dbDiscountCode.EndTime}\"", nameof(DiscountCode.StartTime));
            }
        }

        // lack of knowledge of buisiness rules to check
        // maybe completed later. need for experties

        await _unitOfWork.DiscountCodeRepository.AddAsync(discountCode);

        if (await _unitOfWork.Complete() > 0)
        {
            return discountCode;
        }

        throw new Exceptions.ApplicationException("Failed to save", "");
    }
}
