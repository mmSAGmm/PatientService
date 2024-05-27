using DbDataAccess.Abstractions;
using Patient.Domain.Abstractions;
using Patient.DomainModels.QueryParse;
using System.Text.RegularExpressions;

namespace Patient.Domain.Implementation
{
    public class PatientBossService : IPatientBossService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientBossService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
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
                var pRs = Parse(ptn);
                if (pRs.Status == Status.Success)
                {
                    results.Add(pRs);
                }
            }

            return _patientRepository.Search(results, token);
        }

        private Regex _regex = new Regex(@"(?<prefix>[\w]{2}){0,1}(?<date>\d{4}-\d{2}-\d{2}){1}(?<time>T[\d]{2}(:[\d]{2}){0,1}(:\d\d){0,1}){0,1}");
        
        private ParseResult Parse(string pattern)
        {
            string prefix = string.Empty;
            string date = string.Empty;
            string time = string.Empty;
            var match = _regex.Match(pattern);
            if (match.Success)
            {
                if (match.Groups["prefix"].Success)
                {
                    prefix = match.Groups["prefix"].Value;
                }

                if (match.Groups["date"].Success)
                {
                    date = match.Groups["date"].Value;
                }

                if (match.Groups["time"].Success)
                {
                    time = match.Groups["time"].Value.Trim('T');
                }

                return ParseResult.Result(prefix, date, time);
            }
            else
            {
                return ParseResult.Fail();
            }
        }
    }
}
