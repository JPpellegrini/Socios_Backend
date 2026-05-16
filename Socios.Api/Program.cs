using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;
using Socios.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Socios.Api.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DbContext

builder.Services.AddDbContext<SociosDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("CJR_Socios")));

#endregion

#region Inyecciones de dependencias

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

#endregion

#region Authentication - Basic Auth

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

#endregion

builder.Services.AddAuthorization();

var app = builder.Build();

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
