using Microsoft.Extensions.DependencyInjection;
using Patient.Domain.Abstractions;
using Patient.Domain.Implementation;

namespace Patient.Domain
{
    public static class DomainExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services) 
        {
            services.AddTransient<IPatientBossService, PatientBossService>();
            services.AddTransient<IQueryParser, QueryParser>();
            return services;
        }
    }
}
