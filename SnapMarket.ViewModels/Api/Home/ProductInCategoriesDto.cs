using System;

namespace SnapMarket.ViewModels.Api.Home
{
    public  class ProductInCategoriesDto : BaseViewModel<string>
    {
        public string Seller { get; set; }
        public string NameOfBrand { get; set; }
        public long Price { get; set; }
        public int Stock { get; set; }
        public string ImageName { get; set; }
        public bool? IsDelete { get; set; }
        public bool IsProvide { get; set; }
        public string Status { get; set; }
        public double PercentDiscount { get; set; }
        public string NameOfCategories { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string PersianExpirationDate { get; set; }
        public string ActionDiscount { get; set; }
    }
}
