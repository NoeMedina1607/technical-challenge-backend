namespace EnterpriseApp.Application.Dtos
{
    public record CreateCompanyDto(
        string Identification,
        string Name,
        string TradeName,
        string? Category,
        string PaymentScheme,
        string Status,
        string EconomicActivity,
        string GovernmentBranch
    );
}
