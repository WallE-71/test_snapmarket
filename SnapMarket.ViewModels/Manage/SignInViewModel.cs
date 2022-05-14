using Microsoft.AspNetCore.Mvc;
using SnapMarket.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SnapMarket.ViewModels.Manage
{
    public class SignInViewModel
    {
        [Display(Name = "نام کاربری"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string UserName { get; set; }


        [DataType(DataType.Password), Display(Name = "کلمه عبور"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Password { get; set; }


        [Display(Name = "مرا به خاطر بسپار؟")]
        public bool RememberMe { get; set; }


        //[GoogleRecaptchaValidation, BindProperty(Name = "g-recaptcha-response")]
        //public string GoogleRecaptchaResponse { get; set; }
    }
}
