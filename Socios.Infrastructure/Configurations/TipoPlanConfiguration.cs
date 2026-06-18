using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class TipoPlanConfiguration : IEntityTypeConfiguration<TipoPlan>
    {
        public void Configure(EntityTypeBuilder<TipoPlan> builder)
        {
            builder.ToTable("tipo_planes");
            builder.HasKey(c => c.Id_TipoPlan);
            builder.HasOne(x => x.TipoCuota)
                .WithMany()
                .HasForeignKey(x => x.Id_TipoCuota)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(c => c.EdadTope);
            builder.Property(c => c.Tipo_Plan).IsRequired();
        }
    }
}
