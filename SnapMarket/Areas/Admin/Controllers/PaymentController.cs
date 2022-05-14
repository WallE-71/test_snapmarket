﻿using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Attributes;
using SnapMarket.Common.Extensions;
using SnapMarket.Data.Repositories;
using SnapMarket.ViewModels.DynamicAccess;
using static SnapMarket.ViewModels.RequestPay.RequestPayViewModel;

namespace SnapMarket.Areas.Admin.Controllers
{
    [DisplayName("مدیریت پرداخت ها")]
    public class PaymentController : BaseController
    {
        private readonly IUnitOfWork _uw;
        private const string RequestPayNotFound = "پرداختی یافت نشد.";
        public PaymentController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet, DisplayName("پرداخت ها"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> GetRequestPays(int offset, int limit, string search, string order, string sort)
        {
            List<ItemRequestPay> viewModels;
            int total = _uw.BaseRepository<RequestPay>().CountEntities();
            if (limit == 0)
                limit = total;
            viewModels = await _uw.PaymentRepository.GetPaginateRequestPaysAsync(offset, limit, true, search);
            if (search != null)
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(string requestPayId)
        {
            if (!requestPayId.HasValue())
                ModelState.AddModelError(string.Empty, RequestPayNotFound);
            else
            {
                var pay = await _uw.BaseRepository<RequestPay>().FindByIdAsync(requestPayId);
                if (pay == null)
                    ModelState.AddModelError(string.Empty, RequestPayNotFound);
                else
                    return PartialView("_DeleteConfirmation", pay);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(RequestPay model)
        {
            if (model.Id == null)
                ModelState.AddModelError(string.Empty, RequestPayNotFound);
            else
            {
                var pay = await _uw.BaseRepository<RequestPay>().FindByIdAsync(model.Id);
                if (pay == null)
                    ModelState.AddModelError(string.Empty, RequestPayNotFound);
                else
                {
                    pay.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<RequestPay>().Update(pay);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", pay);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ پرداختی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var request = await _uw.BaseRepository<RequestPay>().FindByIdAsync(int.Parse(splite));
                    request.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<RequestPay>().Update(request);
                }
                await _uw.Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(RequestPayNotFound);
        }
    }
}
