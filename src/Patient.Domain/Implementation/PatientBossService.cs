using DbDataAccess.Abstractions;
using Patient.Domain.Abstractions;

namespace Patient.Domain.Implementation
{
    public class PatientBossService : IPatientBossService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientBossService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public Task Add(DomainModels.Patient patient)
        {
            return _patientRepository.Add(patient);
        }

        public Task Delete(Guid Id)
        {
            return _patientRepository.Delete(Id);
        }

        public Task<DomainModels.Patient> Get(Guid Id)
        {
            return _patientRepository.Get(Id);
        }

        public Task Update(DomainModels.Patient patient)
        {
            return _patientRepository.Update(patient);
        }

        public Task<IReadOnlyCollection<DomainModels.Patient>> Search(string pattern) => throw new NotImplementedException();
    }
}
