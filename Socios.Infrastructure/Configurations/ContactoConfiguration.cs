using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class ContactoConfiguration : IEntityTypeConfiguration<Contacto>
    {
        public void Configure(EntityTypeBuilder<Contacto> builder)
        {
            builder.ToTable("contactos");
            builder.HasKey(c => c.Id_Contacto);

            builder.Property(c => c.Tipo).IsRequired().HasMaxLength(50);
            builder.Property(c => c.ContactoEntidad).IsRequired().HasMaxLength(250);

            builder.HasOne(c => c.Entidad)
                .WithMany()
                .HasForeignKey(c => c.Id_Entidad)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
