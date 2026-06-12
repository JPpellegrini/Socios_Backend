using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class CierreContableConfiguration : IEntityTypeConfiguration<CierreContable>
    {
        public void Configure(EntityTypeBuilder<CierreContable> builder)
        {
            builder.ToTable("cierres_contables");
            builder.HasKey(c => c.Id_Cierre);
            builder.Property(c => c.Tipo).IsRequired().HasMaxLength(20);
            builder.Property(c => c.Mes_Periodo).HasColumnType("int");
            builder.Property(c => c.Anio_Periodo).HasColumnType("int");
            builder.Property(c => c.Fechor).IsRequired();
            builder.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.Id_Usuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
