using System.ComponentModel.DataAnnotations;
using SnapMarket.Entities;
using SnapMarket.Common.Attributes;
using System.Collections.Generic;

namespace SnapMarket.ViewModels.Slider
{
    public class SliderViewModel : BaseViewModel<int>
    {
        public string ImageFile { get; set; }

        [Display(Name = "آدرس اسلایدر"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), UrlValidate("/", @"\", " ")]
        public string Url { get; set; }

        [Display(Name = "محل قرارگیری اسلایدر"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public ImageLocation ImageLocation { get; set; }
        public TypeOfSlider TypeOfSlider { get; set; }        
        public string Title { get; set; }
        public List<string> ImageFiles { get; set; }
        public List<string> DescriptionsSlider { get; set; }        
    }
}
