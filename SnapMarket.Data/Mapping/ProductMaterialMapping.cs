using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnapMarket.Entities;

namespace SnapMarket.Data.Mapping
{
    public class ProductMaterialMapping : IEntityTypeConfiguration<ProductMaterial>
    {
        public void Configure(EntityTypeBuilder<ProductMaterial> builder)
        {
            builder.HasKey(pm => new { pm.ProductId, pm.MaterialId });
            builder
              .HasOne(pm => pm.Product)
              .WithMany(p => p.ProductMaterials)
              .HasForeignKey(pm => pm.ProductId);

            builder
               .HasOne(p => p.Material)
               .WithMany(m => m.ProductMaterials)
               .HasForeignKey(pm => pm.MaterialId);
        }
    }
}
