using Patient.DomainModels.QueryParse;

namespace DbDataAccess.Abstractions
{
    public interface IPatientRepository
    {
        public Task<Patient.DomainModels.Patient> Get(Guid Id, CancellationToken token);

        public Task<IEnumerable<Patient.DomainModels.Patient>> Search(IEnumerable<ParseResult> parseResults, CancellationToken token);

        public Task Add(Patient.DomainModels.Patient model, CancellationToken token);
        
        public Task Update(Patient.DomainModels.Patient model, CancellationToken token);

        public Task Delete(Guid Id, CancellationToken token);
    }
}
