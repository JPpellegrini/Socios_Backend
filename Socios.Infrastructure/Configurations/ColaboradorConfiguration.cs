using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class ColaboradorConfiguration : IEntityTypeConfiguration<Colaborador>
    {
        public void Configure(EntityTypeBuilder<Colaborador> builder)
        {
            builder.ToTable("colaboradores");
            builder.HasKey(c => c.Id_Colaborador);

            builder.HasOne(c => c.Prestacion)
                .WithMany()
                .HasForeignKey(c => c.Id_Prestacion)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Entidad)
                .WithMany()
                .HasForeignKey(c => c.Id_Entidad)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
