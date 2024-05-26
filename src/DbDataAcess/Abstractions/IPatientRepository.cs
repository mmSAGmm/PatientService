namespace DbDataAccess.Abstractions
{
    public interface IPatientRepository
    {
        public Task<Patient.DomainModels.Patient> Get(Guid Id);

        public Task Add(Patient.DomainModels.Patient model);
        
        public Task Update(Patient.DomainModels.Patient model);

        public Task Delete(Guid Id);
    }
}
