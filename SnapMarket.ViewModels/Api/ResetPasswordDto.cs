using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SnapMarket.ViewModels.Api
{
    public class ResetPasswordDto
    {
        [/*Display(Name = "کد اعتبارسنجی"),*/ Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Code { get; set; }

        [/*Display(Name = "ایمیل"), */Required(ErrorMessage = "وارد نمودن {0} الزامی است."), EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نمی باشد.")]
        public string Email { get; set; }

        [/*Display(Name = "کلمه عبور جدید"), */DataType(DataType.Password), StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [/*Display(Name = "تکرار کلمه عبور جدید"), */DataType(DataType.Password), Compare("NewPassword", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد.")]
        public string ConfirmNewPassword { get; set; }
    }
}
