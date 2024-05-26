using DbDataAccess;
using FluentValidation;
using FluentValidation.AspNetCore;
using Patient.Domain;
using PatientService.Automapper;
using PatientService.Middleware;
using PatientService.RequestModels;
using PatientService.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions();


builder.Services
    .AddControllers(c=> c.Filters.Add(new ValidationFilterAttribute()));
builder.Services
    .AddDomain()
    .AddDbs(builder.Configuration)
    .AddAutoMapper(x => x.AddProfile(typeof(ServiceProfile)))
    .AddFluentValidationAutoValidation()
    .AddScoped<IValidator<CreatePatientRequestModel>, CreatePatientModelValidation>()
    .AddScoped<IValidator<UpdatePatientRequestModel>, UpdatePatientModelValidation>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();