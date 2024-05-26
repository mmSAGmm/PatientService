using DbDataAccess.Abstractions;
using DbDataAccess.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace DbDataAccess
{
    public static class DbExtensions
    {
        public static IServiceCollection AddDbs(this IServiceCollection services)
        {
            services.AddSingleton<IPatientRepository, PatientRepository>();
            return services;
        }
    }
}
