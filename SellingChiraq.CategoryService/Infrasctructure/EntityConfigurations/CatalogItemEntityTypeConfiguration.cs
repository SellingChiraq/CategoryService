using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SellingChiraq.CategoryService.Core.Domain;
using SellingChiraq.CategoryService.Infrasctructure.Context;


namespace SellingChiraq.CategoryService.Infrasctructure.EntityConfigurations;
public class CatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable("Catalog",CatalogContext.DEFAULT_SCHEMA);

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
                .UseHiLo("catalog_hilo")
                .IsRequired();

        builder.Property(cb => cb.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(cb => cb.Price)
               .IsRequired();

        builder.Property(cb => cb.PictureFileName)
               .IsRequired(false);

        builder.Ignore(cb => cb.PictureUri);

        builder.HasOne(cb => cb.CatalogBrand)
               .WithMany()
               .HasForeignKey(cb=>cb.CatalogBrandId);

        builder.HasOne(cb => cb.CatalogType)
               .WithMany()
               .HasForeignKey(cb=>cb.CatalogTypeId);
    }
}

