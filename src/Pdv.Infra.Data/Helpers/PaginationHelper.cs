using Microsoft.EntityFrameworkCore;
using Pdv.Domain.Common;

namespace Pdv.Infra.Data.Helpers;

public static class PaginationHelper
{
    public static async Task<PagedResult<T>> CreatePaginationResponseAsync<T>
        (IQueryable<T> source, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, page, pageSize, count);
    }
}