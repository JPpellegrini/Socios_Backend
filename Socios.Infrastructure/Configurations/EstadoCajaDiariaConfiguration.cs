using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class EstadoCajaDiariaConfiguration : IEntityTypeConfiguration<EstadoCajaDiaria>
    {
        public void Configure(EntityTypeBuilder<EstadoCajaDiaria> builder)
        {
            builder.ToTable("estado_cajadiaria");
            builder.HasKey(e => e.Id_CajaDiaria);
            
            builder.Property(c => c.Tipo).IsRequired().HasMaxLength(20);
            builder.Property(c => c.FechaHora).IsRequired();
            builder.Property(c => c.Saldo).IsRequired().HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.Id_Usuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
