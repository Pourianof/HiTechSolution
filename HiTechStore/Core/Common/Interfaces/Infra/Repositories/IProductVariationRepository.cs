using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IProductVariationRepository : IRepositoryWithIntegerId<ProductVariation, ProductVariationDto>
{ }