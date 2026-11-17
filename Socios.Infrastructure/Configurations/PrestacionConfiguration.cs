using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class PrestacionConfiguration : IEntityTypeConfiguration<Prestacion>
    {
        public void Configure(EntityTypeBuilder<Prestacion> builder)
        {
            builder.ToTable("prestaciones", t =>
            {
                // La base solo acepta los valores válidos, aunque se cargue por fuera de la API.
                t.HasCheckConstraint("CK_prestaciones_Tipo", "\"Tipo\" IN ('COLABORACION', 'PRESTACION')");
                t.HasCheckConstraint("CK_prestaciones_Estado", "\"Estado\" IN ('ACTIVA', 'INACTIVA')");
            });
            builder.HasKey(c => c.Id_Prestacion);
            builder.Property(c => c.NombrePrestacion).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Tipo).IsRequired().HasMaxLength(20);
            builder.Property(c => c.Estado).IsRequired().HasMaxLength(10);
        }
    }
}
