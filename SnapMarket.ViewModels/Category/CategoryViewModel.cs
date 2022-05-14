using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SnapMarket.ViewModels.Category
{
    public class CategoryViewModel : BaseViewModel<int>
    {
        [JsonIgnore]
        public int? ParentId { get; set; }

        [Display(Name = "دسته پدر")]
        public string ParentName { get; set; }

        public virtual ICollection<CategoryViewModel> SubCategories { get; set; }
    }
}
