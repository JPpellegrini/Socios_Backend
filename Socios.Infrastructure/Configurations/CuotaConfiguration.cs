using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class CuotaConfiguration : IEntityTypeConfiguration<Cuota>
    {
        public void Configure(EntityTypeBuilder<Cuota> builder)
        {
            builder.ToTable("cuotas");
            builder.HasKey(c => c.Id_Deuda);
            builder.HasOne(s => s.Entidad)
                .WithMany()
                .HasForeignKey(s => s.Id_Entidad)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(c => c.Mes_Periodo).HasColumnType("int");
            builder.Property(c => c.Anio_Periodo).HasColumnType("int");
            builder.HasOne(s => s.TipoCuota)
                .WithMany()
                .HasForeignKey(s => s.Id_TipoCuota)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(c => c.Monto).HasColumnType("decimal(18,2)");
            builder.Property(c => c.FechaHoraGeneracion).IsRequired();
            builder.Property(c => c.FechaVencimiento).IsRequired();
            builder.Property(c => c.Estado).IsRequired().HasMaxLength(20);
            builder.HasOne(s => s.Caja)
                .WithMany()
                .HasForeignKey(s => s.Id_Movimiento)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
