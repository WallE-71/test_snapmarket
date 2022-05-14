﻿using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SnapMarket.Entities;
using SnapMarket.ViewModels;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.Common.Attributes;
using SnapMarket.ViewModels.DynamicAccess;

namespace SnapMarket.Areas.Admin.Controllers
{
    [DisplayName("مدیریت عناصر")]
    public class MaterialController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string MaterialNotFound = "عنصری یافت نشد.";
        public MaterialController(IUnitOfWork uw, IMapper mapper)
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
        public async Task<IActionResult> GetMaterials(string search, string order, int offset, int limit, string sort)
        {
            List<BaseViewModel<int>> viewModels;
            int total = _uw.BaseRepository<Material>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "نام")
            {
                if (order == "asc")
                    viewModels = await _uw.MaterialRepository.GetPaginateMaterialsAsync(offset, limit, "Name", search);
                else
                    viewModels = await _uw.MaterialRepository.GetPaginateMaterialsAsync(offset, limit, "Name desc", search);
            }
            else
                viewModels = await _uw.MaterialRepository.GetPaginateMaterialsAsync(offset, limit, "Name", search);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderMaterial(int materialId)
        {
            var material = new Material();
            if (materialId != 0)
            {
                var findMaterial = await _uw.BaseRepository<Material>().FindByIdAsync(materialId);
                if (findMaterial != null)
                    material.Name = findMaterial.Name;
                else
                    ModelState.AddModelError(string.Empty, MaterialNotFound);
            }
            return PartialView("_RenderMaterial", material);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(Material model)
        {
            if (ModelState.IsValid)
            {
                var findMaterial = await _uw.BaseRepository<Material>().FindByIdAsync(model.Id);
                if (model.Id != 0)
                {
                    findMaterial.Name = model.Name;
                    findMaterial.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    _uw.BaseRepository<Material>().Update(findMaterial);
                    await _uw.Commit();
                    TempData["notification"] = EditSuccess;
                }
                else
                {
                    var material = new Material();
                    material.Name = model.Name;
                    var materials = await _uw.BaseRepository<Material>().FindAllAsync();
                    if (materials.Count() != 0)
                        material.Id = materials.OrderByDescending(c => c.Id).First().Id + 1;
                    else
                        material.Id = 1;
                    material.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    await _uw.BaseRepository<Material>().CreateAsync(material);
                    await _uw.Commit();
                    TempData["notification"] = InsertSuccess;
                }
            }
            return PartialView("_RenderMaterial", model);
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int materialId)
        {
            if (materialId == 0)
                ModelState.AddModelError(string.Empty, MaterialNotFound);
            else
            {
                var material = await _uw.BaseRepository<Material>().FindByIdAsync(materialId);
                if (material == null)
                    ModelState.AddModelError(string.Empty, MaterialNotFound);
                else
                    return PartialView("_DeleteConfirmation", material);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Material model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, MaterialNotFound);
            else
            {
                var material = await _uw.BaseRepository<Material>().FindByIdAsync(model.Id);
                if (material == null)
                    ModelState.AddModelError(string.Empty, MaterialNotFound);
                else
                {
                    material.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Material>().Update(material);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", material);
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
                    var material = await _uw.BaseRepository<Material>().FindByIdAsync(int.Parse(splite));
                    material.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Material>().Update(material);
                }
                await _uw.Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(MaterialNotFound);
        }
    }
}
