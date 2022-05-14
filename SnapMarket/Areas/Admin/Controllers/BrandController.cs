﻿using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using SnapMarket.Entities;
using SnapMarket.ViewModels;
using SnapMarket.ViewModels.DynamicAccess;

namespace SnapMarket.Areas.Admin.Controllers
{
    [DisplayName("مدیریت برند ها")]
    public class BrandController : BaseController
    {
        private readonly IUnitOfWork _uw;
        private readonly IMapper _mapper;
        private const string BrandNotFound = "برند یافت نشد.";
        private const string BrandDuplicate = "نام برند تکراری است.";
        public BrandController(IUnitOfWork uw, IMapper mapper)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands(string search, string order, int offset, int limit, string sort)
        {
            List<BaseViewModel<int>> viewModels;
            int total = _uw.BaseRepository<Brand>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "برند")
            {
                if (order == "asc")
                    viewModels = await _uw.BrandRepository.GetPaginateBrandsAsync(offset, limit, "Name", search);
                else
                    viewModels = await _uw.BrandRepository.GetPaginateBrandsAsync(offset, limit, "Name desc", search);
            }
            else
                viewModels = await _uw.BrandRepository.GetPaginateBrandsAsync(offset, limit, "Name", search);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderBrand(int colorId)
        {
            var brand = new Brand();
            if (colorId != 0)
            {
                var findBrand = await _uw.BaseRepository<Brand>().FindByIdAsync(colorId);
                if (findBrand != null)
                    brand.Name = findBrand.Name;
                else
                    ModelState.AddModelError(string.Empty, BrandNotFound);
            }
            return PartialView("_RenderBrand", brand);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(Brand model)
        {
            if (ModelState.IsValid)
            {
                var findBrand = await _uw.BaseRepository<Brand>().FindByIdAsync(model.Id);
                if (model.Id != 0)
                {
                    findBrand.Name = model.Name;
                    findBrand.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    _uw.BaseRepository<Brand>().Update(findBrand);
                    await _uw.Commit();
                    TempData["notification"] = EditSuccess;
                }
                else
                {
                    var brand = new Brand();
                    brand.Name = model.Name;
                    var brands = await _uw.BaseRepository<Brand>().FindAllAsync();
                    if (brands.Count() != 0)
                        brand.Id = brands.OrderByDescending(c => c.Id).First().Id + 1;
                    else
                        brand.Id = 1;
                    brand.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    await _uw.BaseRepository<Brand>().CreateAsync(brand);
                    await _uw.Commit();
                    TempData["notification"] = InsertSuccess;
                }
            }
            return PartialView("_RenderBrand", model);
        }

        [HttpGet, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Delete(int brandId)
        {
            if (brandId == 0)
                ModelState.AddModelError(string.Empty, BrandNotFound);
            else
            {
                var brand = _uw.Context.Brands.FirstOrDefault(b => b.Id == brandId);
                if (brand == null)
                    ModelState.AddModelError(string.Empty, BrandNotFound);
                else
                    return PartialView("_DeleteConfirmation", brand);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Brand model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, BrandNotFound);
            else
            {
                var brand = _uw.Context.Brands.FirstOrDefault(b => b.Id == model.Id);
                if (brand == null)
                    ModelState.AddModelError(string.Empty, BrandNotFound);
                else
                {
                    brand.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Brand>().Update(brand);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", brand);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ برندی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var brand = await _uw.BaseRepository<Brand>().FindByIdAsync(int.Parse(splite));
                    brand.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Brand>().Update(brand);
                }
                await _uw.Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(BrandNotFound);
        }
    }
}
