using Pdv.Domain.Common;
using Pdv.Domain.Entities;

namespace Pdv.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product> AddAsync(Product product, CancellationToken ct = default);
    Task<PagedResult<Product>> ListAllAsync(int pageNumber, int pageSize, CancellationToken ct = default);
    Task<Product?> FirstOrDefaultAsync(Guid id, CancellationToken ct = default);
    Task<Product?> FirstOrDefaultByBarcodeAsync(string barcode, CancellationToken ct = default);
    Task<bool> ExistsBarcodeAsync(string barcode, CancellationToken ct = default);
    Task<Product> UpdateAsync(Product product, CancellationToken ct = default);
    Task<bool> SoftDeleteAsync(Product product, CancellationToken ct = default);
}
