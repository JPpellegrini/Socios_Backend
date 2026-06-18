using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class TipoCuotaConfiguration : IEntityTypeConfiguration<TipoCuota>
    {
        public void Configure(EntityTypeBuilder<TipoCuota> builder)
        {
            builder.ToTable("tipo_cuotas");
            builder.HasKey(c => c.Id_TipoCuota);
            builder.Property(c => c.Concepto).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Importe).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(c => c.Tiene_EdadTope).IsRequired().HasColumnType("boolean");
            builder.Property(c => c.Fecha_ultimamodif).IsRequired();
        }
    }
}
