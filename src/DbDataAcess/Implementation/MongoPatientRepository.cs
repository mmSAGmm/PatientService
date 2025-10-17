using DbDataAccess.Abstractions;
using Db.DataAccess.Options;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Patient.DomainModels.QueryParse;
using System.Text.Json;
using Db.DataAccess.EF.Models;

namespace DbDataAccess.Implementation
{
	public class MongoPatientRepository : IPatientRepository
	{
		private readonly IMongoCollection<MongoPatientEntity> _collection;

        public MongoPatientRepository(IMongoClient mongoClient, IOptions<MongoOption> options)
		{
            var opt = options.Value;
            var database = mongoClient.GetDatabase(opt.DatabaseName);
            _collection = database.GetCollection<MongoPatientEntity>(opt.CollectionName);
		}

		public async Task AddAsync(Patient.DomainModels.Patient model, CancellationToken token)
		{
			var entity = new MongoPatientEntity
			{
				Id = model.Id.ToString(),
				Json = JsonSerializer.Serialize(model),
				BirthDate = model.BirthDate
			};
			await _collection.InsertOneAsync(entity, cancellationToken: token);
		}

		public async Task DeleteAsync(Guid Id, CancellationToken token)
		{
			await _collection.DeleteOneAsync(x => x.Id == Id.ToString(), token);
		}

		public async Task<Patient.DomainModels.Patient?> GetAsync(Guid Id, CancellationToken token)
		{
			var entity = await _collection.Find(x => x.Id == Id.ToString()).FirstOrDefaultAsync(token);
			if (entity == null) return null;
			return JsonSerializer.Deserialize<Patient.DomainModels.Patient>(entity.Json);
		}

		public async Task<IEnumerable<Patient.DomainModels.Patient>> SearchAsync(IEnumerable<ParseResult> parseResults, CancellationToken token)
		{
			var filterBuilder = Builders<MongoPatientEntity>.Filter;
			FilterDefinition<MongoPatientEntity>? filter = null;

			foreach (var pR in parseResults)
			{
				if (pR.Prefix == Prefix.Approximately && pR.Date.HasValue)
				{
					var dtUnix = new DateTimeOffset(new DateTime(
						pR.Date.Value.Year,
						pR.Date.Value.Month,
						pR.Date.Value.Day,
						pR.Time.HasValue ? pR.Time.Value.Hour : 0,
						pR.Time.HasValue ? pR.Time.Value.Minute : 0,
						pR.Time.HasValue ? pR.Time.Value.Second : 0)).ToUnixTimeSeconds();

					var lower = DateTimeOffset.FromUnixTimeSeconds((long)(dtUnix * 0.9)).UtcDateTime;
					var upper = DateTimeOffset.FromUnixTimeSeconds((long)(dtUnix * 1.1)).UtcDateTime;
					var part = filterBuilder.Gt(x => x.BirthDate, lower) & filterBuilder.Lt(x => x.BirthDate, upper);
					filter = filter == null ? part : filter & part;
					continue;
				}

				if (pR.Date.HasValue && pR.Time.HasValue)
				{
					var dt = new DateTime(pR.Date.Value.Year, pR.Date.Value.Month, pR.Date.Value.Day, pR.Time.Value.Hour, pR.Time.Value.Minute, pR.Time.Value.Second);
					FilterDefinition<MongoPatientEntity> part = pR.Prefix switch
					{
						Prefix.Equal => filterBuilder.Eq(x => x.BirthDate, dt),
						Prefix.LessThan => filterBuilder.Lt(x => x.BirthDate, dt),
						Prefix.GraterThan => filterBuilder.Gt(x => x.BirthDate, dt),
						Prefix.GreaterOrEqual => filterBuilder.Gte(x => x.BirthDate, dt),
						Prefix.LessOrEqual => filterBuilder.Lte(x => x.BirthDate, dt),
						_ => throw new InvalidOperationException($"{pR.Prefix} is unknown")
					};
					filter = filter == null ? part : filter & part;
				}
				else if (pR.Date.HasValue && !pR.Time.HasValue)
				{
					var dateOnly = new DateTime(pR.Date.Value.Year, pR.Date.Value.Month, pR.Date.Value.Day);
					var next = dateOnly.AddDays(1);
					var part = filterBuilder.Gte(x => x.BirthDate, dateOnly) & filterBuilder.Lt(x => x.BirthDate, next);
					filter = filter == null ? part : filter & part;
				}
			}

			if (filter == null)
			{
				throw new InvalidOperationException("No conditions for search");
			}

			var cursor = await _collection.FindAsync(filter, cancellationToken: token);
			var list = await cursor.ToListAsync(token);
			return list.Select(e => JsonSerializer.Deserialize<Patient.DomainModels.Patient>(e.Json)!).ToList();
		}

		public async Task UpdateAsync(Patient.DomainModels.Patient model, CancellationToken token)
		{
			var update = Builders<MongoPatientEntity>.Update
				.Set(x => x.Json, JsonSerializer.Serialize(model))
				.Set(x => x.BirthDate, model.BirthDate);
			await _collection.UpdateOneAsync(x => x.Id == model.Id.ToString(), update, new UpdateOptions { IsUpsert = false }, token);
		}

	
	}
}


