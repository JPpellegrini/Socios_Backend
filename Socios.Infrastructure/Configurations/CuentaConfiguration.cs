using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class CuentaConfiguration : IEntityTypeConfiguration<Cuenta>
    {
        public void Configure(EntityTypeBuilder<Cuenta> builder)
        {
            builder.ToTable("cuentas");
            builder.HasKey(c => c.Id_Cuenta);
            builder.Property(c => c.NombreCuenta).IsRequired().HasMaxLength(100);
            builder.Property(c => c.NroCuenta).IsRequired();
            builder.Property(c => c.Estado).IsRequired().HasMaxLength(20);
        }
    }
}
