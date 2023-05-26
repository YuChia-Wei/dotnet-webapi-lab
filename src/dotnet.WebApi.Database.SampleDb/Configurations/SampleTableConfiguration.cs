using dotnet.WebApi.Database.SampleDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dotnet.WebApi.Database.SampleDb.Configurations;

public class SampleTableConfiguration : IEntityTypeConfiguration<SampleTable>
{
    /// <summary>
    /// Configures the entity of type <typeparamref name="TEntity" />.
    /// </summary>
    /// <param name="entity">The entity to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<SampleTable> entity)
    {
        entity.HasKey(e => e.SerialId);

        entity.ToTable("SampleTable", "dbo", builder => builder.HasComment("範例資料"));

        entity.Property(e => e.SerialId)
              .HasComment("序號");

        entity.Property(e => e.Description)
              .HasMaxLength(100)
              .IsUnicode()
              .HasComment("敘述");

        entity.Property(e => e.DocumentNum)
              .IsRequired()
              .HasComment("文件編號");
    }
}