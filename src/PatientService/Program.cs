using DbDataAccess;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Patient.Domain;
using PatientService.HostedServices;
using PatientService.Middleware;
using PatientService.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions();
builder.Services.AddHostedService<CrawlerService>();

builder.Services.AddOpenTelemetry()
       .WithTracing(builder => builder
           .AddAspNetCoreInstrumentation()
           .AddHttpClientInstrumentation()
           //.AddConsoleExporter()

           
           )
        .WithMetrics(builder => builder
            .AddAspNetCoreInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            // Metrics provided by System.Net libraries
            .AddMeter("System.Net.Http")
            .AddMeter("System.Net.NameResolution")

            .AddHttpClientInstrumentation()
          //  .AddConsoleExporter()
            .AddPrometheusExporter())
        .WithLogging(builder => { });



builder.Services
    .AddControllers(c => c.Filters.Add(new ValidationFilterAttribute()));

builder.Services
    .AddDomain()
    .AddDbs(builder.Configuration)
    .AddService();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapPrometheusScrapingEndpoint();

app.Run();