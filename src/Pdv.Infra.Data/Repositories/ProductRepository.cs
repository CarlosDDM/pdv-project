using Microsoft.EntityFrameworkCore;
using Pdv.Domain.Common;
using Pdv.Domain.Entities;
using Pdv.Domain.Interfaces;
using Pdv.Infra.Data.Context;
using Pdv.Infra.Data.Helpers;

namespace Pdv.Infra.Data.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly PdvDbContext _context;

    public ProductRepository(PdvDbContext context)
    {
        _context = context;
    }

    public async Task<Product> AddAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Add(product);

        await _context.SaveChangesAsync(ct);

        return product;
    }

    public async Task<PagedResult<Product>> ListAllAsync(int pageNumber, int pageSize, CancellationToken ct = default)
    {

        var query = _context.Products
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id);

        return await PaginationHelper.CreatePaginationResponseAsync(query, pageNumber, pageSize, ct);

    }

    public async Task<bool> ExistsBarcodeAsync(string barcode, CancellationToken ct = default)
    {
        return await _context.Products.AnyAsync(x => x.Barcode == barcode, ct);
    }

    public async Task<Product?> FirstOrDefaultAsync(Guid id, CancellationToken ct = default)
    {
      return await _context.Products
            .AsNoTracking()
            .Include(x => x.SeelingPrices
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id)
                .Take(1))
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    }

    public async Task<Product?> FirstOrDefaultByBarcodeAsync(string barcode, CancellationToken ct = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(x => x.SeelingPrices
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id)
                .Take(1))
            .FirstOrDefaultAsync(x => x.Barcode == barcode, ct);
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(ct);

        return product;
    }

    public async Task<bool> SoftDeleteAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Update(product);

        var changes = await _context.SaveChangesAsync(ct);
        return changes > 0;
    }

}

