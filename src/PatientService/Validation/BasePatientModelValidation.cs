using FluentValidation;
using PatientService.RequestModels;

namespace PatientService.Validation
{
    public class BasePatientModelValidation<T> : AbstractValidator<T> where T: BasePatientRequestModel
    {
        public BasePatientModelValidation()
        {
            RuleFor(x=> x.Family)
                .NotEmpty()
                .WithMessage("Invalid family");
            RuleFor(x => x.BirthDate)
                .Must(x => x != default)
                .WithMessage("Invalid BirthDate");
        }
    }
}
