using DbDataAccess.Abstractions;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Text.Json;

namespace DbDataAccess.Implementation
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string _connectionString;

        public PatientRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Add(Patient.DomainModels.Patient model)
        {
            using var connection = new SqliteConnection(_connectionString);

            using var cmd
                = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO tbUsers (id, json, birthDate) SELECT (@id, @json, @birthDate)";
            cmd.Parameters.Add(
                new SqliteParameter("id", model.Id));
            cmd.Parameters.Add(
                new SqliteParameter("json", JsonSerializer.Serialize(model)));
            cmd.Parameters.Add(
                new SqliteParameter("birthDate", model.BirthDate));

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(Guid Id)
        {
            using var connection = new SqliteConnection();
            using var cmd
                = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM tbUsers WHERE Id = @id";
            cmd.Parameters.Add(
                new SqliteParameter("id", Id));

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<Patient.DomainModels.Patient> Get(Guid Id)
        {
            using var connection = new SqliteConnection();
            using DbCommand cmd
                = connection.CreateCommand();
            cmd.CommandText = $"SELECT id, json, birthDate FROM tbUsers WHERE Id = @id";
            cmd.Parameters.Add(
                new SqliteParameter("id", Id));

            var dbResult = await cmd.ExecuteReaderAsync();
            var json = dbResult.GetString(1);

            var target = JsonSerializer.Deserialize<Patient.DomainModels.Patient>(json);

            return target;
        }

        public async Task Update(Patient.DomainModels.Patient model)
        {
            using var connection = new SqliteConnection(_connectionString);

            using var cmd
                = connection.CreateCommand();
            cmd.CommandText = $"Update tbUsers json = @json, birthDate = @birthDate WHERE id = @id";
            cmd.Parameters.Add(
                new SqliteParameter("id", model.Id));
            cmd.Parameters.Add(
                new SqliteParameter("json", JsonSerializer.Serialize(model)));
            cmd.Parameters.Add(
                new SqliteParameter("birthDate", model.BirthDate));

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
