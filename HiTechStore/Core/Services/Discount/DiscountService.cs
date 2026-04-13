using System.Security.Claims;

using AutoMapper;

using HiTechStore.Core.Auth;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Helpers;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Discount;
using HiTechStore.Data.Mapping;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.Extensions.Primitives;

namespace HiTechStore.Core.Services.Discount;

public class DiscountService(
    IUnitOfWork unitOfWork,
    IDiscountCodeGenerator codeGenerator,
    IServiceProvider serviceProvider,
    IDiscountConditionScriptParser scriptParser,
    IMapper mapper,
    ICurrentUserProvider userProvider
    )
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

            hasGenerated = discountCode is not null;
        }

        return code!;
    }

    public Task<PagedResultDto<DiscountDto>> GetDiscounts(DiscountQuery? discountQuery)
    {
        var query = discountQuery ?? new();

        query.SortBy ??= new QueryFilterItem("sortBy")
            .AddOperatorValuePair(QueryOperator.Equal, new StringValues("id,endTime"));
        query.Limit ??= new QueryFilterItem("limit")
            .AddOperatorValuePair(QueryOperator.Equal, new StringValues("10"));

        query.DiscountType ??= DiscountType.All;

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

        var validateScript = (string? script, int index, string name) =>
        {
            if (script is not null)
            {
                var conditionTree = scriptParser.Parse(script);
                if (conditionTree is null)
                {
                    throw new ModelException(
                     "Invalid condition script", $"Rule's {name} condition script could not interpret as a expression which evaluate a boolean", $"{nameof(DiscountCreationDto.Rules)}[{index}].{name}");
                }

                return conditionTree;
            }

            return default;
        };

        for (int index = 0; index < discount.Rules!.Count(); index++)
        {
            var rule = discount.Rules!.ElementAt(index);

            var productScript = rule.ProductScript;
            ConditionComponent? productConditionTree = validateScript(productScript, index, nameof(DiscountRuleCreationDto.ProductScript));

            var userScript = rule.UserScript;
            ConditionComponent? userConditionTree = validateScript(userScript, index, nameof(DiscountRuleCreationDto.UserScript));

            if (productConditionTree is null && userConditionTree is null)
            {
                throw new ModelException(
                    "No condition script",
                    "Must specify either a product or user valid script",
                    $"{nameof(DiscountCreationDto.Rules)}[{index}]"
                );
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

        discount.IsDiscountCode = true;
        discount.CreatorId = userProvider.UserId!;

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

        discount.CreatorId = userProvider.UserId!;
        discount.IsDiscountCode = false;

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

        var currentUser = await unitOfWork.UserRepository.GetUserByIdAsync(userId);

        if (currentUser is null)
        {
            // todo: throw unauthorized result

            return new() { IsDiscountAppliable = false };
        }

        foreach (var rule in availableDiscount.Rules!)
        {

            var ruleUserScript = rule.UserRawConditionScript;
            var userConditionTree = ruleUserScript is null ? default : scriptParser.Parse(ruleUserScript);

            if (userConditionTree is null)
            {
                continue;
            }

            var userToExprMapper = serviceProvider.GetRequiredService<IConditionComponentTreeToLambdaExpression>();
            var userEvaluator = userToExprMapper.Map<User>(userConditionTree);
            var isUserAuthorized = userEvaluator.Compile().Invoke(currentUser!);

            if (!isUserAuthorized)
            {
                // if user condition not passed, then this discount not associate to user
                continue;
            }

            // for items in user's cart filtering we have two options:
            // use Product-Script(Rule.RawProductScript) as a filtering
            // or use User-Script itself but we must extract item filtering
            // and then applying it seperately and in isolated context.

            // List<ProductVariation> productsWhichPassedCondition = new();

            // calculate the discount which can assign to user cart
            var discountAction = mapper.Map<DiscountActionDto>(rule.DiscountAction);
            return new DiscountResultDto
            {
                DiscountCode = discountCode,
                IsDiscountAppliable = true,
                // AppliedTo = productsWhichPassedCondition.Any() ? DiscountTarget.Products.ToString() : DiscountTarget.Cart.ToString(),
                // DiscountedProducts = mapper.Map<IEnumerable<ProductVariationDto>>(productsWhichPassedCondition),
                Discount = discountAction
            };
        }

        return new DiscountResultDto
        {
            IsDiscountAppliable = false,
        };
    }

    public async Task<Models.Discount?> GetActiveDiscountCodeOf(string discountCode)
    {
        var now = DateTime.UtcNow;

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

