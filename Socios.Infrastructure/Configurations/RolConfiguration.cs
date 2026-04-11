using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class RolConfiguration : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.ToTable("roles");
            builder.HasKey(r => r.Id_Rol);
            builder.Property(r => r.RolNombre).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Descripcion).IsRequired().HasMaxLength(255);
        }
    }
}
