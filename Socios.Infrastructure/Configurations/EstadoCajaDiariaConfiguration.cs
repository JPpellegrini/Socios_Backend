using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class EstadoCajaDiariaConfiguration : IEntityTypeConfiguration<EstadoCajaDiaria>
    {
        public void Configure(EntityTypeBuilder<EstadoCajaDiaria> builder)
        {
            builder.ToTable("estado_caja_diaria");
            builder.HasKey(e => e.Id_CajaDiaria);
            builder.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.Id_Usuario)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(c => c.Tipo).IsRequired().HasMaxLength(20);
            builder.Property(c => c.FechaHora).IsRequired();
            builder.Property(c => c.Saldo).HasColumnType("decimal(18,2)");
        }
    }
}
