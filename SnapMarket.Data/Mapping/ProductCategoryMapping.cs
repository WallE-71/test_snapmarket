﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnapMarket.Entities;

namespace SnapMarket.Data.Mapping
{
    public class ProductCategoryMapping : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.HasKey(t => new { t.ProductId, t.CategoryId });
            builder
              .HasOne(p => p.Product)
              .WithMany(t => t.ProductCategories)
              .HasForeignKey(f => f.ProductId);

            builder
               .HasOne(p => p.Category)
               .WithMany(t => t.ProductCategories)
               .HasForeignKey(f => f.CategoryId);
        }
    }
}
