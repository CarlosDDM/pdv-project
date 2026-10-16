using Pdv.Domain.Entities;
using Pdv.Domain.Interfaces;

namespace Pdv.Infra.Data.Repositories;

public sealed class CompanyRepository : ICompanyRepository
{
    public Task<Company> AddAsync()
    {
        throw new NotImplementedException();
    }
}
