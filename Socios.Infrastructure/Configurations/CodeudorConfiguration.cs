using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class CodeudorConfiguration : IEntityTypeConfiguration<Codeudor>
    {
        public void Configure(EntityTypeBuilder<Codeudor> builder)
        {
            builder.ToTable("codeudores");
            builder.HasKey(c => new { c.Id_Entidad_Codeudor, c.Id_Entidad });

            builder.HasOne(c => c.Entidad)
                .WithMany()
                .HasForeignKey(c => c.Id_Entidad)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.EntidadCodeudor)
                .WithMany()
                .HasForeignKey(c => c.Id_Entidad_Codeudor)
                .OnDelete(DeleteBehavior.Restrict);

            // Evitar que los pares sean iguales mediante comprobación en la aplicación o trigger en BD.
        }
    }
}
