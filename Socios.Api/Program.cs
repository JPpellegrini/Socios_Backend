using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;
using Socios.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Socios.Api.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DbContext

builder.Services.AddDbContext<SociosDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("CJR_Socios")));

#endregion

#region Inyecciones de dependencias

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

#endregion

// Authentication
//builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();//.RequireAuthorization();

app.Run();
