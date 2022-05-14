using System;

namespace SnapMarket.Entities
{
    public class Advertising : BaseEntity<int>
    {
        public string Url { get; set; }
        public string ImageName { get; set; }
        public ImageLocation ImageLocation { get; set; }
    }
}
