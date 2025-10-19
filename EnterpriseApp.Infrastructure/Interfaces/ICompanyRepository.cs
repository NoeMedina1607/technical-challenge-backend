using EnterpriseApp.Application.Dtos;
using EnterpriseApp.Infrastructure.Models;

namespace EnterpriseApp.Application.Interfaces
{
    public interface ICompanyRepository
    {
        Task<int> CreateCompanyAsync(CreateCompanyDto companyDto, CancellationToken cancellationToken);
        Task<int> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<CompanyModel> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<CompanyModel>> ListAsync(CancellationToken cancellationToken);
        Task<int> UpdateAsync(int id, UpdateCompanyDto companyDto, CancellationToken cancellationToken);
    }
}
