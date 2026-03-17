using System;

using HiTechStore.Models;

namespace HiTechStore.Core.Services.Discount;

public interface IDiscountEntityResolverContext
{
    IUnitOfWork? UnitOfWork { get; init; }
    Cart? Cart { get; init; }
    User? User { get; init; }
    IEnumerable<ProductVariation>? MatchedProducts { get; init; }
}


public class DiscountEntityResolverContext : IDiscountEntityResolverContext
{
    public IUnitOfWork? UnitOfWork { get; init; }
    public Cart? Cart { get; init; }
    public User? User { get; init; }
    public IEnumerable<ProductVariation>? MatchedProducts { get; init; }
}