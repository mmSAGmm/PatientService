using Db.DataAccess.Abstractions;
using Db.DataAccess.Implementation;
using Db.DataAccess.Options;
using DbDataAccess.Abstractions;
using DbDataAccess.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbDataAccess
{
    public static class DbExtensions
    {
        public static IServiceCollection AddDbs(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MySqlOption>(configuration.GetSection("MySql"));
            services.AddSingleton<IConnectionProvider, MySqlConnectionProvider>();
            services.AddSingleton<IPatientRepository, AdoPatientRepository>();
            //services.AddTransient<IPatientRepository, EfPatientRepository>();
            return services;
        }
    }
}
