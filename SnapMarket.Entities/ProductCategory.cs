using SnapMarket.Entities.Identity;

namespace SnapMarket.Entities
{
    public class ProductCategory
    {
        public string ProductId { get; set; }
        public Product Product { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
