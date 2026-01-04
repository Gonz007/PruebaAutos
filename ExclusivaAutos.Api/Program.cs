using ExclusivaAutos.Api.Middlewares;
using ExclusivaAutos.Application.Services;
using ExclusivaAutos.Domain.Interfaces;
using ExclusivaAutos.Infraestructure.Configuration;
using ExclusivaAutos.Infraestructure.Security;
using ExclusivaAutos.Infrastructure.Http;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ExternalApiSettings>(
    builder.Configuration.GetSection("ExternalApiSettings"));

var encryptionKey = builder.Configuration["Encryption:Key"]
    ?? throw new InvalidOperationException("Encryption:Key no configurado");
var encryptionIv = builder.Configuration["Encryption:IV"]
    ?? throw new InvalidOperationException("Encryption:IV no configurado");

builder.Services.AddSingleton<IEncryptionService>(
    new EncryptionService(encryptionKey, encryptionIv));

builder.Services.AddHttpClient();

builder.Services.AddScoped<ICustomerApplitacionService, CustomerService>();
builder.Services.AddScoped<ICustomerService, ExternalCustomerService>();
builder.Services.AddScoped<IOAuthTokenService, OAuthTokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();