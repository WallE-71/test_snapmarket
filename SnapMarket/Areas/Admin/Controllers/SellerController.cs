using System;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SnapMarket.Entities;
using SnapMarket.ViewModels;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.ViewModels.Product;
using SnapMarket.ViewModels.DynamicAccess;

namespace SnapMarket.Areas.Admin.Controllers
{
    [DisplayName("مدیریت فروشندگان")]
    public class SellerController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _context;
        private const string ProductNotFound = "محصول یافت نشد.";
        private const string SellerNotFound = "فروشنده یافت نشد.";
        public SellerController(IUnitOfWork uw, IMapper mapper, IWebHostEnvironment env, IHttpContextAccessor context)
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
        public async Task<IActionResult> GetSellers(string search, string order, int offset, int limit, string sort)
        {
            List<SellerViewModel> viewModels;
            int total = _uw.BaseRepository<Seller>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "brand")
            {
                if (order == "asc")
                    viewModels = await _uw.SellerRepository.GetPaginateSellersAsync(offset, limit, "Brand", search);
                else
                    viewModels = await _uw.SellerRepository.GetPaginateSellersAsync(offset, limit, "Brand desc", search);
            }
            else if (sort == "userVote")
            {
                if (order == "asc")
                    viewModels = await _uw.SellerRepository.GetPaginateSellersAsync(offset, limit, "UserVote", search);
                else
                    viewModels = await _uw.SellerRepository.GetPaginateSellersAsync(offset, limit, "UserVote desc", search);
            }
            else if (sort == "persianRegisterDate")
            {
                if (order == "asc")
                    viewModels = await _uw.SellerRepository.GetPaginateSellersAsync(offset, limit, "RegisterDate", search);
                else
                    viewModels = await _uw.SellerRepository.GetPaginateSellersAsync(offset, limit, "RegisterDate desc", search);
            }
            else
                viewModels = await _uw.SellerRepository.GetPaginateSellersAsync(offset, limit, "RegisterDate desc", search);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> CreateOrUpdate(int sellerId)
        {
            var viewModel = new SellerViewModel();
            if (sellerId == 0)
                ModelState.AddModelError(string.Empty, SellerNotFound);
            else
            {
                var seller = await _uw.BaseRepository<Seller>().FindByIdAsync(sellerId);
                if (seller != null)
                {
                    viewModel = _mapper.Map<SellerViewModel>(seller);
                    var store = await _uw.Context.Stores.FirstOrDefaultAsync(s => s.SellerId == seller.Id);
                    if (store != null)
                    {
                        viewModel.Store = store.Name;
                        viewModel.Address = store.Address;
                        viewModel.TelNumber = store.TelNumber;
                        viewModel.PostalCode = store.PostalCode;
                        viewModel.SampleProduct = store.Description;
                        viewModel.PersianEstablishmentDate = store.EstablishmentDate.ConvertMiladiToShamsi("yyyy/MM/dd");
                    }
                    if (seller.InsertTime > DateTime.Now)
                    {
                        viewModel.PersianInsertTime = seller.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd");
                        viewModel.PersianInsertTime = seller.InsertTime.Value.TimeOfDay.ToString();
                    }

                    if (seller.InsertTime > DateTime.Now)
                        viewModel.PersianInsertTime = seller.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd");
                }
                else
                    ModelState.AddModelError(string.Empty, SellerNotFound);
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(SellerViewModel viewModel)
        {
            if (viewModel.Id != 0)
            {
                ModelState.Remove("SellerImageFile");
                ModelState.Remove("NationalIdImageFile");
                ModelState.Remove("DocumentImageFile");
            }
            if (ModelState.IsValid)
            {
                if (viewModel.Id != 0)
                {
                    var lastImages = await _uw.FileRepository.GetImagesAsync(null, viewModel.Id, null);
                    if (viewModel.SellerImageFile != null)
                        await ImageManager(null, viewModel.SellerImageFile, viewModel.Id, lastImages);
                    if (viewModel.NationalIdImageFile != null)
                        await ImageManager(null, viewModel.NationalIdImageFile, viewModel.Id, null);
                    if (viewModel.DocumentImageFile.HasValue())
                        await ImageManager(viewModel.DocumentImageFile, null, viewModel.Id, null);

                    var seller = await _uw.BaseRepository<Seller>().FindByIdAsync(viewModel.Id);
                    if (seller == null)
                        ModelState.AddModelError(string.Empty, SellerNotFound);
                    else
                    {
                        viewModel.UpdateTime = DateTime.Now;
                        _uw.BaseRepository<Seller>().Update(_mapper.Map(viewModel, seller));

                        var store = await _uw.BaseRepository<Store>().FindByConditionAsync(s => s.Name == viewModel.Store && s.PostalCode == viewModel.PostalCode);
                        if (store.Count() != 0)
                        {
                            store.FirstOrDefault().Name = viewModel.Store;
                            store.FirstOrDefault().Address = viewModel.Address;
                            store.FirstOrDefault().TelNumber = viewModel.TelNumber;
                            store.FirstOrDefault().PostalCode = viewModel.PostalCode;
                            store.FirstOrDefault().Description = viewModel.SampleProduct;
                            store.FirstOrDefault().EstablishmentDate = viewModel.PersianEstablishmentDate.ConvertShamsiToMiladi();
                            store.FirstOrDefault().UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                            _uw.BaseRepository<Store>().Update(store.FirstOrDefault());
                        }
                        else if (viewModel.Store.HasValue())
                        {
                            var newStore = new Store
                            {
                                Id = viewModel.Id,
                                Name = viewModel.Store,
                                SellerId = viewModel.Id,
                                Address = viewModel.Address,
                                TelNumber = viewModel.TelNumber,
                                PostalCode = viewModel.PostalCode,
                                Description = viewModel.SampleProduct,
                                EstablishmentDate = viewModel.PersianEstablishmentDate.ConvertShamsiToMiladi(),
                                InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now)
                            };
                            await _uw.BaseRepository<Store>().CreateAsync(newStore);
                        }
                        await _uw.Commit();
                        ViewBag.Alert = EditSuccess;
                    }
                }
                else
                {
                    var maxSeller = new Seller();
                    var sellers = await _uw.BaseRepository<Seller>().FindAllAsync();
                    if (sellers.Count() != 0)
                        maxSeller = sellers.OrderByDescending(c => c.Id).FirstOrDefault();

                    viewModel.InsertTime = DateTime.Now;
                    viewModel.Id = sellers.Count() == 0 ? 1 : maxSeller.Id + 1;
                    await _uw.BaseRepository<Seller>().CreateAsync(_mapper.Map<Seller>(viewModel));
                    await _uw.Commit();

                    if (viewModel.Store.HasValue())
                    {
                        var newStore = new Store
                        {
                            Id = viewModel.Id,
                            Name = viewModel.Store,
                            SellerId = viewModel.Id,
                            Address = viewModel.Address,
                            TelNumber = viewModel.TelNumber,
                            PostalCode = viewModel.PostalCode,
                            Description = viewModel.SampleProduct,
                            EstablishmentDate = viewModel.PersianEstablishmentDate.ConvertShamsiToMiladi(),
                            InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now)
                        };
                        await _uw.BaseRepository<Store>().CreateAsync(newStore);
                        await _uw.Commit();
                    }

                    if (viewModel.SellerImageFile != null)
                        await ImageManager(null, viewModel.SellerImageFile, viewModel.Id, null);
                    if (viewModel.NationalIdImageFile != null)
                        await ImageManager(null, viewModel.NationalIdImageFile, viewModel.Id, null);
                    if (viewModel.DocumentImageFile.HasValue())
                        await ImageManager(viewModel.DocumentImageFile, null, viewModel.Id, null);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(viewModel);
        }

        [HttpPost, ActionName("AddGroup"), DisplayName("افزودن محصولات"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> AddProducts(int sellerId, string productId, string[] sellerProductIds)
        {
            var product = new Product();
            var products = new List<Product>();
            var seller = await _uw.BaseRepository<Seller>().FindByIdAsync(sellerId);
            if (seller == null)
                return Ok(SellerNotFound);

            var splited = new string[sellerProductIds.Length];
            foreach (var item in sellerProductIds)
                splited = item.Split(',');

            if (sellerProductIds[0] != "undefined")
            {
                foreach (var item in splited)
                {
                    product = await _uw.BaseRepository<Product>().FindByIdAsync(item);
                    product.SellerId = sellerId;
                    products.Add(product);
                    _uw.BaseRepository<Product>().UpdateRange(products);
                }
            }
            else if (productId.HasValue())
            {
                product = await _uw.BaseRepository<Product>().FindByIdAsync(productId);
                product.SellerId = sellerId;
                _uw.BaseRepository<Product>().Update(product);
            }
            await _uw.Commit();
            ViewBag.Success = "به فروشگاه اضافه گردید!";
            return Ok("Success");
        }

        [HttpGet, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int sellerId)
        {
            if (sellerId == 0)
                ModelState.AddModelError(string.Empty, SellerNotFound);
            else
            {
                var seller = await _uw.BaseRepository<Seller>().FindByIdAsync(sellerId);
                if (seller == null)
                    ModelState.AddModelError(string.Empty, SellerNotFound);
                else
                    return PartialView("_DeleteConfirmation", seller);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Seller model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, SellerNotFound);
            else
            {
                var seller = await _uw.BaseRepository<Seller>().FindByIdAsync(model.Id);
                if (seller == null)
                    ModelState.AddModelError(string.Empty, SellerNotFound);
                else
                {
                    seller.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Seller>().Update(seller);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", seller);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ فروشنده ای برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var role = await _uw.BaseRepository<Seller>().FindByIdAsync(splite.ToString());
                    role.RemoveTime = DateTime.Now;
                    _uw.BaseRepository<Seller>().Update(role);
                    await _uw.Commit();
                }
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(SellerNotFound);
        }

        [HttpGet]
        public async Task<IActionResult> IsRegisterOrNotRegister(int sellerId)
        {
            if (sellerId != 0)
            {
                var seller = await _uw.BaseRepository<Seller>().FindByIdAsync(sellerId);
                if (seller != null)
                {
                    if (seller.IsComplete)
                    {
                        seller.IsComplete = false;
                        seller.RegisterDate = null;
                    }
                    else
                    {
                        seller.IsComplete = true;
                        seller.RegisterDate = DateTime.Now;
                    }

                    seller.UpdateTime = DateTime.Now;
                    _uw.BaseRepository<Seller>().Update(seller);
                    await _uw.Commit();
                    return Json("Success");
                }
            }
            return Json($"فروشنده ای با شناسه '{sellerId}' یافت نشد !!!");
        }

        public async Task ImageManager(string file, IFormFile imgFile, int sellerId, List<string> lastImages)
        {
            var checkRootFolder = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(checkRootFolder))
                Directory.CreateDirectory(checkRootFolder);

            var checkAngularFolder = Path.Combine(_env.WebRootPath + "/../../Angular/src/assets/images/", "sellerImages");
            if (!Directory.Exists(checkAngularFolder))
                Directory.CreateDirectory(checkAngularFolder);

            if (lastImages != null)
            {
                foreach (var img in lastImages)
                {
                    FileExtensions.DeleteFile($"{_env.WebRootPath}/images/{img}");
                    FileExtensions.DeleteFile($"{_env.WebRootPath}/../../Angular/src/assets/images/sellerImages/{img}");
                }
                var fileStores = await _uw.BaseRepository<FileStore>().FindByConditionAsync(f => f.SellerId == sellerId);
                if (fileStores.Count() != 0)
                    _uw.BaseRepository<FileStore>().DeleteRange(fileStores);
            }

            var image = $"seller-{StringExtensions.GenerateId(10)}.jpg";
            if (imgFile != null)
            {
                await imgFile.UploadFileAsync($"{_env.WebRootPath}/images/{image}");
                await imgFile.UploadFileAsync($"{_env.WebRootPath}/../../Angular/src/assets/images/sellerImages/{image}");
            }
            else if (file.HasValue())
            {
                file.UploadFileBase64($"{_env.WebRootPath}/images/{image}");
                file.UploadFileBase64($"{_env.WebRootPath}/../../Angular/src/assets/images/sellerImages/{image}");
            }
            var images = new List<string>();
            images.Add(image);
            await _uw.FileRepository.InsertMultiImageAsync(null, sellerId, null, images);
        }

        [HttpGet, DisplayName("مشاهده محصولات"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult ProductsOfSeller(int sellerId, bool? isSeller)
        {
            return View(nameof(ProductsOfSeller), new ProductViewModel { SellerId = sellerId, IsSeller = isSeller });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsForSeller(string search, string order, int offset, int limit, string sort, bool? isSeller, long sellerId)
        {
            List<ProductViewModel> viewModels;
            int total = _uw.BaseRepository<Product>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "name")
            {
                if (order == "asc")
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Name", search, null, isSeller, sellerId);
                else
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Name desc", search, null, isSeller, sellerId);
            }
            else if (sort == "price")
            {
                if (order == "asc")
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Price", search, null, isSeller, sellerId);
                else
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Price desc", search, null, isSeller, sellerId);
            }
            else if (sort == "stock")
            {
                if (order == "asc")
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Stock", search, null, isSeller, sellerId);
                else
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "Stock desc", search, null, isSeller, sellerId);
            }
            else if (sort == "persianInsertTime")
            {
                if (order == "asc")
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "InsertTime", search, null, isSeller, sellerId);
                else
                    viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "InsertTime desc", search, null, isSeller, sellerId);
            }
            else
                viewModels = await _uw.ProductRepository.GetPaginateProductAsync(offset, limit, "InsertTime desc", search, null, isSeller, sellerId);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("حذف")]
        public async Task<IActionResult> DeleteOfStore(int sellerId, string productId)
        {
            if (sellerId == 0)
                ModelState.AddModelError(string.Empty, SellerNotFound);
            else
            {
                var product = await _uw.BaseRepository<Product>().FindByIdAsync(productId);
                if (product == null)
                    ModelState.AddModelError(string.Empty, ProductNotFound);
                else
                    return PartialView("_DeleteConfirmation", product);
            }
            return PartialView("_DeleteOfStoreConfirmation");
        }

        [HttpPost, ActionName("DeleteOfStore")]
        public async Task<IActionResult> DeleteOfStoreConfirmed(int sellerId, string productId)
        {
            if (sellerId == 0)
                ModelState.AddModelError(string.Empty, ProductNotFound);
            else
            {
                var product = await _uw.BaseRepository<Product>().FindByIdAsync(productId);
                if (product == null)
                    ModelState.AddModelError(string.Empty, ProductNotFound);
                else
                {
                    product.SellerId = null;
                    product.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    _uw.BaseRepository<Product>().Update(product);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteOfStoreConfirmation", product);
                }
            }
            return PartialView("_DeleteOfStoreConfirmation");
        }

        [HttpPost, ActionName("DeleteGroupProduct")]
        public async Task<IActionResult> DeleteGroupProduct(int sellerId, string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ محصولی ای برای حذف انتخاب نشده است.");
            else
            {
                var ids = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    ids = item.Split(',');

                var seller = await _uw.BaseRepository<Seller>().FindByConditionAsync(s => s.Id == sellerId);
                if (seller != null)
                {
                    foreach (var id in ids)
                    {
                        var product = await _uw.BaseRepository<Product>().FindByIdAsync(id);
                        product.SellerId = null;
                        _uw.BaseRepository<Product>().Update(product);
                        await _uw.Commit();
                    }
                }
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(ProductNotFound);
        }
    }
}
