using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class MovimientoPrestacionConfiguration : IEntityTypeConfiguration<MovimientoPrestacion>
    {
        public void Configure(EntityTypeBuilder<MovimientoPrestacion> builder)
        {
            builder.ToTable("movimientos_prestaciones");
            builder.HasKey(mp => mp.Id_MovimientoPrestacion);
            builder.HasOne(s => s.Caja)
                .WithMany()
                .HasForeignKey(s => s.Id_Movimiento)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(s => s.Prestacion)
                .WithMany()
                .HasForeignKey(s => s.Id_Prestacion)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
