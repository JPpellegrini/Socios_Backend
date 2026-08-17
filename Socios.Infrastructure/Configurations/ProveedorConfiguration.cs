using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
    {
        public void Configure(EntityTypeBuilder<Proveedor> builder)
        {
            builder.ToTable("proveedores");
            builder.HasKey(p => p.Id_Proveedor);

            // El servicio prestado.
            builder.HasOne(p => p.Prestacion)
                .WithMany()
                .HasForeignKey(p => p.Id_Prestacion)
                .OnDelete(DeleteBehavior.Restrict);

            // La entidad (persona o empresa) que es el proveedor.
            builder.HasOne(p => p.Entidad)
                .WithMany()
                .HasForeignKey(p => p.Id_Entidad)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
