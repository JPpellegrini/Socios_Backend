using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class NichoConfiguration : IEntityTypeConfiguration<Nicho>
    {
        public void Configure(EntityTypeBuilder<Nicho> builder)
        {
            builder.ToTable("nichos");
            builder.HasKey(n => n.Id_Nicho);
            builder.HasOne(n => n.Entidad)
                .WithMany()
                .HasForeignKey(n => n.Id_Entidad)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(n => n.Sector).IsRequired().HasMaxLength(100);
            builder.Property(n => n.NroNicho).IsRequired().HasMaxLength(50);
            builder.Property(n => n.ValorNicho).IsRequired();
            builder.Property(n => n.ValorLapida);
            builder.Property(n => n.ConLapida).IsRequired().HasMaxLength(2);
            builder.Property(n => n.Ocupado).IsRequired().HasMaxLength(2);
            builder.Property(n => n.Cuotas).HasColumnType("int");
            builder.Property(n => n.InteresMensual).HasColumnType("decimal(5,2)");
        }
    }
}
