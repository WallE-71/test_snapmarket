using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.ViewModels.Cart;
using SnapMarket.Common.Attributes;
using SnapMarket.Common.Extensions;
using SnapMarket.Services.Contracts;
using SnapMarket.ViewModels.DynamicAccess;

namespace SnapMarket.Areas.Admin.Controllers
{
    [DisplayName("مدیریت سبد خرید")]
    public class CartController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IMemoryCache _cache;
        private const string CartNotFound = "سبد خرید یافت نشد.";
        public CartController(IUnitOfWork uw, IMapper mapper, IMemoryCache cache, IApplicationUserManager userManager)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _cache = cache;
            _cache.CheckArgumentIsNull(nameof(_cache));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet, DisplayName("سبد خرید"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> GetCarts(string search, string order, int offset, int limit, string sort)
        {
            List<CartViewModel> cartViewModels;
            int total = _uw.BaseRepository<Cart>().CountEntities();
            if (limit == 0)
                limit = total;

            cartViewModels = await _uw.CartRepository.GetPaginateCartsAsync(offset, limit, "Id", search);
            if (search != "")
                total = cartViewModels.Count();
            return Json(new { total = total, rows = cartViewModels });
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int cartId)
        {
            if (cartId == 0)
                ModelState.AddModelError(string.Empty, CartNotFound);
            else
            {
                var cart = await _uw.BaseRepository<Cart>().FindByIdAsync(cartId);
                if (cart == null)
                    ModelState.AddModelError(string.Empty, CartNotFound);
                else
                    return PartialView("_DeleteConfirmation", cart);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Cart model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, CartNotFound);
            else
            {
                var cart = await _uw.BaseRepository<Cart>().FindByIdAsync(model.Id);
                if (cart == null)
                    ModelState.AddModelError(string.Empty, CartNotFound);
                else
                {
                    cart.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Cart>().Update(cart);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", cart);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ سبدی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var cart = await _uw.BaseRepository<Cart>().FindByIdAsync(int.Parse(splite));
                    cart.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Cart>().Update(cart);
                }
                await _uw.Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(CartNotFound);
        }
    }
}
