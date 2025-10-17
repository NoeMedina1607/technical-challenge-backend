namespace EnterpriseApp.Application.Dtos
{
    public record UpdateCompanyDto(
        string Name,
        string TradeName,
        string? Category,
        string PaymentScheme,
        string Status,
        string EconomicActivity,
        string GovernmentBranch
    );
}
