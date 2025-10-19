namespace EnterpriseApp.Application.Dtos
{
    public record DgiiCompanyDto(
        string Identification,
        string CompanyName,
        string TradeName,
        string? Category,
        string PaymentScheme,
        string Status,
        string EconomicActivity,
        string GovernmentBranch
    );
}
