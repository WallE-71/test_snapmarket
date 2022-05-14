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
using SnapMarket.Common.Extensions;
using SnapMarket.Common.Attributes;
using SnapMarket.ViewModels.Category;
using SnapMarket.ViewModels.DynamicAccess;

namespace SnapMarket.Areas.Admin.Controllers
{
    [DisplayName("مدیریت دسته بندی ها")]
    public class CategoryController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IMemoryCache _cache;
        private const string CategoryDuplicate = "نام دسته تکراری است.";
        private const string CategoryNotFound = "دسته ی درخواستی یافت نشد.";
        public CategoryController(IUnitOfWork uw, IMapper mapper, IMemoryCache cache)
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
        public async Task<IActionResult> GetCategories(string search, string order, int offset, int limit, string sort)
        {
            List<CategoryViewModel> categories;
            int total = _uw.BaseRepository<Category>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "name")
            {
                if (order == "asc")
                    categories = await _uw.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, "CategoryInfo.Name", search);
                else
                    categories = await _uw.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, "CategoryInfo.Name desc", search);
            }
            else if (sort == "parentCategoryName")
            {
                if (order == "asc")
                    categories = await _uw.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, "ParentInfo.Name", search);
                else
                    categories = await _uw.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, "ParentInfo.Name desc", search);
            }
            else
                categories = await _uw.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, "CategoryInfo.Name", search);

            if (search != "")
                total = categories.Count();

            return Json(new { total = total, rows = categories });
        }

        [HttpGet, AjaxOnly, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderCategory(int categoryId)
        {
            var categoryViewModel = new CategoryViewModel();
            ViewBag.Categories = await _uw.CategoryRepository.GetAllCategoriesAsync();
            if (categoryId != 0)
            {
                var category = await _uw.BaseRepository<Category>().FindByIdAsync(categoryId);
                _uw.Context.Entry(category).Reference(c => c.Parent).Load();

                if (category != null)
                    categoryViewModel = _mapper.Map<CategoryViewModel>(category);
                else
                    ModelState.AddModelError(string.Empty, CategoryNotFound);
            }
            return PartialView("_RenderCategory", categoryViewModel);
        }

        [HttpPost, AjaxOnly]
        public async Task<IActionResult> CreateOrUpdate(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_uw.CategoryRepository.IsExistCategory(viewModel.Name, viewModel.Id))
                    ModelState.AddModelError(string.Empty, CategoryDuplicate);
                else
                {
                    int maxId;
                    var categories = await _uw.BaseRepository<Category>().FindAllAsync();
                    if (categories.Count() != 0)
                        maxId = categories.OrderByDescending(c => c.Id).First().Id + 1;
                    else
                        maxId = 1;
                    if (viewModel.ParentName.HasValue())
                    {
                        var parentCategory = _uw.CategoryRepository.FindByCategoryName(viewModel.ParentName);
                        if (parentCategory != null)
                            viewModel.ParentId = parentCategory.Id;
                        else
                        {
                            var parent = new Category()
                            {
                                Id = maxId,
                                Name = viewModel.Name
                            };
                            parent.InsertTime = DateTime.Now;
                            await _uw.BaseRepository<Category>().CreateAsync(parent);
                            viewModel.ParentId = parent.Id;
                        }
                    }

                    if (viewModel.Id != 0)
                    {
                        var category = await _uw.BaseRepository<Category>().FindByIdAsync(viewModel.Id);
                        if (category != null)
                        {
                            category.UpdateTime = DateTime.Now;
                            _uw.BaseRepository<Category>().Update(_mapper.Map(viewModel, category));
                            await _uw.Commit();
                            TempData["notification"] = EditSuccess;
                        }
                        else
                            ModelState.AddModelError(string.Empty, CategoryNotFound);
                    }
                    else
                    {
                        viewModel.Id = maxId;
                        viewModel.InsertTime = DateTime.Now; ;
                        await _uw.BaseRepository<Category>().CreateAsync(_mapper.Map<Category>(viewModel));
                        await _uw.Commit();
                        TempData["notification"] = InsertSuccess;
                    }
                }
            }
            return PartialView("_RenderCategory", viewModel);
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int categoryId)
        {
            if (categoryId == 0 || categoryId == null)
                ModelState.AddModelError(string.Empty, CategoryNotFound);
            else
            {
                var category = await _uw.BaseRepository<Category>().FindByIdAsync(categoryId);
                if (category == null)
                    ModelState.AddModelError(string.Empty, CategoryNotFound);
                else
                    return PartialView("_DeleteConfirmation", category);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Category model)
        {
            if (model.Id == 0 || model.Id == null)
                ModelState.AddModelError(string.Empty, CategoryNotFound);
            else
            {
                var category = await _uw.BaseRepository<Category>().FindByIdAsync(model.Id);
                if (category == null)
                    ModelState.AddModelError(string.Empty, CategoryNotFound);
                else
                {
                    var childCategory = _uw.BaseRepository<Category>().FindByConditionAsync(c => c.ParentId == category.Id).Result.ToList();
                    if (childCategory.Count() != 0)
                    {
                        foreach (var c in childCategory)
                        {
                            c.RemoveTime = DateTime.Now;
                            _uw.BaseRepository<Category>().Update(c);
                            await _uw.Commit();
                        }
                    }
                    category.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Category>().Update(category);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", category);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ دسته بندی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var childCategory = _uw.BaseRepository<Category>().FindByConditionAsync(c => c.ParentId == int.Parse(splite)).Result.ToList();
                    if (childCategory.Count() != 0)
                    {
                        foreach (var c in childCategory)
                        {
                            c.RemoveTime = DateTime.Now;
                            _uw.BaseRepository<Category>().Update(c);
                            await _uw.Commit();
                        }
                    }
                    var category = await _uw.BaseRepository<Category>().FindByIdAsync(int.Parse(splite));
                    category.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Category>().Update(category);
                }
                await _uw.Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(CategoryNotFound);
        }
    }
}
