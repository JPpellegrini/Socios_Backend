using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class EntidadTipoConfiguration : IEntityTypeConfiguration<EntidadTipo>
    {
        public void Configure(EntityTypeBuilder<EntidadTipo> builder)
        {
            builder.ToTable("entidad_tipos");
            builder.HasKey(e => e.Id_EntidadTipo);

            builder.Property(e => e.Fecha_Alta).IsRequired().HasColumnType("date");
            builder.Property(e => e.Estado).IsRequired().HasMaxLength(50);

            builder.HasOne(e => e.Entidad)
                .WithMany()
                .HasForeignKey(e => e.Id_Entidad)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.TipoEntidad)
                .WithMany()
                .HasForeignKey(e => e.Id_Tipo)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
