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
            builder.HasKey(c => c.Id_Codeudor);

            // La entidad codeudora (la persona que avala).
            builder.HasOne(c => c.EntidadCodeudor)
                .WithMany()
                .HasForeignKey(c => c.Id_EntidadCodeudor)
                .OnDelete(DeleteBehavior.Restrict);

            // La entidad del socio al que avala.
            builder.HasOne(c => c.Entidad)
                .WithMany()
                .HasForeignKey(c => c.Id_Entidad)
                .OnDelete(DeleteBehavior.Restrict);

            // Evitar que los pares sean iguales mediante comprobación en la aplicación o trigger en BD.
        }
    }
}
