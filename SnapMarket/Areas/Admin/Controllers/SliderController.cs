using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using AutoMapper;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.Common.Attributes;
using SnapMarket.ViewModels.Slider;
using SnapMarket.ViewModels.DynamicAccess;

namespace SnapMarket.Areas.Admin.Controllers
{
    [DisplayName("مدیریت بنر و اسلایدرها")]
    public class SliderController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IWebHostEnvironment _env;
        private const string SliderNotFound = "اسلایدر درخواستی یافت نشد.";
        public SliderController(IUnitOfWork uw, IMapper mapper, IWebHostEnvironment env)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSliders(string search, string order, int offset, int limit, string sort)
        {
            var viewModels = new List<SliderViewModel>();
            int total = _uw.BaseRepository<Slider>().CountEntities();
            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "imageLocation")
            {
                if (order == "asc")
                    viewModels = await _uw.SliderRepository.GetPaginateSlidersAsync(offset, limit, "ImageLocation", search);
                else
                    viewModels = await _uw.SliderRepository.GetPaginateSlidersAsync(offset, limit, "ImageLocation desc", search);
            }
            else
                viewModels = await _uw.SliderRepository.GetPaginateSlidersAsync(offset, limit, "Id", search);

            if (search != "")
                total = viewModels.Count();

            foreach (var item in viewModels)
            {
                if (item.TypeOfSlider == TypeOfSlider.Static)
                    item.ImageFile = await _uw.FileRepository.FindImageAsync(null, null, item.Id);
                else if (item.TypeOfSlider != 0)
                    item.ImageFiles = await _uw.FileRepository.GetImagesAsync(null, null, item.Id);
            }

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> CreateOrUpdate(int sliderId)
        {
            ViewBag.ImageName = null;
            var viewModel = new SliderViewModel();
            if (sliderId != 0)
            {
                ViewBag.SliderId = sliderId;
                var slider = await _uw.BaseRepository<Slider>().FindByIdAsync(sliderId);
                if (slider == null)
                    return BadRequest(SliderNotFound);

                viewModel = _mapper.Map<SliderViewModel>(slider);

                if (slider.TypeOfSlider == TypeOfSlider.Static)
                    ViewBag.ImageName = await _uw.FileRepository.FindImageAsync(null, null, sliderId);
                else if (slider.TypeOfSlider != 0)
                    ViewBag.Sliders = await _uw.FileRepository.GetImagesAsync(null, null, sliderId);

                viewModel.Title = slider.TypeOfSlider == TypeOfSlider.Static ? slider.Name : null;
                //viewModel.TitleSlider1 = slider.TypeOfSlider == TypeOfSlider.Dynamic ? slider.Name : null;
                viewModel.Description = slider.TypeOfSlider == TypeOfSlider.Static ? slider.Description : null;
                //viewModel.DescriptionSlider1 = slider.TypeOfSlider == TypeOfSlider.Control ? slider.Description : null;
            }
            else
            {
                ViewBag.SliderId = 0;
                var sliders = await _uw.BaseRepository<Slider>().FindAllAsync();
                if (sliders.Count() != 0)
                    viewModel.Id = sliders.OrderByDescending(c => c.Id).First().Id + 1;
                else
                    viewModel.Id = 1;
            }
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(SliderViewModel viewModel)
        {
            ModelState.Remove("Name");
            if (ModelState.IsValid)
            {
                viewModel.Name = null;
                if (viewModel.Description.HasValue() || viewModel.Title.HasValue())
                {
                    viewModel.Name = viewModel.Title;
                    viewModel.TypeOfSlider = TypeOfSlider.Static;
                }
                //else if (viewModel.DescriptionSlider.HasValue())
                //{
                //    viewModel.Name = viewModel.TitleSlider1;
                //    viewModel.TypeOfSlider = TypeOfSlider.Dynamic;
                //    viewModel.Description = viewModel.DescriptionSlider;
                //}
                //else if (viewModel.DescriptionSlider3.HasValue())
                //{
                //    viewModel.TypeOfSlider = TypeOfSlider.Control2;
                //    viewModel.Description = viewModel.DescriptionSlider3;
                //}
                //else
                //{
                //    viewModel.Description = null;
                //    viewModel.TypeOfSlider = TypeOfSlider.Control;
                //}

                var IsSuccess = false;
                var slider = await _uw.BaseRepository<Slider>().FindByIdAsync(viewModel.Id);
                if (viewModel.Id != 0 && slider != null)
                {
                    var lastImage = await _uw.FileRepository.FindImageAsync(null, null, viewModel.Id);
                    if (viewModel.ImageFile.HasValue())
                        await InsertImageAsync(viewModel.ImageFile, viewModel.Id, lastImage);

                    slider.Name = viewModel.Name;
                    slider.Description = viewModel.Description;
                    viewModel.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    _uw.BaseRepository<Slider>().Update(slider);
                    await _uw.Commit();
                    IsSuccess = true;
                    TempData["notification"] = EditSuccess;
                }
                else
                {
                    var list = new List<string>();
                    var images = await HandleImagesUploaded(viewModel.Id);
                    foreach (var img in images)
                    {
                        var splited = img.Split($"slider-{viewModel.Id}-");
                        foreach (var img2 in splited)
                        {
                            var splited2 = img2.Split("-");
                            foreach (var image in splited2)
                            {
                                list.Add(image.Trim().TrimStart().TrimEnd());
                                break;
                            }
                        }
                    }

                    foreach (var item in list)
                    {
                        if (item != "")
                        {
                            if (item == "1")
                                viewModel.TypeOfSlider = TypeOfSlider.Dynamic;
                            else if (item == "2")
                                viewModel.TypeOfSlider = TypeOfSlider.Control1;
                            else if (item == "3")
                                viewModel.TypeOfSlider = TypeOfSlider.Control2;
                            else
                                viewModel.TypeOfSlider = TypeOfSlider.Static;
                        }
                    }
                    viewModel.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    await _uw.BaseRepository<Slider>().CreateAsync(_mapper.Map<Slider>(viewModel));
                    await _uw.Commit();
                    IsSuccess = true;
                    TempData["notification"] = InsertSuccess;
                }
                if (IsSuccess && viewModel.ImageFile.HasValue())
                    await InsertImageAsync(viewModel.ImageFile, viewModel.Id, null);
                if (IsSuccess && !viewModel.ImageFile.HasValue())
                    await HandleImagesUploaded(viewModel.Id, true);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpPost, Route("UploadSlider")]
        public async Task<IActionResult> UploadImages(IEnumerable<IFormFile> files, int sliderId, int idOfSlider)
        {
            var uploadsRootFolder = Path.Combine(_env.WebRootPath, "slider");
            var uploadInAngularFolder = Path.Combine(_env.WebRootPath + "\\..\\..\\Angular\\src\\assets", "slider");
            if (!Directory.Exists(uploadsRootFolder))
                Directory.CreateDirectory(uploadsRootFolder);
            if (!Directory.Exists(uploadInAngularFolder))
                Directory.CreateDirectory(uploadInAngularFolder);

            var nameImages = new List<string>();
            foreach (var item in files)
            {
                if (item != null)
                {
                    var fileExtension = Path.GetExtension(item.FileName);
                    var imageName = String.Concat($"slider-{sliderId}-{idOfSlider}-{StringExtensions.GenerateId(10)}", fileExtension);
                    var path = Path.Combine(uploadsRootFolder, imageName);
                    var pathAngular = Path.Combine(uploadInAngularFolder, imageName);
                    using (var memory = new MemoryStream())
                    {
                        await item.CopyToAsync(memory);
                        using (var Image = new MagickImage(memory.ToArray()))
                        {
                            Image.Resize(Image.Width / 2, Image.Height / 2);
                            Image.Quality = 50;
                            Image.Write(path);
                            Image.Write(pathAngular);
                        }
                    }
                    nameImages.Add(imageName);
                    //CompressImage(path);
                }
            }
            return new JsonResult("success");
        }

        private async Task<List<string>> HandleImagesUploaded(int sliderId, bool save = false)
        {
            var nameImages = new List<string>();
            var uploadsRootFolder = Path.Combine(_env.WebRootPath, "slider");
            if (Directory.Exists(uploadsRootFolder))
            {
                var files = Directory.GetFiles(uploadsRootFolder);
                foreach (var file in files)
                {
                    var info = new FileInfo(file);
                    var name = info.Name;
                    var spliteId = name.Contains($"slider-{sliderId}-");
                    if (spliteId)
                        nameImages.Add(info.Name);
                }
            }
            if (save && nameImages.Count != 0)
                await _uw.FileRepository.InsertMultiImageAsync(null, null, sliderId, nameImages);
            else
                return nameImages;
            return null;
        }

        [HttpGet, AjaxOnly(), DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int sliderId)
        {
            if (sliderId == 0)
                ModelState.AddModelError(string.Empty, SliderNotFound);
            else
            {
                var slider = await _uw.BaseRepository<Slider>().FindByIdAsync(sliderId);
                if (slider == null)
                    ModelState.AddModelError(string.Empty, SliderNotFound);
                else
                    return PartialView("_DeleteConfirmation", slider);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Slider model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, SliderNotFound);
            else
            {
                var slider = await _uw.BaseRepository<Slider>().FindByIdAsync(model.Id);
                if (slider == null)
                    ModelState.AddModelError(string.Empty, SliderNotFound);
                else
                {
                    slider.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Slider>().Update(slider);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", slider);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ فایلی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var slider = await _uw.BaseRepository<Slider>().FindByIdAsync(int.Parse(splite));
                    slider.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Slider>().Update(slider);
                }
                await _uw.Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(SliderNotFound);
        }

        public async Task InsertImageAsync(string file, int sliderId, string imageName)
        {
            var checkRootFolder = Path.Combine(_env.WebRootPath, "slider");
            if (!Directory.Exists(checkRootFolder))
                Directory.CreateDirectory(checkRootFolder);

            var checkAngularFolder = Path.Combine(_env.WebRootPath + "/../../Angular/src/assets/slider/", "slider");
            if (!Directory.Exists(checkAngularFolder))
                Directory.CreateDirectory(checkAngularFolder);

            if (imageName.HasValue())
            {
                FileExtensions.DeleteFile($"{_env.WebRootPath}/slider/{imageName}");
                FileExtensions.DeleteFile($"{_env.WebRootPath}/../../Angular/src/assets/slider/{imageName}");
            }
            var image = $"slider-{StringExtensions.GenerateId(10)}.jpg";
            file.UploadFileBase64($"{_env.WebRootPath}/slider/{image}");
            file.UploadFileBase64($"{_env.WebRootPath}/../../Angular/src/assets/slider/{image}");

            var images = new List<string>();
            images.Add(image);
            await _uw.FileRepository.InsertMultiImageAsync(null, null, sliderId, images);
        }
    }
}
