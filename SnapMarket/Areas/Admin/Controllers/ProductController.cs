using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using QRCoder;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.Common.Attributes;
using SnapMarket.ViewModels.Product;
using SnapMarket.ViewModels.DynamicAccess;

namespace SnapMarket.Areas.Admin.Controllers
{
    [DisplayName("مدیریت محصولات")]
    public class ProductController : BaseController
    {
        private readonly IUnitOfWork _uw;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _context;
        private const string ProductNotFound = "محصول درخواستی یافت نشد.";
        public ProductController(IUnitOfWork uw, IMapper mapper, IWebHostEnvironment env, IHttpContextAccessor context)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(string search, string order, int offset, int limit, string sort)
        {
            List<ProductViewModel> viewModels;
            int total = _uw.BaseRepository<Product>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "shortName")
            {
                if (order == "asc")
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Name", search, null, null, 0);
                else
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Name desc", search, null, null, 0);
            }
            else if (sort == "displayPrice")
            {
                if (order == "asc")
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Price", search, null, null, 0);
                else
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Price desc", search, null, null, 0);
            }
            else if (sort == "stock")
            {
                if (order == "asc")
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Stock", search, null, null, 0);
                else
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Stock desc", search, null, null, 0);
            }
            else if (sort == "persianInsertTime")
            {
                if (order == "asc")
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "InsertTime", search, null, null, 0);
                else
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "InsertTime desc", search, null, null, 0);
            }
            else if (sort == "displayDiscount")
            {
                if (order == "asc")
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Discount", search, null, null, 0);
                else
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Discount desc", search, null, null, 0);
            }
            else if (sort == "displayStates")
            {
                if (order == "asc")
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "States", search, null, null, 0);
                else
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "States desc", search, null, null, 0);
            }
            else
                viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "InsertTime desc", search, null, null, 0);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> CreateOrUpdate(string productId)
        {
            var viewModel = new ProductViewModel();
            ViewBag.Brand = _uw.Context.Brands.Select(b => b.Name).ToList();
            ViewBag.Colors = _uw.Context.Colors.Select(t => t.Name).ToList();
            ViewBag.Materials = _uw.Context.Materials.Select(t => t.Name).ToList();
            ViewBag.Guarantee = _uw.Context.Guarantees.Select(g => g.Name).ToList();
            viewModel.ProductCategoriesViewModel = new ProductCategoriesViewModel(await _uw.CategoryRepository.GetAllCategoriesAsync(), null);

            if (productId.HasValue())
            {
                var product = await _uw.ProductRepository.FindProductByIdAsync(productId);
                if (product.Count != 0)
                {
                    viewModel = _mapper.Map<ProductViewModel>(product.FirstOrDefault());

                    //viewModel.ImageFiles = await FindProductImages(product.FirstOrDefault().Id);
                    if (product.FirstOrDefault().IsPrefered)
                        viewModel.IsPrefered = true;

                    if (product.FirstOrDefault().InsertTime != null)
                        viewModel.IsPresent = true;

                    if (product.FirstOrDefault().NameOfColor.HasValue())
                    {
                        var splited = product.FirstOrDefault().NameOfColor.Split(",");
                        foreach (var color in splited)
                            viewModel.PrimaryColor = color;
                    }
                    else
                        viewModel.IsTransparent = true;

                    viewModel.IsEdit = true;
                    viewModel.CheckStatus = true;
                    viewModel.CheckCategories = true;
                    viewModel.NameOfBrand = product.Select(p => p.NameOfBrand).Distinct().ToArray().CombineWith(',');
                    viewModel.NameOfColor = product.Select(p => p.NameOfColor).Distinct().ToArray().CombineWith(',');
                    viewModel.NameOfMaterial = product.Select(p => p.NameOfMaterial).Distinct().ToArray().CombineWith(',');
                    viewModel.PersianExpirationDate = product.FirstOrDefault().ExpirationDate.ConvertMiladiToShamsi("yyyy/MM/dd");
                    viewModel.PersianInsertDate = product.FirstOrDefault().InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd");
                    viewModel.PersianInsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(product.FirstOrDefault().InsertTime.Value).TimeOfDay.ToString();
                    viewModel.ProductCategoriesViewModel = new ProductCategoriesViewModel(await _uw.CategoryRepository.GetAllCategoriesAsync(), product.Select(n => n.IdOfCategories).Distinct().ToArray());
                }
            }
            else
                viewModel.Id = StringExtensions.GenerateId(10);
            viewModel.ImageName = await _uw.FileRepository.FindImageAsync(productId, null, null);
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(ProductViewModel viewModel)
        {
            if (viewModel.IsEdit)
                ModelState.Remove("ImageFile");

            ModelState.Remove("Barcode");
            ModelState.Remove("NameOfCategories");
            viewModel.ProductCategoriesViewModel = new ProductCategoriesViewModel(await _uw.CategoryRepository.GetAllCategoriesAsync(), viewModel.CategoryIds);
            if (ModelState.IsValid)
            {
                if (viewModel.IsTransparent)
                {
                    viewModel.NameOfColor = "";
                    viewModel.PrimaryColor = "";
                    viewModel.ProductColors = null;
                }
                else if (!viewModel.NameOfColor.Split(",").IsValidColorTagsInput())
                {
                    ViewBag.Error = "لطفا برای درج کد رنگ فقط بصورت هگزادسیمال #ffffff استفاده نمایید!";
                    return View(viewModel);
                
                }

                var product = await _uw.BaseRepository<Product>().FindByConditionAsync(p => p.Id == viewModel.Id, null, p => p.ProductCategories, p => p.ProductColors, p => p.ProductMaterials);
                if (viewModel.Id.HasValue() && product.Count() != 0)
                {
                    if (product.FirstOrDefault().Barcode.Count() == 0)
                        viewModel.Barcode = CreateQRCode("نام محصول: " + viewModel.Name + ", قیمت: " + viewModel.Price + " (ریال)" + ", تاریخ انقضا: " + viewModel.PersianExpirationDate);
                    else
                        viewModel.Barcode = product.FirstOrDefault().Barcode;
                    if (viewModel.IsPresent)
                    {
                        if (viewModel.PersianInsertDate.HasValue() && viewModel.PersianInsertTime.HasValue())
                        {
                            var persianTimeArray = viewModel.PersianInsertTime.Split(':');
                            viewModel.InsertTime = viewModel.PersianInsertDate.ConvertShamsiToMiladi().Date + new TimeSpan(int.Parse(persianTimeArray[0]), int.Parse(persianTimeArray[1]), 0);
                        }
                        else
                            viewModel.InsertTime = product.FirstOrDefault().InsertTime;
                    }
                    else
                        viewModel.InsertTime = product.FirstOrDefault().InsertTime;

                    if (viewModel.InsertTime > DateTime.Now)
                    {
                        viewModel.IsComplete = false;
                        viewModel.States = ProductState.CoomingSoon;
                    }
                    else
                        viewModel.IsComplete = true;

                    var lastImage = await _uw.FileRepository.FindImageAsync(viewModel.Id, null, null);
                    if (viewModel.ImageFile.HasValue())
                        await InsertImage(viewModel.ImageFile, viewModel.Id, lastImage);

                    if (viewModel.NameOfColor.HasValue())
                        viewModel.ProductColors = await _uw.ColorRepository.InsertProductColors(viewModel.NameOfColor.Split(","), viewModel.PrimaryColor, product.FirstOrDefault().Id);
                    else if (!viewModel.IsTransparent && !viewModel.NameOfColor.HasValue())
                        viewModel.ProductColors = product.FirstOrDefault().ProductColors;

                    if (viewModel.NameOfMaterial.HasValue())
                        viewModel.ProductMaterials = await _uw.MaterialRepository.InsertProductMaterials(viewModel.NameOfMaterial.Split(','), product.FirstOrDefault().Id);
                    else
                        viewModel.ProductMaterials = product.FirstOrDefault().ProductMaterials;

                    if (viewModel.NameOfBrand.HasValue())
                        viewModel.BrandId = await _uw.BrandRepository.InsertProductBrand(viewModel.NameOfBrand);
                    else
                    {
                        var brands = await _uw.BaseRepository<Brand>().FindByConditionAsync(b => b.Id == product.FirstOrDefault().BrandId);
                        viewModel.BrandId = brands.FirstOrDefault().Id;
                    }

                    if (viewModel.NameOfGuarantee.HasValue())
                        viewModel.GuaranteeId = await _uw.ProductRepository.InsertProductGuarantee(viewModel.NameOfGuarantee);
                    else
                    {
                        var guarantees = await _uw.BaseRepository<Guarantee>().FindByConditionAsync(g => g.Id == product.FirstOrDefault().GuaranteeId);
                        viewModel.GuaranteeId = guarantees.FirstOrDefault().Id;
                    }

                    if (viewModel.CategoryIds == null)
                        viewModel.ProductCategories = product.FirstOrDefault().ProductCategories;
                    else
                        viewModel.ProductCategories = viewModel.CategoryIds.Select(c => new ProductCategory { CategoryId = c, ProductId = product.FirstOrDefault().Id }).ToList();

                    viewModel.UpdateTime = DateTime.Now;
                    viewModel.IsPrefered = product.FirstOrDefault().IsPrefered;
                    viewModel.ExpirationDate = viewModel.PersianExpirationDate.ConvertShamsiToMiladi().Date;
                    _uw.BaseRepository<Product>().Update(_mapper.Map(viewModel, product.FirstOrDefault()));
                    await _uw.Commit();
                    ViewBag.Alert = EditSuccess;
                }
                else
                {
                    viewModel.Barcode = CreateQRCode("نام محصول: " + viewModel.Name + ", قیمت: " + viewModel.Price + " (ریال)" + ", تاریخ انقضا: " + viewModel.PersianExpirationDate);
                    if (viewModel.InsertTime > DateTime.Now)
                    {
                        viewModel.IsComplete = false;
                        viewModel.States = ProductState.CoomingSoon;
                    }
                    else
                        viewModel.IsComplete = true;

                    if (viewModel.IsPresent)
                    {
                        if (!viewModel.PersianInsertDate.HasValue() && !viewModel.PersianInsertTime.HasValue())
                            viewModel.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                        else
                        {
                            var persianTimeArray = viewModel.PersianInsertTime.Split(':');
                            viewModel.InsertTime = viewModel.PersianInsertDate.ConvertShamsiToMiladi().Date + new TimeSpan(int.Parse(persianTimeArray[0]), int.Parse(persianTimeArray[1]), 0);
                        }
                    }
                    else
                        viewModel.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);

                    if (!viewModel.IsTransparent && viewModel.NameOfColor.HasValue())
                        viewModel.ProductColors = await _uw.ColorRepository.InsertProductColors(viewModel.NameOfColor.Split(","), viewModel.PrimaryColor, viewModel.Id);

                    if (viewModel.NameOfMaterial.HasValue())
                        viewModel.ProductMaterials = await _uw.MaterialRepository.InsertProductMaterials(viewModel.NameOfMaterial.Split(","), viewModel.Id);
                    else
                        viewModel.ProductMaterials = null;

                    if (viewModel.NameOfBrand.HasValue())
                        viewModel.BrandId = await _uw.BrandRepository.InsertProductBrand(viewModel.NameOfBrand);
                    else
                        viewModel.BrandId = 0;

                    if (viewModel.NameOfGuarantee.HasValue())
                        viewModel.GuaranteeId = await _uw.ProductRepository.InsertProductGuarantee(viewModel.NameOfGuarantee);
                    else
                        viewModel.GuaranteeId = 0;

                    if (viewModel.CategoryIds.Count() != 0)
                        viewModel.ProductCategories = viewModel.CategoryIds.Select(c => new ProductCategory { CategoryId = c }).ToList();
                    else
                        viewModel.ProductCategories = null;

                    viewModel.ExpirationDate = viewModel.PersianExpirationDate.ConvertShamsiToMiladi().Date;
                    await _uw.BaseRepository<Product>().CreateAsync(_mapper.Map<Product>(viewModel));
                    await _uw.Commit();
                    await InsertImage(viewModel.ImageFile, viewModel.Id, null);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(viewModel);
        }

        [HttpGet, AjaxOnly(), DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(string productId)
        {
            if (!productId.HasValue())
                ModelState.AddModelError(string.Empty, ProductNotFound);
            else
            {
                var product = await _uw.BaseRepository<Product>().FindByIdAsync(productId);
                if (product == null)
                    ModelState.AddModelError(string.Empty, ProductNotFound);
                else
                    return PartialView("_DeleteConfirmation", product);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly(), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Product model)
        {
            if (model.Id == null)
                ModelState.AddModelError(string.Empty, ProductNotFound);
            else
            {
                var product = await _uw.BaseRepository<Product>().FindByIdAsync(model.Id);
                if (product == null)
                    ModelState.AddModelError(string.Empty, ProductNotFound);
                else
                {
                    product.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Product>().Update(product);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", product);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly(), DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ محصولی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var product = await _uw.BaseRepository<Product>().FindByIdAsync(int.Parse(splite));
                    product.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Product>().Update(product);
                }
                await _uw.Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(ProductNotFound);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeProductState(string productId, string productState)
        {
            if (productId.HasValue())
            {
                var product = await _uw.BaseRepository<Product>().FindByIdAsync(productId);
                if (product != null)
                {
                    product.States =
                        productState == "1" || productState == "دمو" ? ProductState.Demo :
                        productState == "2" || productState == "در انبار" ? ProductState.Stockroom :
                        productState == "3" || productState == "آماده عرضه" ? ProductState.Ready :
                        productState == "4" || productState == "موجو نیست" ? ProductState.EndOfStock :
                        productState == "5" || productState == "منقضی شده" ? ProductState.ExpirationEnd :
                        productState == "6" || productState == "مرجوع شده" ? ProductState.Returned :
                        productState == "7" || productState == "آسیب دیده" ? ProductState.Corrupted :
                        ProductState.CoomingSoon;

                    product.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    _uw.BaseRepository<Product>().Update(product);
                    await _uw.Commit();
                    return Json("Success");
                }
            }
            return Json($"محصولی با شناسه '{productId}' یافت نشد !!!");
        }

        [HttpGet, DisplayName("مدیریت تخفیف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderDiscount(string productId)
        {
            var viewModel = new DiscountViewModel();
            viewModel.ProductId = productId;
            var discount = await _uw.Context.Discounts.FirstOrDefaultAsync(d => d.ProductId == productId);
            if (discount != null)
            {
                var product = await _uw.BaseRepository<Product>().FindByIdAsync(productId);
                if (product.Price < 1000)
                    ModelState.AddModelError(string.Empty, "قیمت محصول برای اعمال تخفیف مناسب نیست!");
                if (discount.EndDate < DateTime.Now)
                    ModelState.AddModelError(string.Empty, "مهلت تخفیف به پایان رسیده است!");
                viewModel = _mapper.Map<DiscountViewModel>(discount);
                viewModel.PersianStartDate = discount.StartDate.ToString("yyyy/MM/dd");
                viewModel.PersianEndDate = discount.EndDate.ToString("yyyy/MM/dd");
            }
            return PartialView("_RenderDiscount", viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertOrUpdateDiscount(DiscountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.ProductId.HasValue())
                {
                    var product = await _uw.BaseRepository<Product>().FindByIdAsync(viewModel.ProductId);
                    if (product.Price < 1000)
                    {
                        ModelState.AddModelError(string.Empty, "قیمت محصول برای اعمال تخفیف مناسب نیست!");
                        return PartialView("_RenderDiscount", viewModel);
                    }
                }

                viewModel.EndDate = viewModel.PersianEndDate.ConvertShamsiToMiladi();
                viewModel.StartDate = viewModel.PersianEndDate.ConvertShamsiToMiladi();
                if (viewModel.EndDate < DateTime.Now)
                {
                    ModelState.AddModelError(string.Empty, "تاریخ پایان تخفیف را با دقت انتخاب نمایید!");
                    return PartialView("_RenderDiscount", viewModel);
                }

                var discounts = await _uw.BaseRepository<Discount>().FindByConditionAsync(d => d.ProductId == viewModel.ProductId);
                if (discounts.Count() == 0)
                {
                    var discount = new Discount();
                    if (!viewModel.ProductId.HasValue())
                        viewModel.ProductId = viewModel.DiscountCode;
                    await _uw.BaseRepository<Discount>().CreateAsync(_mapper.Map(viewModel, discount));
                }
                else
                {
                    viewModel.UpdateTime = DateTime.Now;
                    _uw.BaseRepository<Discount>().Update(_mapper.Map(viewModel, discounts.FirstOrDefault()));
                }
                await _uw.Commit();
                TempData["notification"] = OperationSuccess;
            }
            return PartialView("_RenderDiscount", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDiscount(string productId)
        {
            if (!productId.HasValue())
                return Ok("تخفیفی پیدا نشد!");
            else
            {
                var discount = await _uw.BaseRepository<Discount>().FindByIdAsync(productId);
                if (discount == null)
                    return BadRequest("محصول تخفیف ندارد!");
                else
                {
                    if (discount.EndDate < DateTime.Now)
                        return Ok("تخفیف برای حذف معتبر نیست!");
                    discount.EndDate = DateTime.Now;
                    _uw.BaseRepository<Discount>().Update(discount);
                    await _uw.Commit();
                    return Ok("Success");
                }
            }
        }

        [NonAction]
        private byte[] CreateQRCode(string product)
        {
            var _qrCode = new QRCodeGenerator();
            var _qrCodeData = _qrCode.CreateQrCode(product, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(_qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            using (var stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        [HttpGet]
        public async Task<IActionResult> IsPreferedOrNotPrefered(string productId)
        {
            if (productId.HasValue())
            {
                var product = await _uw.BaseRepository<Product>().FindByIdAsync(productId);
                if (product != null)
                {
                    if (product.IsPrefered)
                        product.IsPrefered = false;
                    else
                        product.IsPrefered = true;

                    product.UpdateTime = DateTime.Now;
                    _uw.BaseRepository<Product>().Update(product);
                    await _uw.Commit();
                    return Json("Success");
                }
            }
            return Json($"محصولی با شناسه '{productId}' یافت نشد !!!");
        }

        public async Task<string> InsertImage(string file, string productId, string imageName)
        {
            var checkRootFolder = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(checkRootFolder))
                Directory.CreateDirectory(checkRootFolder);

            var checkAngularFolder = Path.Combine(_env.WebRootPath + "/../../Angular/src/assets/images/", "productImages");
            if (!Directory.Exists(checkAngularFolder))
                Directory.CreateDirectory(checkAngularFolder);

            if (imageName.HasValue())
            {
                FileExtensions.DeleteFile($"{_env.WebRootPath}/images/{imageName}");
                FileExtensions.DeleteFile($"{_env.WebRootPath}/../../Angular/src/assets/images/productImages/{imageName}"); ;
            }
            var image = $"product-{StringExtensions.GenerateId(10)}.jpg";
            file.UploadFileBase64($"{_env.WebRootPath}/images/{image}");
            file.UploadFileBase64($"{_env.WebRootPath}/../../Angular/src/assets/images/productImages/{image}");

            var images = new List<string>();
            images.Add(image);
            await _uw.FileRepository.InsertMultiImageAsync(productId, null, null, images);
            return image;
        }

        public async Task<JsonResult> UploadProductImage(IFormFile file)
        {
            await FileExtensions.UploadFileAsync(file, $"{_env.WebRootPath}/images/{file.FileName}");
            return Json(new { location = $"{_context.HttpContext.Request.Scheme}://{_context.HttpContext.Request.Host}/images/{file.FileName}" });
        }
    }
}
