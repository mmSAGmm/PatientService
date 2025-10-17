using DbDataAccess.Abstractions;
using Patient.Domain.Abstractions;
using Patient.DomainModels.QueryParse;

namespace Patient.Domain.Implementation
{
    public class PatientBossService : IPatientBossService
    {
        private readonly IPatientRepository _patientRepository;

        private readonly IQueryParser _queryParser;

        public PatientBossService(IPatientRepository patientRepository, IQueryParser queryParser)
        {
            _patientRepository = patientRepository;
            _queryParser = queryParser;
        }

        public Task AddAsync(DomainModels.Patient patient, CancellationToken token)
        {
            return _patientRepository.AddAsync(patient, token);
        }

        public Task DeleteAsync(Guid Id, CancellationToken token)
        {
            return _patientRepository.DeleteAsync(Id, token);
        }

        public Task<DomainModels.Patient> GetAsync(Guid Id, CancellationToken token)
        {
            return _patientRepository.GetAsync(Id, token);
        }

        public Task UpdateAsync(DomainModels.Patient patient, CancellationToken token)
        {
            return _patientRepository.UpdateAsync(patient, token);
        }

        public Task<IEnumerable<DomainModels.Patient>> SearchAsync(string[] pattern, CancellationToken token)
        {
            var results = new List<ParseResult>();
            foreach (var ptn in pattern)
            {
                var pRs = _queryParser.Parse(ptn);
                if (pRs?.Status == Status.Success)
                {
                    results.Add(pRs);
                }
            }

            return _patientRepository.SearchAsync(results, token);
        }
    }
}
