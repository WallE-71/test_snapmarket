﻿using System;
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
    [DisplayName("مدیریت شهر ها")]
    public class CityController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string CityNotFound = "شهر یافت نشد.";
        public CityController(IUnitOfWork uw, IMapper mapper)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index(int cityId)
        {
            return View(nameof(Index), new CityProvinceViewModel { CityId = cityId });
        }

        [HttpGet]
        public async Task<IActionResult> GetCities(string search, string order, int offset, int limit, string sort, int cityId)
        {
            List<CityProvinceViewModel> viewModels;
            int total = _uw.BaseRepository<City>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (order == "asc")
            {
                viewModels = await _uw.Context.Cities.Include(c => c.Province)
                                .Where(c => (cityId == 0 || c.Id.Equals(cityId)) && c.Name.Contains(search))
                                .OrderBy("Name").Skip(offset).Take(limit)
                                .Select(c => new CityProvinceViewModel
                                {
                                    Id = c.Id,
                                    CityName = c.Name,
                                    ProvinceName = c.Province.Name,
                                    numberOfUser = _uw.Context.Users.Where(u => u.CityId == c.Id).Count(),
                                }).ToListAsync();
            }
            else
            {
                viewModels = await _uw.Context.Cities.Include(c => c.Province)
                                   .Where(c => (cityId == 0 || c.Id.Equals(cityId)) && c.Name.Contains(search))
                                   .OrderBy("Name desc").Skip(offset).Take(limit)
                                   .Select(c => new CityProvinceViewModel
                                   {
                                       Id = c.Id,
                                       CityName = c.Name,
                                       ProvinceName = c.Province.Name,
                                       numberOfUser = _uw.Context.Users.Where(u => u.CityId == c.Id).Count(),
                                   }).ToListAsync();
            }

            if (search != "")
                total = viewModels.Count();

            foreach (var item in viewModels)
                item.Row = ++offset;

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderCity(int cityId)
        {
            ViewBag.Provinces = await (from p in _uw.Context.Provinces
                                       where (p.RemoveTime == null)
                                       select new TreeViewProvince { id = p.Id, title = p.Name }).ToListAsync();

            var viewModel = new CityProvinceViewModel();
            if (cityId != 0)
            {
                var city = await _uw.BaseRepository<City>().FindByIdAsync(cityId);
                if (city != null)
                    viewModel.CityName = city.Name;
                else
                    ModelState.AddModelError(string.Empty, CityNotFound);
            }
            return PartialView("_RenderCity", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(CityProvinceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var province = _uw.Context.Provinces.Where(p => p.Name == viewModel.ProvinceName).First();
                if (viewModel.Id != 0)
                {
                    var city = await _uw.BaseRepository<City>().FindByIdAsync(viewModel.Id);
                    if (city != null)
                    {
                        city.ProvinceId = province.Id;
                        city.Name = viewModel.CityName;
                        city.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                        _uw.BaseRepository<City>().Update(city);
                        await _uw.Commit();
                        TempData["notification"] = EditSuccess;
                    }
                    else
                        ModelState.AddModelError(string.Empty, CityNotFound);
                }
                else
                {
                    var city = new City();
                    city.ProvinceId = province.Id;
                    city.Name = viewModel.CityName;
                    var cities = await _uw.BaseRepository<City>().FindAllAsync();
                    if (cities.Count() != 0)
                        city.Id = cities.OrderByDescending(c => c.Id).First().Id + 1;
                    else
                        city.Id = 1;
                    city.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    await _uw.BaseRepository<City>().CreateAsync(city);
                    await _uw.Commit();
                    TempData["notification"] = InsertSuccess;
                }
            }
            return PartialView("_RenderCity", viewModel);
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int cityId)
        {
            if (cityId == 0)
                ModelState.AddModelError(string.Empty, CityNotFound);
            else
            {
                var city = await _uw.BaseRepository<City>().FindByIdAsync(cityId);
                if (city == null)
                    ModelState.AddModelError(string.Empty, CityNotFound);
                else
                    return PartialView("_DeleteConfirmation", city);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(City model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, CityNotFound);
            else
            {
                var city = await _uw.BaseRepository<City>().FindByIdAsync(model.Id);
                if (city == null)
                    ModelState.AddModelError(string.Empty, CityNotFound);
                else
                {
                    city.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<City>().Update(city);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", city);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ شهری برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var city = await _uw.BaseRepository<City>().FindByIdAsync(int.Parse(splite));
                    city.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<City>().Update(city);
                }
                await _uw.Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(CityNotFound);
        }
    }
}
