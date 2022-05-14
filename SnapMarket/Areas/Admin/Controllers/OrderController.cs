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
using SnapMarket.ViewModels.Order;
using SnapMarket.Common.Extensions;
using SnapMarket.Common.Attributes;
using SnapMarket.ViewModels.DynamicAccess;

namespace SnapMarket.Areas.Admin.Controllers
{
    [DisplayName("مدیریت سفارشات")]
    public class OrderController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IMemoryCache _cache;
        private const string OrderNotFound = "سفارش درخواستی یافت نشد.";
        public OrderController(IUnitOfWork uw, IMapper mapper, IMemoryCache cache)
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

        [HttpGet]
        public IActionResult GetOrders(string search, string order, int offset, int limit, string sort)
        {
            List<OrderViewModel> viewModels;
            int total = _uw.BaseRepository<Order>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "productName")
            {
                if (order == "asc")
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "ProductName", search);
                else
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "ProductName desc", search);
            }
            else if (sort == "requestPayId")
            {
                if (order == "asc")
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "RequestPayId", search);
                else
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "RequestPayId desc", search);
            }
            else if (sort == "amountPaid")
            {
                if (order == "asc")
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "AmountPaid", search);
                else
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "AmountPaid desc", search);
            }
            else if (sort == "quantity")
            {
                if (order == "asc")
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "Quantity", search);
                else
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "Quantity desc", search);
            }
            else if (sort == "persianInsertTime")
            {
                if (order == "asc")
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "InsertTime", search);
                else
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "InsertTime desc", search);
            }
            else if (sort == "orderState")
            {
                if (order == "asc")
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "OrderState", search);
                else
                    viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "OrderState desc", search);
            }
            else
                viewModels = _uw.OrderRepository.GetPaginateOrders(offset, limit, "RequestPayId", search);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet]
        public async Task<IActionResult> ChangeOrderState(int orderId, string orderState)
        {
            if (orderId == 0)
            {
                var order = await _uw.BaseRepository<Order>().FindByIdAsync(orderId);
                if (order != null)
                {
                    order.UpdateTime = DateTime.Now;
                    order.States = orderState == "0" ? OrderState.Processing : orderState == "1" ? OrderState.Confirmed :
                        orderState == "2" ? OrderState.Transmission : OrderState.Delivered;
                    _uw.BaseRepository<Order>().Update(order);
                    await _uw.Commit();
                    return Json("Success");
                }
            }
            return Json($"عضوی با شناسه '{orderId}' یافت نشد !!!");
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int orderId)
        {
            if (orderId == 0)
                ModelState.AddModelError(string.Empty, OrderNotFound);
            else
            {
                var order = await _uw.BaseRepository<Order>().FindByIdAsync(orderId);
                if (order == null)
                    ModelState.AddModelError(string.Empty, OrderNotFound);
                else
                    return PartialView("_DeleteConfirmation", order);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Order model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, OrderNotFound);
            else
            {
                var order = await _uw.BaseRepository<Order>().FindByIdAsync(model.Id);
                if (order == null)
                    ModelState.AddModelError(string.Empty, OrderNotFound);
                else
                {
                    order.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Order>().Update(order);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", order);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ سفارشی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var order = await _uw.BaseRepository<Order>().FindByIdAsync(int.Parse(splite));
                    order.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Order>().Update(order);
                }
                await _uw.Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(OrderNotFound);
        }
    }
}
