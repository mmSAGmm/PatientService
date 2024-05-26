using FluentValidation;
using PatientService.RequestModels;

namespace PatientService.Validation
{
    public class UpdatePatientModelValidation : BasePatientModelValidation<UpdatePatientRequestModel>
    {
        public UpdatePatientModelValidation()
        {
            RuleFor(x => x.Id).NotEqual(Guid.Empty);
        }
    }
}
