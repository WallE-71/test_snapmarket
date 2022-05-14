using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnapMarket.Entities;

namespace SnapMarket.Data.Mapping
{
    public class LikeMapping : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(t => new { t.CommentId, t.BrowserId });
            builder
              .HasOne(p => p.Comment)
              .WithMany(t => t.Likes)
              .HasForeignKey(f => f.CommentId);
        }
    }
}
