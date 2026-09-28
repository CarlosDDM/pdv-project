using Pdv.Domain.Entities;

namespace Pdv.Domain.Interfaces;

public interface ISeelingPriceRepository
{
    Task<SeelingPrice> AddAsync(SeelingPrice seelingPrice, CancellationToken ct = default);
}
