using EnterpriseApp.Application.Dtos;

namespace EnterpriseApp.Application.Interfaces
{
    public interface IDgiiService
    {
        Task<DgiiCompanyDto?> GetByRncAsync(string rnc, CancellationToken ct);
    }
}
