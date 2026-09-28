using Pdv.Domain.Entities;
using Pdv.Domain.Interfaces;
using Pdv.Infra.Data.Context;

namespace Pdv.Infra.Data.Repositories;

public sealed class SeelingPriceRepository : ISeelingPriceRepository
{
    private readonly PdvDbContext _context;

    public SeelingPriceRepository(PdvDbContext context)
    {
        _context = context;
    }

    public async Task<SeelingPrice> AddAsync(SeelingPrice seelingPrice, CancellationToken ct = default)
    {
        _context.SeelingPrices.Add(seelingPrice);

        await _context.SaveChangesAsync(ct);

        return seelingPrice;
    }
}
