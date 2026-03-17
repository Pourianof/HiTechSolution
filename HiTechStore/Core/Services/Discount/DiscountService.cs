using System.Security.Claims;

using AutoMapper;

using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Helpers;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.DiscountCode;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.Extensions.Primitives;

namespace HiTechStore.Core.Services.Discount;

public class DiscountService(
    IUnitOfWork unitOfWork,
    IDiscountCodeGenerator codeGenerator,
    IDiscountEntityResolver discountEntityResolver,
    IMapper mapper)
    : IDiscountService
{
    public async Task<bool> DeleteDiscountCode(int id)
    {
        var discountCode = await unitOfWork.DiscountCodeRepository.GetModelByIdAsync(id);

        if (discountCode is null)
        {
            return false;
        }

        await unitOfWork.DiscountCodeRepository.Delete(discountCode);

        return true;
    }

    public string GenerateRanomCode(int length = 10)
    {
        var hasGenerated = false;
        string? code = default;

        while (!hasGenerated)
        {
            code = codeGenerator.GenerateCode(length);

            var discountCode = unitOfWork.DiscountCodeRepository.GetDiscountCodeByNameAsync(code).Result;

            hasGenerated = discountCode is null;
        }

        return code!;
    }

    public Task<PagedResultDto<DiscountCodeDto>> GetAllDiscountCodes(DiscountQuery? discountQuery)
    {
        var query = discountQuery ?? new();

        query.SortBy ??= new QueryFilterItem("sortBy")
            .AddOperatorValuePair(QueryOperator.Equal, new StringValues("id,endTime"));
        query.Limit ??= new QueryFilterItem("limit")
            .AddOperatorValuePair(QueryOperator.Equal, new StringValues("10"));

        return unitOfWork.DiscountCodeRepository.GetAllProjectedAsync(query);
    }

    public async Task<DiscountCode> RegisterDiscountCode(DiscountCode discountCode)
    {
        var dbDiscountCodes = await unitOfWork.DiscountCodeRepository.GetDiscountCodeByNameAsync(discountCode.Code!);

        if (dbDiscountCodes is not null)
        {
            var overlappingDiscount = dbDiscountCodes.FirstOrDefault(
                    dbdc => !(discountCode.StartTime > dbdc!.EndTime
                        || discountCode.EndTime < dbdc.StartTime)
                );
            if (overlappingDiscount is not null)
            {
                // overlap state
                throw new ModelException("Overlapping date range", $"There is another discount with code \"{discountCode.Code}\" which start at {overlappingDiscount.StartTime} and ends at \"{overlappingDiscount.EndTime}\"", nameof(DiscountCode.StartTime));
            }
        }

        // lack of knowledge of buisiness rules to check
        // maybe completed later. need for experties

        await unitOfWork.DiscountCodeRepository.AddAsync(discountCode);

        if (await unitOfWork.Complete() > 0)
        {
            return discountCode;
        }

        throw new Exceptions.ApplicationException("Failed to save", "Something went wrong to save database");
    }

    async public Task<DiscountCodeDto?> UpdateDiscountCode(int id, DiscountCodeUpdateDto discountCodeUpdateDto, ClaimsPrincipal claims)
    {
        var discountCode = await unitOfWork.DiscountCodeRepository.GetModelByIdAsync(id);

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

        return new DiscountCodeDto()
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
        var now = DateTime.Now;
        var discounts = await unitOfWork.DiscountCodeRepository.GetDiscountCodeByNameAsync(discountCode);
        var availableDiscount = discounts.FirstOrDefault(
            (d) => d!.EndTime > now && d.StartTime < now
        );


        if (availableDiscount is null || availableDiscount.IsDeactivated)
        {
            throw new NotFoundException("Discount not found", $"Discount named \"{discountCode}\" not exist");
        }

        var userCart = await unitOfWork.CartRepository.GetUserActiveCartAsync(userId);

        foreach (var rule in availableDiscount.Rules!)
        {
            var isRuleAppliable = false;
            List<ProductVariation> productsWhichPassedCondition = new();
            foreach (var condGroup in rule.Conditions)
            {
                var isConditionGroupEstablished = true;
                foreach (var condition in condGroup.Conditions!)
                {
                    // check condition
                    var criteria = condition.EntityProperty;
                    string conditionValue = condition.Value!; // discount creator specify that

                    // how extract criteriaValue?
                    // in discountCode: Cart
                    // in normal discount: just product table

                    // get the criteria value
                    var entityPath = (await unitOfWork.DiscountEntityRepository.GetPropertyById(criteria!.DiscountEntityPropertyId))?.Path;
                    if (entityPath is null)
                    {
                        throw new NotFoundException("The specified discount code cannot get handle");
                    }

                    // compare to condition value based on operator
                    var criteriaValue = await discountEntityResolver.GetDiscountEntityInterpreter(entityPath!)
                                                    .Interpret(condition.Operation, conditionValue, new DiscountEntityResolverContext()
                                                    {
                                                        Cart = userCart,
                                                        MatchedProducts = productsWhichPassedCondition,
                                                        UnitOfWork = unitOfWork,
                                                        User = new User() { Id = userId }
                                                    });

                    // if comparation return false then the condition group
                    // will short circuit and fail
                    if (!criteriaValue.IsConditionPassed)
                    {
                        isConditionGroupEstablished = false;
                        // condition -> false -> condGroup -> false ->  circuit break
                        break;
                    }

                    if (criteriaValue.IsProductBase)
                    {
                        productsWhichPassedCondition.AddRange(criteriaValue.ConditionMatchedProducts!);
                    }
                }

                if (isConditionGroupEstablished)
                {
                    // at least one condGroup -> true -> circuit break -> rules  established
                    isRuleAppliable = true;
                    break;
                }
                else
                {
                    continue;
                }
            }

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
                }
                ;
            }
        }

        return new DiscountResultDto
        {
            IsDiscountAppliable = false,
        }
        ;
    }
}