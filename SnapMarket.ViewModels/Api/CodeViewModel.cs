using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace SnapMarket.ViewModels.Api
{
    public class CodeViewModel
    {
        #region SendCodeTwoFactor
        //public string SelectedProvider { get; set; }


        //public ICollection<SelectListItem> Providers { get; set; }
        #endregion
        //////////////////////////////////////////


        #region VerfiyCode
        public string Provider { get; set; } = "Email";


        [Display(Name = "کد اعتبارسنجی"), /*Required(ErrorMessage = "وارد نمودن {0} الزامی است."), */JsonIgnore]
        public string Code { get; set; }


        [Display(Name = "Remember me?"), JsonIgnore]
        public bool RememberMe { get; set; } = true;


        //[Display(Name = "مرا به خاطر بسپار؟")]
        //public bool RememberBrowser { get; set; } = true;
        #endregion
    }
}
