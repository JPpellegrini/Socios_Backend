using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuarios");
            builder.HasKey(x => x.Id_Usuario);
            builder.Property(x => x.UsuarioNombre).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Password).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Estado).IsRequired().HasMaxLength(50);
            builder.HasOne(x => x.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(x => x.Id_Rol)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
