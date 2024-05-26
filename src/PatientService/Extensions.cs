using FluentValidation;
using FluentValidation.AspNetCore;
using PatientService.Automapper;
using PatientService.RequestModels;
using PatientService.Validation;

public static class Extensions
{
    public static IServiceCollection AddService(this IServiceCollection services)
    {
        services.AddAutoMapper(x => x.AddProfile(typeof(ServiceProfile)))
            .AddFluentValidationAutoValidation()
            .AddScoped<IValidator<CreatePatientRequestModel>, CreatePatientModelValidation>()
            .AddScoped<IValidator<UpdatePatientRequestModel>, UpdatePatientModelValidation>();

        return services;
    }
}