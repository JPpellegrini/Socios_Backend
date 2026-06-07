using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Configurations
{
    public class TipoEntidadConfiguration : IEntityTypeConfiguration<TipoEntidad>
    {
        public void Configure(EntityTypeBuilder<TipoEntidad> builder)
        {
            builder.ToTable("tipo_entidades");
            builder.HasKey(c => c.Id_Tipo);
            builder.Property(c => c.NombreTipoEntidad).IsRequired().HasMaxLength(50);
        }
    }
}
