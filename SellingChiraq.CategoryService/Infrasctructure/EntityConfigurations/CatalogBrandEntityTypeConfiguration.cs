using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SellingChiraq.CategoryService.Core.Domain;
using SellingChiraq.CategoryService.Infrasctructure.Context;


namespace SellingChiraq.CategoryService.Infrasctructure.EntityConfigurations;
public class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
{
    public void Configure(EntityTypeBuilder<CatalogBrand> builder)
    {
        builder.ToTable("CatalogBrand",CatalogContext.DEFAULT_SCHEMA);

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
                .UseHiLo("catalog_brand_hilo")
                .IsRequired();

        builder.Property(cb => cb.Brand)
               .IsRequired()
               .HasMaxLength(100);
    }
}

