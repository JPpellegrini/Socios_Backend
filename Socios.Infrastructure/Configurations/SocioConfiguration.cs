using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class SocioConfiguration : IEntityTypeConfiguration<Socio>
    {
        public void Configure(EntityTypeBuilder<Socio> builder)
        {
            builder.ToTable("socios");
            builder.HasKey(s => s.Id_Entidad);
            builder.HasOne(s => s.ObraSocial)
                .WithMany()
                .HasForeignKey(s => s.Id_OS)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(s => s.Plan).HasMaxLength(50);
            builder.Property(s => s.Sepelio).HasMaxLength(10);
            builder.Property(s => s.Cobrador).HasMaxLength(50);
            builder.Property(s => s.Numero_Afiliado).HasMaxLength(100);
            builder.HasOne(s => s.Entidad)
                .WithMany()
                .HasForeignKey(s => s.Id_Entidad)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
