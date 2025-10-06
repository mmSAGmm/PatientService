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

        public Task Add(DomainModels.Patient patient, CancellationToken token)
        {
            return _patientRepository.Add(patient, token);
        }

        public Task Delete(Guid Id, CancellationToken token)
        {
            return _patientRepository.Delete(Id, token);
        }

        public Task<DomainModels.Patient> Get(Guid Id, CancellationToken token)
        {
            return _patientRepository.Get(Id, token);
        }

        public Task Update(DomainModels.Patient patient, CancellationToken token)
        {
            return _patientRepository.Update(patient, token);
        }

        public Task<IEnumerable<DomainModels.Patient>> Search(string[] pattern, CancellationToken token)
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

            return _patientRepository.Search(results, token);
        }
    }
}
