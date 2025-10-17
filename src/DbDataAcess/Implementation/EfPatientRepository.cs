using Db.DataAccess.EF;
using DbDataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Patient.DomainModels.QueryParse;
using System.Text.Json;

namespace Db.DataAccess.Implementation
{
    public class EfPatientRepository(DbContex dbContext) : IPatientRepository
    {
        private const string dbFormat = "yyyy-MM-dd HH:mm:ss";

        private const string dateFormat = "yyyy-MM-dd";

        public async Task AddAsync(Patient.DomainModels.Patient model, CancellationToken token)
        {
            dbContext.Patients.Add(new EF.Models.Patient
            {
                Id = model.Id.ToString(),
                Json = JsonSerializer.Serialize(model),
                BirthDate = model.BirthDate,
            });
            await dbContext.SaveChangesAsync(token);
        }

        public async Task DeleteAsync(Guid Id, CancellationToken token)
        {
            var user = new EF.Models.Patient { Id = Id.ToString() };
            dbContext.Patients.Attach(user);
            dbContext.Patients.Remove(user);
            await dbContext.SaveChangesAsync(token);
        }

        public async Task<Patient.DomainModels.Patient> GetAsync(Guid Id, CancellationToken token)
        {
            var result = await dbContext.Patients.FindAsync(Id.ToString(), token);

            if (result != null)
            {
                var json = result.Json;

                var target = JsonSerializer.Deserialize<Patient.DomainModels.Patient>(json);
                return target;
            }

            return null;
        }

        public async Task<IEnumerable<Patient.DomainModels.Patient>> SearchAsync(IEnumerable<ParseResult> parseResults, CancellationToken token)
        {
            List<string> queries = new List<string>();
            List<MySqlParameter> parameters = new List<MySqlParameter>();

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

                        AddParameter(parameters, $"pg{i}", DateTimeOffset.FromUnixTimeSeconds((long)(dtUnix * 0.9))
                            .ToString(dbFormat));
                        AddParameter(parameters, $"pl{i}", DateTimeOffset.FromUnixTimeSeconds((long)(dtUnix * 1.1))
                            .ToString(dbFormat));
                    }

                    continue;
                }

                if (pR.Date.HasValue && pR.Time.HasValue)
                {
                    queries.Add($"birthDate {op} @p{i}");
                    AddParameter(parameters, $"p{i}",
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
                    AddParameter(parameters, $"p{i}",
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

            var users = await dbContext.Patients
                        .FromSqlRaw($"SELECT id, json, birthDate FROM tbPatients WHERE {string.Join(" AND ", queries)}", parameters.ToArray())
                        .ToListAsync();

            var rs = new List<Patient.DomainModels.Patient>();

            foreach (var dbResult in users)
            {
                var json = dbResult.Json;
                //TOOD: to serializers intreface
                var target = JsonSerializer.Deserialize<Patient.DomainModels.Patient>(json);
                rs.Add(target);
            }

            return rs;
        }

        public async Task UpdateAsync(Patient.DomainModels.Patient model, CancellationToken token)
        {
            var entity = await GetAsync(model.Id, token);
            if (entity != null)
            {
                entity.Active = model.Active;
                entity.BirthDate = model.BirthDate;
                entity.Gender = model.Gender;
                entity.Use = model.Use;
                entity.Family = model.Family;
                entity.Given = model.Given;
                await dbContext.SaveChangesAsync();
            }
        }

        private void AddParameter(List<MySqlParameter> parameters, string name, object value)
        {
            var idParameter = new MySqlParameter();
            idParameter.ParameterName = name;
            idParameter.Value = value.ToString();
            parameters.Add(idParameter);
        }
    }
}
