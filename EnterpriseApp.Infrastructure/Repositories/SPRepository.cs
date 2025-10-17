using EnterpriseApp.Application.Interfaces;
using EnterpriseApp.Domain;
using EnterpriseApp.Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EnterpriseApp.Infrastructure.Repositories
{
    public class SPRepository : ISPRepository
    {
        private readonly AppDbContext _db;

        public SPRepository(AppDbContext db) => _db = db;

        public IQueryable<TResult> GetStoredProcedureResulRawt<TResult>
        (string storeProcedureName, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(storeProcedureName))
                throw new NullOrEmptySPException();

            var procedureTuple = GetProcedureString(storeProcedureName, parameters);

            if (parameters.Length == 0)
            {
                return _db.Database.SqlQueryRaw<TResult>(storeProcedureName);
            }

            var d = _db.Database.SqlQueryRaw<TResult>(procedureTuple.Item1, procedureTuple.Item2.ToArray());

            return d;
        }

        private (string, List<SqlParameter>) GetProcedureString(string SPName, params object[] parameters)
        {
            StringBuilder SPExecution = new StringBuilder();
            List<SqlParameter> sqlParameters = new();

            SPExecution.Append($"EXEC {SPName} ");

            int paramIndex = 1;
            foreach (var param in parameters)
            {
                string paramIdentifier = $"@param{paramIndex}";

                SPExecution.Append($"{paramIdentifier}, ");
                sqlParameters.Add(new SqlParameter(paramIdentifier, param ?? DBNull.Value));
                paramIndex++;
            }

            if (parameters.Length > 0)
            {
                SPExecution.Remove(SPExecution.Length - 2, 2);
            }

            return (SPExecution.ToString(), sqlParameters);
        }
    }
}
