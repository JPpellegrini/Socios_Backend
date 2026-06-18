using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class CajaConfiguration : IEntityTypeConfiguration<Caja>
    {
        public void Configure(EntityTypeBuilder<Caja> builder)
        {
            builder.ToTable("caja");
            builder.HasKey(x => x.Id_Movimiento);
            builder.HasOne(x => x.EstadoCajaDiaria)
                .WithMany()
                .HasForeignKey(x => x.Id_CajaDiaria)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.FechaHoraMov).IsRequired();
            builder.Property(x => x.Tipo_Movimiento).IsRequired().HasMaxLength(20);
            builder.HasOne(x => x.Entidad)
                .WithMany()
                .HasForeignKey(x => x.Id_Entidad)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(c => c.Monto).HasColumnType("decimal(18,2)");
            builder.HasOne(x => x.MetodoPago)
                .WithMany()
                .HasForeignKey(x => x.Id_MetodoPago)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Cuenta)
                .WithMany()
                .HasForeignKey(x => x.Id_Cuenta)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.DetalleMovimiento)
                .WithMany()
                .HasForeignKey(x => x.Id_Detmov)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.Observacion).HasMaxLength(200);
        }
    }
}
