using Db.DataAccess.Abstractions;
using Db.DataAccess.Implementation;
using Db.DataAccess.Options;
using DbDataAccess.Abstractions;
using DbDataAccess.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DbDataAccess
{
    public static class DbExtensions
    {
        public static IServiceCollection AddDbs(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MySqlOption>(configuration.GetSection("MySql"));
            services.Configure<MongoOption>(configuration.GetSection("Mongo"));
            services.AddSingleton<IConnectionProvider, MySqlConnectionProvider>();
            services.AddSingleton<IMongoClient>(sp => new MongoClient(sp.GetRequiredService<IOptions<MongoOption>>().Value.ConnectionString));
            //services.AddSingleton<IPatientRepository, MongoPatientRepository>();
            services.AddSingleton<IPatientRepository, AdoPatientRepository>();
            //services.AddTransient<IPatientRepository, EfPatientRepository>();
            //services.AddTransient<IPatientRepository, DapperPatientRepository>();
            return services;
        }
    }
}
