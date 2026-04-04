using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;
using Socios.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Socios.Api.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region DbContext

builder.Services.AddDbContext<SociosDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("CJR_Socios")));

#endregion

#region Inyecciones de dependencias

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

#endregion

// Authentication
builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
