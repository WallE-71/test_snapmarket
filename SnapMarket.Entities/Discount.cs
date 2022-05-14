using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnapMarket.Entities
{
    public class Discount
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ProductId { get; set; }
        public double Percent { get; set; }
        public string DiscountCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime EndDate { get; set; }
    }

    //public enum DiscountType
    //{
    //    [Display(Name = "ندارد")]
    //    None = 1,

    //    [Display(Name = "ویژه")]
    //    Special = 2,

    //    [Display(Name = "فصلی")]
    //    Season = 3,

    //    [Display(Name = "هفتگی")]
    //    Weekly = 4,

    //    [Display(Name = "کد تخفیف")]
    //    Code = 5,
    //}
}
