using Patient.DomainModels.QueryParse;

namespace DbDataAccess.Abstractions
{
    public interface IPatientRepository
    {
        public Task<Patient.DomainModels.Patient> GetAsync(Guid Id, CancellationToken token);

        public Task<IEnumerable<Patient.DomainModels.Patient>> SearchAsync(IEnumerable<ParseResult> parseResults, CancellationToken token);

        public Task AddAsync(Patient.DomainModels.Patient model, CancellationToken token);
        
        public Task UpdateAsync(Patient.DomainModels.Patient model, CancellationToken token);

        public Task DeleteAsync(Guid Id, CancellationToken token);
    }
}
