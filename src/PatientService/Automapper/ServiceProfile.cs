using AutoMapper;
using PatientService.RequestModels;
using PatientService.ResponseModels;

namespace PatientService.Automapper
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile() 
        {
            CreateMap<Patient.DomainModels.Patient, GetPatientResponseModel>();
            CreateMap<CreatePatientRequestModel, Patient.DomainModels.Patient>()
                .ForMember(x => x.Id, x => x.MapFrom(s => Guid.NewGuid()));
            CreateMap<UpdatePatientRequestModel, Patient.DomainModels.Patient>();
        }
    }
}
