
using AutoMapper;

using HiTechStore.Core.Helpers;
using HiTechStore.Models;

namespace HiTechStore.Data.Mapping.Discount;


public class ScriptToConditionComponentResolver(IDiscountConditionScriptParser scriptParser) : ITypeConverter<string, ConditionComponent>
{

    public ConditionComponent Convert(string script, ConditionComponent destination, ResolutionContext context)
    {
        return scriptParser.Parse(script)!;
    }
}