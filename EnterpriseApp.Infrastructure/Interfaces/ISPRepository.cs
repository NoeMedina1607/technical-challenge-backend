namespace EnterpriseApp.Application.Interfaces
{
    public interface ISPRepository
    {
        IQueryable<TResult> GetStoredProcedureResulRawt<TResult>(string storeProcedureName, params object[] parameters);
    }
}
