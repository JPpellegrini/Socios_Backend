using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class PrestacionConfiguration : IEntityTypeConfiguration<Prestacion>
    {
        public void Configure(EntityTypeBuilder<Prestacion> builder)
        {
            builder.ToTable("prestaciones");
            builder.HasKey(c => c.Id_Prestacion);
            builder.Property(c => c.NombrePrestacion).IsRequired().HasMaxLength(100);
        }
    }
}
