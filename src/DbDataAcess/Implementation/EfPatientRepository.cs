using Db.DataAccess.EF;
using DbDataAccess.Abstractions;
using Patient.DomainModels.QueryParse;
using System.Text.Json;

namespace Db.DataAccess.Implementation
{
    public class EfPatientRepository(DbContex dbContext) : IPatientRepository
    {

        public async Task Add(Patient.DomainModels.Patient model, CancellationToken token)
        {
            dbContext.Patients.Add(new EF.Models.Patient
            {
                Id = model.Id.ToString(),
                Json = JsonSerializer.Serialize(model),
                BirthDate = model.BirthDate,
            });
            await dbContext.SaveChangesAsync(token);
        }

        public async Task Delete(Guid Id, CancellationToken token)
        {
            var user = new EF.Models.Patient { Id = Id.ToString() };
            dbContext.Patients.Attach(user);
            dbContext.Patients.Remove(user);
            await dbContext.SaveChangesAsync(token);
        }

        public async Task<Patient.DomainModels.Patient> Get(Guid Id, CancellationToken token)
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

        public Task<IEnumerable<Patient.DomainModels.Patient>> Search(IEnumerable<ParseResult> parseResults, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task Update(Patient.DomainModels.Patient model, CancellationToken token)
        {
            var entity = await Get(model.Id, token);
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
    }
}
