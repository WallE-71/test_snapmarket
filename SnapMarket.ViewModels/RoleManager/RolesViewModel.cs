using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System;

namespace SnapMarket.ViewModels.RoleManager
{
    public class RolesViewModel
    {
        [JsonPropertyName("Id")]
        public int? Id { get; set; }


        [JsonPropertyName("ردیف")]
        public int Row { get; set; }


        [Display(Name = "عنوان نقش"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")/*, JsonPropertyName("عنوان نقش")*/]
        public string Name { get; set; }


        [Display(Name = "توضیحات"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")/*, JsonPropertyName("توضیحات")*/]
        public string Description { get; set; }


        [Display(Name = "تعداد کاربران")]
        public int UsersCount { get; set; }


        [JsonIgnore]
        public DateTime? InsertTime { get; set; }
    }
}
