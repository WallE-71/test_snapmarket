using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.ViewModels.City;
using SnapMarket.Common.Extensions;
using SnapMarket.Common.Attributes;
using SnapMarket.ViewModels.DynamicAccess;

namespace SnapMarket.Areas.Admin.Controllers
{
    [DisplayName("مدیریت استان ها")]
    public class ProvinceController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string ProvinceNotFound = "استان یافت نشد.";
        public ProvinceController(IUnitOfWork uw, IMapper mapper)
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
        public async Task<IActionResult> GetProvinces(string search, string order, int offset, int limit, string sort)
        {
            List<CityProvinceViewModel> viewModels;
            int total = _uw.BaseRepository<Province>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (order == "asc")
            {
                viewModels = await _uw.Context.Provinces.Include(p => p.Cities)
                                .Where(p => p.RemoveTime == null && p.Name.Contains(search))
                                .OrderBy("Name").Skip(offset).Take(limit)
                                .Select(p => new CityProvinceViewModel
                                {
                                    Id = p.Id,
                                    ProvinceName = p.Name,
                                    numberOfCities = p.Cities.Count,
                                    CityId = p.Cities.Where(p => p.ProvinceId == p.Id).FirstOrDefault().Id
                                }).ToListAsync();
            }
            else
            {
                viewModels = await _uw.Context.Provinces.Include(p => p.Cities)
                               .Where(p => p.RemoveTime == null && p.Name.Contains(search))
                               .OrderBy("Name desc").Skip(offset).Take(limit)
                               .Select(p => new CityProvinceViewModel
                               {
                                   Id = p.Id,
                                   ProvinceName = p.Name,
                                   numberOfCities = p.Cities.Count,
                                   CityId = p.Cities.Where(p => p.ProvinceId == p.Id).FirstOrDefault().Id
                               }).ToListAsync();
            }

            if (search != "")
                total = viewModels.Count();

            foreach (var item in viewModels)
                item.Row = ++offset;

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderProvince(int provinceId)
        {
            var viewModel = new CityProvinceViewModel();
            if (provinceId != 0)
            {
                var province = await _uw.BaseRepository<Province>().FindByIdAsync(provinceId);
                if (province != null)
                {
                    viewModel.Id = provinceId;
                    viewModel.ProvinceName = province.Name;
                }
                else
                    ModelState.AddModelError(string.Empty, ProvinceNotFound);
            }
            return PartialView("_RenderProvince", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(CityProvinceViewModel viewModel)
        {
            ModelState.Remove("CityName");
            if (ModelState.IsValid)
            {
                if (viewModel.Id != 0)
                {
                    var province = await _uw.BaseRepository<Province>().FindByIdAsync(viewModel.Id);
                    if (province != null)
                    {
                        province.Name = viewModel.ProvinceName;
                        province.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                        _uw.BaseRepository<Province>().Update(province);
                        await _uw.Commit();
                        TempData["notification"] = EditSuccess;
                    }
                    else
                        ModelState.AddModelError(string.Empty, ProvinceNotFound);
                }
                else
                {
                    var province = new Province();
                    province.Name = viewModel.ProvinceName;
                    var provinces = await _uw.BaseRepository<Province>().FindAllAsync();
                    if (provinces.Count() != 0)
                    {
                        var maxProvince = provinces.OrderByDescending(c => c.Id).First();
                        province.Id = maxProvince.Id + 1;
                    }
                    else
                        province.Id = 1;
                    province.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    await _uw.BaseRepository<Province>().CreateAsync(province);
                    await _uw.Commit();
                    TempData["notification"] = InsertSuccess;
                }
            }
            return PartialView("_RenderProvince", viewModel);
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int provinceId)
        {
            if (provinceId == 0)
                ModelState.AddModelError(string.Empty, ProvinceNotFound);
            else
            {
                var province = await _uw.BaseRepository<Province>().FindByIdAsync(provinceId);
                if (province == null)
                    ModelState.AddModelError(string.Empty, ProvinceNotFound);
                else
                    return PartialView("_DeleteConfirmation", province);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Province model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, ProvinceNotFound);
            else
            {
                var province = await _uw.BaseRepository<Province>().FindByIdAsync(model.Id);
                if (province == null)
                    ModelState.AddModelError(string.Empty, ProvinceNotFound);
                else
                {
                    province.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Province>().Update(province);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", province);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ استانی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var province = await _uw.BaseRepository<Province>().FindByIdAsync(int.Parse(splite));
                    province.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Province>().Update(province);
                }
                await _uw.Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(ProvinceNotFound);
        }
    }
}
