
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Repository.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            var guidToStringConverter = new ValueConverter<Guid, string>(
            v => v.ToString(),
            v => Guid.Parse(v));

            builder.Property(u => u.Id)
                .HasConversion(guidToStringConverter)
                .HasMaxLength(36);
        }

    }
}
