using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class DetalleMovimientoConfiguration : IEntityTypeConfiguration<DetalleMovimiento>
    {
        public void Configure(EntityTypeBuilder<DetalleMovimiento> builder)
        {
            builder.ToTable("detalle_movimientos");
            builder.HasKey(c => c.Id_Detmov);
            builder.Property(c => c.NombreDetalleMovimiento).IsRequired().HasMaxLength(100);
        }
    }
}
