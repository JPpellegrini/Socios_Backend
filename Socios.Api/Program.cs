using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;
using Socios.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Socios.Api.Authentication;
using Socios.Api.Middleware;
using Socios.Application.UseCases.Socios;
using Socios.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DbContext - Configuracion por entorno

var configuration = builder.Configuration;

var environment = configuration["Environment"];

string dbConnectionString;

if (environment == "Development")
{
    // PostgreSQL local
    dbConnectionString = configuration.GetConnectionString("CJR_Socios") 
        ?? throw new InvalidOperationException("Connection string 'PostgreLocal' no encontrada");
    Console.WriteLine("[Development] Usando PostgreSQL local");
}
else if (environment == "Staging")
{
    // Supabase para Staging
    dbConnectionString = configuration.GetConnectionString("Supabase_Staging") 
        ?? throw new InvalidOperationException("Connection string 'Supabase_Staging' no encontrada");
    Console.WriteLine("[Staging] Usando Supabase");
}
else
{
    // Azure PostgreSQL para Producci�n
    dbConnectionString = configuration.GetConnectionString("Azure_Production") 
        ?? throw new InvalidOperationException("Connection string 'Azure_Production' no encontrada");
    Console.WriteLine("[Production] Usando Azure PostgreSQL");
}

builder.Services.AddDbContext<SociosDbContext>(options =>
    options.UseNpgsql(dbConnectionString));

#endregion

#region Inyecciones de dependencias

// Repositorios (uno por entidad, cada uno se ocupa solo de su tabla)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICiudadRepository, CiudadRepository>();
builder.Services.AddScoped<ISocioRepository, SocioRepository>();
builder.Services.AddScoped<IEntidadRepository, EntidadRepository>();
builder.Services.AddScoped<IEntidadTipoRepository, EntidadTipoRepository>();
builder.Services.AddScoped<IContactoRepository, ContactoRepository>();
builder.Services.AddScoped<ITipoEntidadRepository, TipoEntidadRepository>();
builder.Services.AddScoped<IEntidadBajaRepository, EntidadBajaRepository>();

// Unidad de trabajo (dueña de la transacción) y casos de uso (orquestan la lógica)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISocioUseCase, SocioUseCase>();

#endregion

#region Authentication - Basic Auth

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

#endregion

builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware de manejo de errores: va primero (lo más "afuera") para atrapar
// cualquier excepción que ocurra más adentro y devolver una respuesta prolija.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
