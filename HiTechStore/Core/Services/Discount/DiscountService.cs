using System.Security.Claims;

using AutoMapper;

using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Helpers;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Discount;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.Extensions.Primitives;

namespace HiTechStore.Core.Services.Discount;

public class DiscountService(
    IUnitOfWork unitOfWork,
    IDiscountCodeGenerator codeGenerator,
    IDiscountConditionScriptParser scriptParser,
    IMapper mapper)
    : IDiscountService
{
    public async Task<bool> DeleteDiscountCode(int id)
    {
        var discountCode = await unitOfWork.DiscountRepository.GetModelByIdAsync(id);

        if (discountCode is null)
        {
            return false;
        }

        await unitOfWork.DiscountRepository.Delete(discountCode);

        return true;
    }

    public string GenerateRanomCode(int length = 10)
    {
        var hasGenerated = false;
        string? code = default;

        while (!hasGenerated)
        {
            code = codeGenerator.GenerateCode(length);

            var discountCode = unitOfWork.DiscountRepository.GetDiscountCodeByNameAsync(code).Result;

            hasGenerated = discountCode is null;
        }

        return code!;
    }

    public Task<PagedResultDto<DiscountDto>> GetAllDiscountCodes(DiscountQuery? discountQuery)
    {
        var query = discountQuery ?? new();

        query.SortBy ??= new QueryFilterItem("sortBy")
            .AddOperatorValuePair(QueryOperator.Equal, new StringValues("id,endTime"));
        query.Limit ??= new QueryFilterItem("limit")
            .AddOperatorValuePair(QueryOperator.Equal, new StringValues("10"));

        return unitOfWork.DiscountRepository.GetAllProjectedAsync(query);
    }

    private async Task ValidateDiscount(DiscountCreationDto discount)
    {
        if (discount.StartTime > discount.EndTime)
        {
            throw new ModelException("Invalid state", $"{nameof(Models.Discount.StartTime)} cannot be greater than {nameof(Models.Discount.EndTime)}", nameof(DiscountCodeCreationDto.StartTime));
        }

        if (!discount.Rules!.Any())
        {
            throw new ModelException("Invalid state", "At least one rule must define for discount", nameof(DiscountCodeCreationDto.Rules));
        }

        for (int index = 0; index < discount.Rules!.Count(); index++)
        {
            var rule = discount.Rules!.ElementAt(index);
            var conditionTree = scriptParser.Parse(rule.Script!);

            if (conditionTree is null)
            {
                throw new ModelException("Invalid condition script", "Rule's condition script could not interpret as a expression which evaluate a boolean", $"{nameof(Models.Discount.Rules)}[{index}]");
            }
        }
    }


    public async Task<DiscountDto> RegisterDiscountCode(DiscountCodeCreationDto discountCodeCreationDto)
    {

        await ValidateDiscount(discountCodeCreationDto);
        var discount = mapper.Map<Models.Discount>(discountCodeCreationDto);

        var dbDiscountCodes = await unitOfWork.DiscountRepository.GetDiscountCodeByNameAsync(discount.Code!);

        if (dbDiscountCodes is not null)
        {
            var overlappingDiscount = dbDiscountCodes.FirstOrDefault(
                    dbdc => !(discount.StartTime > dbdc!.EndTime
                        || discount.EndTime < dbdc.StartTime)
                );
            if (overlappingDiscount is not null)
            {
                // overlap state
                throw new ModelException("Overlapping date range", $"There is another discount with code \"{discount.Code}\" which start at {overlappingDiscount.StartTime} and ends at \"{overlappingDiscount.EndTime}\"", nameof(Models.Discount.StartTime));
            }
        }

        // lack of knowledge of buisiness rules to check
        // maybe completed later. need for experties

        await unitOfWork.DiscountRepository.AddAsync(discount);

        if (await unitOfWork.Complete() > 0)
        {
            return mapper.Map<DiscountDto>(discount);
        }

        throw new Exceptions.ApplicationException("Failed to save", "Something went wrong to save database");
    }

    public async Task<DiscountDto> RegisterDiscount(DiscountCreationDto discountCreationDto)
    {
        await ValidateDiscount(discountCreationDto);

        // IMapper also convert script string to ConditionComponent by parser
        var discount = mapper.Map<Models.Discount>(discountCreationDto);

        await unitOfWork.DiscountRepository.AddAsync(discount);


        if (await unitOfWork.Complete() > 0)
        {
            return mapper.Map<DiscountDto>(discount);
        }

        throw new Exceptions.ApplicationException("Failed to save", "Something went wrong to save database");
    }

    async public Task<DiscountDto?> UpdateDiscountCode(int id, DiscountCodeUpdateDto discountCodeUpdateDto, ClaimsPrincipal claims)
    {
        var discountCode = await unitOfWork.DiscountRepository.GetModelByIdAsync(id);

        if (discountCode is null)
        {
            return null;
        }

        if (discountCodeUpdateDto.Description is not null)
        {
            discountCode.Description = discountCodeUpdateDto.Description;
        }

        // Not totally clean arch. I did not take it so hard. 
        // it upto infra to handle auth stuff
        if (discountCodeUpdateDto.IsDeactivated is not null)
        {
            var roles = claims.FindFirst(ClaimTypes.Role)?.Value;

            if (roles is null || !IdentityRoles.PrivilagedRoles.Any(r => roles.Contains(r)))
            {
                throw new NotAllowedException(detail: "You have not required access");
            }

            discountCode.IsDeactivated = discountCodeUpdateDto.IsDeactivated.Value;
        }

        await unitOfWork.Complete();

        return new DiscountDto()
        {
            Code = discountCode.Code,
            CreatedAt = discountCode.CreatedAt,
            CreatorId = discountCode.CreatorId,
            Description = discountCode.Description,
            EndTime = discountCode.EndTime!.Value,
            StartTime = discountCode.StartTime!.Value,
            IsDeactivated = discountCode.IsDeactivated
        };
    }


    async public Task<DiscountResultDto> CheckDiscountCodeUsability(string discountCode, string userId)
    {
        var availableDiscount = await GetActiveDiscountCodeOf(discountCode);

        if (availableDiscount is null || availableDiscount.IsDeactivated)
        {
            throw new NotFoundException("Discount not found", $"Discount named \"{discountCode}\" not exist");
        }

        var userCart = await unitOfWork.CartRepository.GetUserActiveCartAsync(userId);

        foreach (var rule in availableDiscount.Rules!)
        {
            var isRuleAppliable = false;
            List<ProductVariation> productsWhichPassedCondition = new();

            if (isRuleAppliable)
            {
                // calculate the discount which can assign to user cart
                var discountAction = mapper.Map<DiscountActionDto>(rule.DiscountAction);
                return new DiscountResultDto
                {
                    DiscountCode = discountCode,
                    IsDiscountAppliable = true,
                    AppliedTo = productsWhichPassedCondition.Any() ? DiscountTarget.Products.ToString() : DiscountTarget.Cart.ToString(),
                    DiscountedProducts = mapper.Map<IEnumerable<ProductVariationDto>>(productsWhichPassedCondition),
                    Discount = discountAction
                };
            }
        }

        return new DiscountResultDto
        {
            IsDiscountAppliable = false,
        };
    }

    public async Task<Models.Discount?> GetActiveDiscountCodeOf(string discountCode)
    {
        var now = DateTime.Now;

        var discounts = await unitOfWork.DiscountRepository.GetDiscountCodeByNameAsync(discountCode);
        var availableDiscount = discounts.FirstOrDefault(
            (d) => d!.EndTime > now && d.StartTime < now
        );

        return availableDiscount;
    }

    public async Task<ConditionParseResult> GetConditionScriptProducts(string conditionScript)
    {
        var conditionTree = scriptParser.Parse(conditionScript);

        if (conditionTree is null)
        {
            return new ConditionParseResult()
            {
                Message = "Could not interpret script to well-defined internal models"
            };
        }

        var discountedProducts = await unitOfWork.Products.GetDiscountedProducts(conditionTree);

        return new ConditionParseResult()
        {
            ResultedProducts = discountedProducts,
            Succeed = true
        }
        ;
    }
}

