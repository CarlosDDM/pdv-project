using Pdv.Domain.Entities;

namespace Pdv.Domain.Interfaces;

public interface ICompanyRepository
{
    Task<Company> AddAsync();
}
