using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class MetodoPagoConfiguration : IEntityTypeConfiguration<MetodoPago>
    {
        public void Configure(EntityTypeBuilder<MetodoPago> builder)
        {
            builder.ToTable("metodos_pago");
            builder.HasKey(c => c.Id_MetodoPago);
            builder.Property(c => c.NombreMetodoPago).IsRequired().HasMaxLength(50);
        }
    }
}
