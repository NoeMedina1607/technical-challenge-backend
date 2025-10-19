using EnterpriseApp.Application.Dtos;
using EnterpriseApp.Application.Interfaces;
using EnterpriseApp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace EnterpriseApp.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ISPRepository _spRepository;
        public CompanyRepository(ISPRepository spRepository)
        {
            _spRepository = spRepository;
        }

        public async Task<int> CreateCompanyAsync(CreateCompanyDto companyDto, CancellationToken cancellationToken)
        {
            try
            {
                var query = _spRepository.GetStoredProcedureResulRawt<int>(
                    "dbo.spCompany_Create",
                    companyDto.Identification,
                    companyDto.Name,
                    companyDto.TradeName,
                    companyDto.Category,
                    companyDto.PaymentScheme,
                    companyDto.Status,
                    companyDto.EconomicActivity,
                    companyDto.GovernmentBranch
                );

                var rows = await query.ToListAsync(cancellationToken);

                return rows.Single();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CompanyModel>> ListAsync(CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<CompanyModel> query =
                    _spRepository.GetStoredProcedureResulRawt<CompanyModel>("dbo.spCompany_List");

                return await query.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CompanyModel> GetByIdAsync(int id,  CancellationToken cancellationToken)
        {
            try
            {
                var query = await _spRepository.GetStoredProcedureResulRawt<CompanyModel>("spCompany_GetById", id)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return query.SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateAsync(int id, UpdateCompanyDto companyDto, CancellationToken cancellationToken)
        {
            try
            {
                var query = _spRepository.GetStoredProcedureResulRawt<int>(
                    "dbo.spCompany_Update",
                    id,
                    companyDto.Name,
                    companyDto.TradeName,
                    companyDto.Category,
                    companyDto.PaymentScheme,
                    companyDto.Status,
                    companyDto.EconomicActivity,
                    companyDto.GovernmentBranch
                );

                var rows = await query.ToListAsync(cancellationToken);

                return rows.Single();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var query = _spRepository.GetStoredProcedureResulRawt<int>("dbo.spCompany_Delete", id);

                var rows = await query.ToListAsync(cancellationToken);
                return rows.SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
