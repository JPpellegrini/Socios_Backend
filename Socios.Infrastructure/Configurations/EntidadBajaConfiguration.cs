using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class EntidadBajaConfiguration : IEntityTypeConfiguration<EntidadBaja>
    {
        public void Configure(EntityTypeBuilder<EntidadBaja> builder)
        {
            builder.ToTable("entidad_bajas");
            builder.HasKey(e => e.Id_Baja);

            builder.Property(e => e.Motivo).IsRequired().HasMaxLength(250);
            builder.Property(e => e.Fecha_Baja).IsRequired();

            builder.HasOne(e => e.EntidadTipo)
                .WithMany()
                .HasForeignKey(e => e.Id_EntidadTipo)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
