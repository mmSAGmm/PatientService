using DbDataAccess;
using Patient.Domain;
using PatientService.Automapper;
using PatientService.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddDomain();
builder.Services.AddDbs();
builder.Services.AddAutoMapper(x => x.AddProfile(typeof(ServiceProfile)));

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();