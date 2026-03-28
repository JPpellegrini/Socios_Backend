using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;
using Socios.Infrastructure.Repositories;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
