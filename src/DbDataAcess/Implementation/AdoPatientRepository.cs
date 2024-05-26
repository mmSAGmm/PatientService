using Db.DataAccess.Abstractions;
using DbDataAccess.Abstractions;
using System.Data;
using System.Data.Common;
using System.Text.Json;

namespace DbDataAccess.Implementation
{
    public class AdoPatientRepository : IPatientRepository
    {
        private readonly IConnectionProvider _connectionProvider;

        public AdoPatientRepository(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task Add(Patient.DomainModels.Patient model)
        {
            using var connection = _connectionProvider.GetConnection();
            await connection.OpenAsync();

            using var cmd
                = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO tbPatients (id, json, birthDate) SELECT @id, @json, @birthDate";

            AddParameter(cmd, "id", model.Id);
            AddParameter(cmd, "json", JsonSerializer.Serialize(model));
            AddParameter(cmd, "birthDate", model.BirthDate.ToString("yyyy-MM-dd hh:mm:ss"));

            await cmd.ExecuteScalarAsync();
        }

        public async Task Delete(Guid Id)
        {
            using var connection = _connectionProvider.GetConnection();
            await connection.OpenAsync();

            using var cmd
                = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM tbPatients WHERE Id = @id";

            AddParameter(cmd, "id", Id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<Patient.DomainModels.Patient> Get(Guid Id)
        {
            using var connection = _connectionProvider.GetConnection();
            await connection.OpenAsync();
            using DbCommand cmd
                = connection.CreateCommand();
            cmd.CommandText = $"SELECT id, json, birthDate FROM tbPatients WHERE Id = @id";

            AddParameter(cmd, "id", Id);

            var dbResult = await cmd.ExecuteReaderAsync();
            if (await dbResult.ReadAsync())
            {
                var json = dbResult.GetString(1);

                var target = JsonSerializer.Deserialize<Patient.DomainModels.Patient>(json);
                return target;
            }

            return null;
        }

        public async Task Update(Patient.DomainModels.Patient model)
        {
            using var connection = _connectionProvider.GetConnection();
            await connection.OpenAsync();
            using var cmd
                = connection.CreateCommand();
            cmd.CommandText = $"Update tbPatients SET json = @json, birthDate = @birthDate WHERE id = @id";

            AddParameter(cmd, "id", model.Id);
            AddParameter(cmd, "json", JsonSerializer.Serialize(model));
            AddParameter(cmd, "birthDate", model.BirthDate.ToString("yyyy-MM-dd hh:mm:ss"));

            await cmd.ExecuteNonQueryAsync();
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
