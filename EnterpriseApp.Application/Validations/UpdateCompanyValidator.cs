using EnterpriseApp.Application.Dtos;
using FluentValidation;

namespace EnterpriseApp.Application.Validations
{
    public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyDto>
    {
        public UpdateCompanyValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.TradeName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.PaymentScheme).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Status).NotEmpty().MaximumLength(50);
            RuleFor(x => x.EconomicActivity).NotEmpty().MaximumLength(200);
            RuleFor(x => x.GovernmentBranch).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Category).MaximumLength(100).When(x => x.Category is not null);
        }
    }
}
