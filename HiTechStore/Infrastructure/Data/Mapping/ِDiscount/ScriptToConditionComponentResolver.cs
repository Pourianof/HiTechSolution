
using AutoMapper;

using HiTechStore.Core.Helpers;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.Mapping.Discount;


public class ScriptToConditionComponentResolver(IDiscountConditionScriptParser scriptParser) : IValueConverter<string, ConditionComponent>
{
    public ConditionComponent Convert(string script, ResolutionContext context)
    {
        return scriptParser.Parse(script)!;
    }
}