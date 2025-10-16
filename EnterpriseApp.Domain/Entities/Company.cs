namespace EnterpriseApp.Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Identification { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string ComercialName { get; set; } = default!;
        public string? Category { get; set; }
        public string PaymentScheme { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string EconomicActivity { get; set; } = default!;
        public string GovernmentBranch { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
