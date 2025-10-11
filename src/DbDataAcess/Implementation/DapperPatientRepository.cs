using Dapper;
using Db.DataAccess.Abstractions;
using DbDataAccess.Abstractions;
using Patient.DomainModels.QueryParse;
using System.Text.Json;

namespace Db.DataAccess.Implementation
{
    public class DapperPatientRepository : IPatientRepository
    {
        private const string dbFormat = "yyyy-MM-dd HH:mm:ss";

        private const string dateFormat = "yyyy-MM-dd";

        private readonly IConnectionProvider _connectionProvider;

        public DapperPatientRepository(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task Add(Patient.DomainModels.Patient model, CancellationToken token)
        {
            using var connection = _connectionProvider.GetConnection();

            var param = new Dictionary<string, object>()
            {
                ["id"] = model.Id,
                ["json"] = JsonSerializer.Serialize(model),
                ["birthDate"] = model.BirthDate.ToString(dbFormat)
            };

            var result
                = await connection.ExecuteAsync($"INSERT INTO tbPatients (id, json, birthDate) SELECT @id, @json, @birthDate", param);
        }

        public async Task Delete(Guid Id, CancellationToken token)
        {
            using var connection = _connectionProvider.GetConnection();

            var param = new Dictionary<string, object>()
            {
                ["id"] = Id,
            };

            await connection.ExecuteAsync($"DELETE FROM tbPatients WHERE Id = @id", param);
        }

        public async Task<Patient.DomainModels.Patient> Get(Guid Id, CancellationToken token)
        {
            using var connection = _connectionProvider.GetConnection();
           
            var param = new Dictionary<string, object>()
            {
                ["id"] = Id,
            };

            var dbResult = await connection.QueryFirstAsync($"SELECT id, json, birthDate FROM tbPatients WHERE Id = @id", param);
            if (await dbResult.ReadAsync(token))
            {
                var json = dbResult.json;

                var target = JsonSerializer.Deserialize<Patient.DomainModels.Patient>(json);
                return target;
            }

            return null;
        }

        public async Task<IEnumerable<Patient.DomainModels.Patient>> Search(IEnumerable<ParseResult> parseResults, CancellationToken token)
        {
            using var connection = _connectionProvider.GetConnection();

            List<string> queries = new List<string>();
            var param = new Dictionary<string, object>();

            for (var i = 0; i < parseResults.Count(); i++)
            {
                var pR = parseResults.ElementAt(i);
                var op = "";
                op = pR.Prefix switch
                {
                    Prefix.Equal => "=",
                    Prefix.LessThan => "<",
                    Prefix.GraterThan => ">",
                    Prefix.GreaterOrEqual => ">=",
                    Prefix.LessOrEqual => "<=",
                    Prefix.StartsAfter => ">",
                    Prefix.EndBefore => "<",
                    Prefix.Approximately => "ap",
                    _ => throw new InvalidOperationException($"{pR.Prefix} is unknown")
                };


                if (pR.Prefix == Prefix.Approximately)
                {
                    if (pR.Date.HasValue)
                    {
                        queries.Add($"birthDate > @pg{i} AND birthDate < @pl{i}");

                        var dtUnix = new DateTimeOffset(new DateTime(
                                pR.Date.Value.Year,
                                pR.Date.Value.Month,
                                pR.Date.Value.Day,
                                pR.Time.HasValue ? pR.Time.Value.Hour : 0,
                                pR.Time.HasValue ? pR.Time.Value.Minute : 0,
                                pR.Time.HasValue ? pR.Time.Value.Second : 0)).ToUnixTimeSeconds();

                        param.Add($"pg{i}", DateTimeOffset.FromUnixTimeSeconds((long)(dtUnix * 0.9)));
                        param.Add($"pl{i}", DateTimeOffset.FromUnixTimeSeconds((long)(dtUnix * 1.1)));
                    }

                    continue;
                }

                if (pR.Date.HasValue && pR.Time.HasValue)
                {
                    queries.Add($"birthDate {op} @p{i}");

                    param.Add($"p{i}",
                        new DateTime(
                            pR.Date.Value.Year,
                            pR.Date.Value.Month,
                            pR.Date.Value.Day,
                            pR.Time.Value.Hour,
                            pR.Time.Value.Minute,
                            pR.Time.Value.Second).ToString(dbFormat));
                }
                else if (pR.Date.HasValue && !pR.Time.HasValue)
                {
                    queries.Add($"DATE(birthDate) {op} @p{i}");
                    param.Add($"p{i}",
                      new DateTime(
                          pR.Date.Value.Year,
                          pR.Date.Value.Month,
                          pR.Date.Value.Day).ToString(dateFormat));
                }
            }

            if (!queries.Any())
            {
                throw new InvalidOperationException("No conditions for search");
            }

            var results = await connection.QueryAsync($"SELECT id, json, birthDate FROM tbPatients WHERE {string.Join(" AND ", queries)}");

            var rs = new List<Patient.DomainModels.Patient>();
            foreach (var dbResult in results)
            {
                var json = dbResult.json;
                //TOOD: to serializers intreface
                var target = JsonSerializer.Deserialize<Patient.DomainModels.Patient>(json);
                rs.Add(target);
            }

            return rs;
        }

        public async Task Update(Patient.DomainModels.Patient model, CancellationToken token)
        {
            using var connection = _connectionProvider.GetConnection();
          
            var param = new Dictionary<string, object>()
            {
                ["id"] = model.Id,
                ["json"] = JsonSerializer.Serialize(model),
                ["birthDate"] = model.BirthDate.ToString(dbFormat),
            };


            await connection.ExecuteAsync($"Update tbPatients SET json = @json, birthDate = @birthDate WHERE id = @id", param);
        }
    }
}
