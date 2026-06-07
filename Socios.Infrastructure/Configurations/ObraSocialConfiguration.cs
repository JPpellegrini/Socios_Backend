using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class ObraSocialConfiguration : IEntityTypeConfiguration<ObraSocial>
    {
        public void Configure(EntityTypeBuilder<ObraSocial> builder)
        {
            builder.ToTable("obras_sociales");
            builder.HasKey(c => c.Id_ObraSocial);
            builder.Property(c => c.NombreObraSocial).IsRequired().HasMaxLength(100);
        }
    }
}
