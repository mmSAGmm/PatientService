using Db.DataAccess.Abstractions;
using DbDataAccess.Abstractions;
using Patient.DomainModels.QueryParse;
using System.Data;
using System.Data.Common;
using System.Text.Json;

namespace DbDataAccess.Implementation
{
    public class AdoPatientRepository : IPatientRepository
    {
        private const string dbFormat = "yyyy-MM-dd HH:mm:ss";

        private const string dateFormat = "yyyy-MM-dd";

        private readonly IConnectionProvider _connectionProvider;

        public AdoPatientRepository(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task Add(Patient.DomainModels.Patient model, CancellationToken token)
        {
            using var connection = _connectionProvider.GetConnection();
            await connection.OpenAsync(token);

            using var cmd
                = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO tbPatients (id, json, birthDate) SELECT @id, @json, @birthDate";

            AddParameter(cmd, "id", model.Id);
            AddParameter(cmd, "json", JsonSerializer.Serialize(model));
            AddParameter(cmd, "birthDate", model.BirthDate.ToString(dbFormat));

            await cmd.ExecuteScalarAsync(token);
        }

        public async Task Delete(Guid Id, CancellationToken token)
        {
            using var connection = _connectionProvider.GetConnection();
            await connection.OpenAsync(token);

            using var cmd
                = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM tbPatients WHERE Id = @id";

            AddParameter(cmd, "id", Id);

            await cmd.ExecuteNonQueryAsync(token);
        }

        public async Task<Patient.DomainModels.Patient> Get(Guid Id, CancellationToken token)
        {
            using var connection = _connectionProvider.GetConnection();
            await connection.OpenAsync(token);
            using DbCommand cmd
                = connection.CreateCommand();
            cmd.CommandText = $"SELECT id, json, birthDate FROM tbPatients WHERE Id = @id";

            AddParameter(cmd, "id", Id);

            using var dbResult = await cmd.ExecuteReaderAsync(token);
            if (await dbResult.ReadAsync(token))
            {
                var json = dbResult.GetString(1);

                var target = JsonSerializer.Deserialize<Patient.DomainModels.Patient>(json);
                return target;
            }

            return null;
        }

        public async Task<IEnumerable<Patient.DomainModels.Patient>> Search(IEnumerable<ParseResult> parseResults, CancellationToken token)
        {
            using var connection = _connectionProvider.GetConnection();
            await connection.OpenAsync(token);
            using DbCommand cmd
                = connection.CreateCommand();

            List<string> queries = new List<string>();

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

                        AddParameter(cmd, $"pg{i}", DateTimeOffset.FromUnixTimeSeconds((long)(dtUnix * 0.9))
                            .ToString(dbFormat));
                        AddParameter(cmd, $"pl{i}", DateTimeOffset.FromUnixTimeSeconds((long)(dtUnix * 1.1))
                            .ToString(dbFormat));
                    }

                    continue;
                }

                if (pR.Date.HasValue && pR.Time.HasValue)
                {
                    queries.Add($"birthDate {op} @p{i}");
                    AddParameter(cmd, $"p{i}",
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
                    AddParameter(cmd, $"p{i}",
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

            cmd.CommandText = $"SELECT id, json, birthDate FROM tbPatients WHERE {string.Join(" AND ", queries)}";

            using var dbResult = await cmd.ExecuteReaderAsync(token)
            var rs = new List<Patient.DomainModels.Patient>();
            while(await dbResult.ReadAsync(token))
            {
                var json = dbResult.GetString(1);
                //TOOD: to serializers intreface
                var target = JsonSerializer.Deserialize<Patient.DomainModels.Patient>(json);
                rs.Add(target);
            }

            return rs;
        }

        public async Task Update(Patient.DomainModels.Patient model, CancellationToken token)
        {
            using var connection = _connectionProvider.GetConnection();
            await connection.OpenAsync(token);
            using var cmd
                = connection.CreateCommand();
            cmd.CommandText = $"Update tbPatients SET json = @json, birthDate = @birthDate WHERE id = @id";

            AddParameter(cmd, "id", model.Id);
            AddParameter(cmd, "json", JsonSerializer.Serialize(model));
            AddParameter(cmd, "birthDate", model.BirthDate.ToString(dbFormat));

            await cmd.ExecuteNonQueryAsync(token);
        }

        private void AddParameter(IDbCommand cmd, string name, object value)
        {
            var idParameter = cmd.CreateParameter();
            idParameter.ParameterName = name;
            idParameter.Value = value.ToString();
            cmd.Parameters.Add(idParameter);
        }
    }
}
