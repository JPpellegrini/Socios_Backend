using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class EntidadConfiguration : IEntityTypeConfiguration<Entidad>
    {
        public void Configure(EntityTypeBuilder<Entidad> builder)
        {
            builder.ToTable("entidades");
            builder.HasKey(e => e.Id_Entidad);

            builder.Property(e => e.Tipo).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Dni).HasMaxLength(50);
            builder.Property(e => e.CuitCuil).HasMaxLength(50);
            builder.Property(e => e.Nombre).HasMaxLength(150);
            builder.Property(e => e.Apellido).HasMaxLength(150);
            builder.Property(e => e.RazonSocial).HasMaxLength(250);
            builder.Property(e => e.Sexo).HasMaxLength(50);
            builder.Property(e => e.Nacimiento).HasColumnType("date");
            builder.Property(e => e.Calle).HasMaxLength(200);
            builder.Property(e => e.Observacion).HasMaxLength(500);

            builder.HasOne(e => e.Ciudad)
                .WithMany()
                .HasForeignKey(e => e.Id_Ciudad)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
