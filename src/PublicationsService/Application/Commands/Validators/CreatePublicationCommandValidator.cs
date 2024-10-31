using FluentValidation;
using PublicationsService.Aplication.Commands;

namespace PublicationsService.Application.Commands.Validators
{
    public class CreatePublicationCommandValidator : AbstractValidator<CreatePublicationCommand>
    {
        public CreatePublicationCommandValidator()
        {
            RuleFor(x => x.IdUser).GreaterThan(0).WithMessage("User Id must be greater than 0");
            RuleFor(x => x.IdRole).GreaterThan(0).WithMessage("Role Id must be greater than 0");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required").Length(5, 100).WithMessage("Title is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required").Length(10,500).WithMessage("Description must be between 10 and 500 characteres");
            RuleFor(x => x.ExpirationDate).GreaterThan(DateTime.UtcNow).WithMessage("Expiration date must be in future");
            RuleFor(x => x.Salary).GreaterThanOrEqualTo(0).WithMessage("Salary must be positive number");
            RuleFor(x => x.Location).NotEmpty().WithMessage("Location is required");
            RuleFor(x => x.Company).NotEmpty().WithMessage("Company is required");
        }
    }
}
