using DbDataAccess;
using Patient.Domain;
using PatientService.HostedServices;
using PatientService.Middleware;
using PatientService.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions();
builder.Services.AddHostedService<CrawlerService>();

builder.Services
    .AddControllers(c=> c.Filters.Add(new ValidationFilterAttribute()));

builder.Services
    .AddDomain()
    .AddDbs(builder.Configuration)
    .AddService();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();