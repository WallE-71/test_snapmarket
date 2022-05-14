using Microsoft.AspNetCore.Mvc;
using SnapMarket.Common.Attributes;

namespace SnapMarket.Areas.Admin.Controllers
{
    [Area(AreaConstants.adminArea)]
    public class BaseController : Controller
    {
        public const string ErrorMessage = "خطایی رخ داده.";
        public const string OperationSuccess = "عملیات با موفقیت انجام شد.";
        public const string DeleteSuccess = "حذف اطلاعات با موفقیت انجام شد.";
        public const string InsertSuccess = "درج اطلاعات با موفقیت انجام شد.";
        public const string EditSuccess = "ویرایش اطلاعات با موفقیت انجام شد.";
        public const string DeleteGroupSuccess = "حذف گروهی اطلاعات با موفقیت انجام شد.";

        public IActionResult Notification()
        {
            return Content(TempData["notification"].ToString());
        }

        [HttpGet, AjaxOnly]
        public IActionResult DeleteGroup()
        {
            return PartialView("_DeleteGroup");
        }
    }
}
