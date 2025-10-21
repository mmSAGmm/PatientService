using DbDataAccess.Abstractions;
using Db.DataAccess.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Patient.DomainModels.QueryParse;
using System.Text.Json;

namespace DbDataAccess.Implementation
{
    public class MongoPatientRepository : IPatientRepository
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoPatientRepository(IMongoClient mongoClient, IOptions<MongoOption> options)
        {
            var opt = options.Value;
            var database = mongoClient.GetDatabase(opt.DatabaseName);
            _collection = database.GetCollection<BsonDocument>(opt.CollectionName);
        }

        public async Task AddAsync(Patient.DomainModels.Patient model, CancellationToken token)
        {
            var doc = new BsonDocument
            {
                { "_id", model.Id.ToString() },
                { "json", JsonSerializer.Serialize(model) },
                { "birthDate", model.BirthDate }
            };
            await _collection.InsertOneAsync(doc, cancellationToken: token);
        }

        public async Task DeleteAsync(Guid Id, CancellationToken token)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", Id.ToString());
            await _collection.DeleteOneAsync(filter, token);
        }

        public async Task<Patient.DomainModels.Patient?> GetAsync(Guid Id, CancellationToken token)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", Id.ToString());
            var entity = await _collection.Find(filter).FirstOrDefaultAsync(token);
            if (entity == null) return null;
            var json = entity.GetValue("json", BsonNull.Value).AsString;
            return JsonSerializer.Deserialize<Patient.DomainModels.Patient>(json);
        }

        public async Task<IEnumerable<Patient.DomainModels.Patient>> SearchAsync(IEnumerable<ParseResult> parseResults, CancellationToken token)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument>? filter = null;

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
                    var part = filterBuilder.Gt("birthDate", lower) & filterBuilder.Lt("birthDate", upper);
                    filter = filter == null ? part : filter & part;
                    continue;
                }

                if (pR.Date.HasValue && pR.Time.HasValue)
                {
                    var dt = new DateTime(pR.Date.Value.Year, pR.Date.Value.Month, pR.Date.Value.Day, pR.Time.Value.Hour, pR.Time.Value.Minute, pR.Time.Value.Second);
                    FilterDefinition<BsonDocument> part = pR.Prefix switch
                    {
                        Prefix.Equal => filterBuilder.Eq("birthDate", dt),
                        Prefix.LessThan => filterBuilder.Lt("birthDate", dt),
                        Prefix.GraterThan => filterBuilder.Gt("birthDate", dt),
                        Prefix.GreaterOrEqual => filterBuilder.Gte("birthDate", dt),
                        Prefix.LessOrEqual => filterBuilder.Lte("birthDate", dt),
                        _ => throw new InvalidOperationException($"{pR.Prefix} is unknown")
                    };
                    filter = filter == null ? part : filter & part;
                }
                else if (pR.Date.HasValue && !pR.Time.HasValue)
                {
                    var dateOnly = new DateTime(pR.Date.Value.Year, pR.Date.Value.Month, pR.Date.Value.Day);
                    var next = dateOnly.AddDays(1);
                    var part = filterBuilder.Gte("birthDate", dateOnly) & filterBuilder.Lt("birthDate", next);
                    filter = filter == null ? part : filter & part;
                }
            }

            if (filter == null)
            {
                throw new InvalidOperationException("No conditions for search");
            }

            var cursor = await _collection.FindAsync(filter, cancellationToken: token);
            var list = await cursor.ToListAsync(token);
            return list.Select(e => JsonSerializer.Deserialize<Patient.DomainModels.Patient>(e["json"].AsString)!).ToList();
        }

        public async Task UpdateAsync(Patient.DomainModels.Patient model, CancellationToken token)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", model.Id.ToString());
            var update = Builders<BsonDocument>.Update
                .Set("json", JsonSerializer.Serialize(model))
                .Set("birthDate", model.BirthDate);
            await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false }, token);
        }


    }
}


