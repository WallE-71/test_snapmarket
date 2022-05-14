using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SnapMarket.Common;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.ViewModels.Product;
using SnapMarket.ViewModels.Api.Home;

namespace SnapMarket.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SnapMarketDBContext _context;
        public ProductRepository(SnapMarketDBContext context)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));
        }

        public async Task<List<ProductViewModel>> GetPaginateProductAsync(int offset, int limit, string orderBy, string searchText, bool? IsDemo, bool? isSeller, long sellerId = 0)
        {
            var nameOfBrand = "";
            var nameOfColor = "";
            var nameOfSeller = "";
            var nameOfMaterial = "";
            var nameOfGuarantee = "";
            var displayDiscount = "";
            var nameOfCategories = "";
            var viewModels = new List<ProductViewModel>();
            var convertPresent = Convert.ToBoolean(IsDemo);
            var convertSeller = Convert.ToBoolean(isSeller);
            var getDateTimesForSearch = searchText.GetDateTimeForSearch();

            var products = await (from p in ((from p in _context.Products.Include(p => p.Visits).Include(p => p.FileStores).Include(p => p.Comments)
                                              where ((sellerId == 0 || convertSeller == false || p.SellerId == sellerId && convertSeller == true)
                                                  && (sellerId == 0 ? true : convertSeller == false ? p.States == ProductState.Ready && p.SellerId == null : p.SellerId == sellerId && convertSeller == true)
                                                  //&& (sellerId == 0 ? true : convertSeller == false ? p.States == ProductState.Ready : p.SellerId == sellerId && convertSeller == true)
                                                  || p.RemoveTime == null && p.Name.Contains(searchText) && (p.InsertTime >= getDateTimesForSearch.First() && p.InsertTime <= getDateTimesForSearch.Last()))
                                                  && (IsDemo == null || (convertPresent ? p.IsComplete && p.InsertTime <= DateTime.Now : !p.IsComplete))
                                              select (new
                                              {
                                                  p.Id,
                                                  p.Name,
                                                  p.Size,
                                                  p.Price,
                                                  p.Stock,
                                                  p.MadeIn,
                                                  p.States,
                                                  p.Weight,
                                                  p.Barcode,
                                                  p.BrandId,
                                                  p.SellerId,
                                                  p.InsertTime,
                                                  p.IsPrefered,
                                                  p.GuaranteeId,
                                                  p.Description,
                                                  p.NumberOfSale,
                                                  p.ExpirationDate,
                                                  NumberOfComments = p.Comments.Count(),
                                                  NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum(),
                                                  ShortName = p.Name.Length > 20 ? p.Name.Substring(0, 20) + "..." : p.Name,
                                                  //ProductState = p.Stock == 0 ? ProductState.EndOfStock : p.InsertTime <= DateTime.Now ? ProductState.Demo : p.InsertTime > DateTime.Now ? ProductState.CoomingSoon : p.InsertTime < p.ExpirationDate ? ProductState.ExpirationEnd : ProductState.Stockroom,
                                              })).Skip(offset).Take(limit))
                                  join a in _context.ProductCategories on p.Id equals a.ProductId into aa
                                  from productCategory in aa.DefaultIfEmpty()
                                  join c in _context.Categories on productCategory.CategoryId equals c.Id into bb
                                  from category in bb.DefaultIfEmpty()

                                  join pm in _context.ProductMaterials on p.Id equals pm.ProductId into cc
                                  from productMaterial in cc.DefaultIfEmpty()
                                  join m in _context.Materials on productMaterial.MaterialId equals m.Id into dd
                                  from material in dd.DefaultIfEmpty()

                                  join pc in _context.ProductColors on p.Id equals pc.ProductId into ee
                                  from productColor in ee.DefaultIfEmpty()
                                  join co in _context.Colors on productColor.ColorId equals co.Id into ff
                                  from color in ff.DefaultIfEmpty()

                                  join s in _context.Sellers on p.SellerId equals s.Id into gg
                                  from seller in gg.DefaultIfEmpty()

                                  join g in _context.Guarantees on p.GuaranteeId equals g.Id into hh
                                  from guarantee in hh.DefaultIfEmpty()

                                  join b in _context.Brands on p.BrandId equals b.Id into ii
                                  from brand in ii.DefaultIfEmpty()

                                  join d in _context.Discounts on p.Id equals d.ProductId into jj
                                  from discount in jj.DefaultIfEmpty()
                                  select (new ProductViewModel
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Size = p.Size,
                                      Price = p.Price,
                                      Stock = p.Stock,
                                      Weight = p.Weight,
                                      MadeIn = p.MadeIn,
                                      Barcode = p.Barcode,
                                      SellerId = p.SellerId,
                                      ShortName = p.ShortName,
                                      InsertTime = p.InsertTime,
                                      IsPrefered = p.IsPrefered,
                                      Description = p.Description,
                                      NumberOfSale = p.NumberOfSale,
                                      NumberOfVisit = p.NumberOfVisit,
                                      ExpirationDate = p.ExpirationDate,
                                      PercentDiscount = discount.Percent,
                                      NumberOfComments = p.NumberOfComments,
                                      DisplayPrice = p.Price.ToString().En2Fa(),
                                      NameOfBrand = brand != null ? brand.Name : "",
                                      NameOfColor = color != null ? color.Name : "",
                                      NameOfSeller = seller != null ? seller.Name : "",
                                      NameOfMaterial = material != null ? material.Name : "",
                                      NameOfCategories = category != null ? category.Name : "",
                                      NameOfGuarantee = guarantee != null ? guarantee.Name : "",
                                      PersianInsertTime = p.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd"),
                                      PersianExpirationDate = p.ExpirationDate.ConvertMiladiToShamsi("yyyy/MM/dd"),
                                      DisplayDiscount =
                                        discount.DiscountCode != null ? "کد تخفیف" :
                                        discount.EndDate.Date <= DateTime.Now.Date ? "ندارد" :
                                        discount.EndDate.Date <= DateTime.Now.Date.AddDays(7) && discount.EndDate.Date > DateTime.Now.Date.AddDays(1) ? "هفتگی" :
                                        discount.EndDate.Date <= DateTime.Now.Date.AddMonths(3) && discount.EndDate.Date > DateTime.Now.Date.AddDays(7) ? "فصلی" :
                                        discount.EndDate.Date <= DateTime.Now.Date.AddDays(1) && discount.EndDate.Date > DateTime.Now.AddHours(6) ? "ویژه" : "ندارد",
                                      //discount.EndDate <= DateTime.Now.AddHours(6) ? "فوری",
                                      DisplayStates =
                                        p.States == ProductState.Demo ? "دمو" :
                                        p.States == ProductState.Stockroom ? "در انبار" :
                                        p.States == ProductState.Ready ? "آماده عرضه" :
                                        p.States == ProductState.EndOfStock ? "موجود نیست" :
                                        p.States == ProductState.ExpirationEnd ? "منقضی شده" :
                                        p.States == ProductState.Returned ? "مرجوع شده" :
                                        p.States == ProductState.Corrupted ? "آسیب دیده" : "بزودی",
                                  })).OrderBy(orderBy).AsNoTracking().ToListAsync();

            var productGroup = products.GroupBy(g => g.Id).Select(g => new { ProductId = g.Key, ProductGroup = g });
            foreach (var item in productGroup)
            {
                nameOfCategories = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfCategories).Distinct())
                {
                    if (nameOfCategories == "")
                        nameOfCategories = a;
                    else
                        nameOfCategories = nameOfCategories + " - " + a;
                }

                nameOfColor = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfColor).Distinct())
                {
                    if (nameOfColor == "")
                        nameOfColor = a;
                    else
                        nameOfColor = nameOfColor + " , " + a;
                }

                nameOfMaterial = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfMaterial).Distinct())
                {
                    if (nameOfMaterial == "")
                        nameOfMaterial = a;
                    else
                        nameOfMaterial = nameOfMaterial + " , " + a;
                }

                nameOfSeller = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfSeller).Distinct())
                {
                    if (nameOfSeller == "")
                        nameOfSeller = a;
                    else
                        nameOfSeller = nameOfSeller + " - " + a;
                }

                nameOfBrand = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfBrand).Distinct())
                {
                    if (nameOfBrand == "")
                        nameOfBrand = a;
                    else
                        nameOfBrand = nameOfBrand + " - " + a;
                }

                nameOfGuarantee = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfGuarantee).Distinct())
                {
                    if (nameOfGuarantee == "")
                        nameOfGuarantee = a;
                    else
                        nameOfGuarantee = nameOfGuarantee + " - " + a;
                }

                displayDiscount = "";
                foreach (var a in item.ProductGroup.Select(a => a.DisplayDiscount).Distinct())
                {
                    if (displayDiscount == "")
                        displayDiscount = a;
                    else
                        displayDiscount = displayDiscount + " - " + a;
                }

                var product = new ProductViewModel()
                {
                    Id = item.ProductId,
                    NameOfBrand = nameOfBrand,
                    NameOfColor = nameOfColor,
                    NameOfSeller = nameOfSeller,
                    NameOfMaterial = nameOfMaterial,
                    NameOfGuarantee = nameOfGuarantee,
                    DisplayDiscount = displayDiscount,
                    NameOfCategories = nameOfCategories,
                    Name = item.ProductGroup.First().Name,
                    Size = item.ProductGroup.First().Size,
                    Stock = item.ProductGroup.First().Stock,
                    MadeIn = item.ProductGroup.First().MadeIn,
                    Weight = item.ProductGroup.First().Weight,
                    Barcode = item.ProductGroup.First().Barcode,
                    SellerId = item.ProductGroup.First().SellerId,
                    ShortName = item.ProductGroup.First().ShortName,
                    ImageName = item.ProductGroup.First().ImageName,
                    Description = item.ProductGroup.First().Description,
                    DisplayPrice = item.ProductGroup.First().DisplayPrice,
                    NumberOfSale = item.ProductGroup.First().NumberOfSale,
                    DisplayStates = item.ProductGroup.First().DisplayStates,
                    NumberOfVisit = item.ProductGroup.First().NumberOfVisit,
                    PercentDiscount = item.ProductGroup.First().PercentDiscount,
                    NumberOfComments = item.ProductGroup.First().NumberOfComments,
                    PersianInsertTime = item.ProductGroup.First().InsertTime == null ? "-" : item.ProductGroup.First().InsertTime > DateTime.Now ? DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().InsertTime, "yyyy/MM/dd") + " " + "(در دست تولید)" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().InsertTime, "yyyy/MM/dd"),
                    PersianExpirationDate = item.ProductGroup.First().ExpirationDate == null ? "-" : item.ProductGroup.First().ExpirationDate < DateTime.Now ? DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().ExpirationDate, "yyyy/MM/dd") + " " + "(منقضی شده)" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().ExpirationDate, "yyyy/MM/dd")
                };
                viewModels.Add(product);
            }

            foreach (var viewModel in viewModels)
            {
                viewModel.Row = ++offset;
                await ChangeOfProductStateAsync(viewModel.Id);
                var discount = await DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await GetProductImage(viewModel.Id);

                viewModel.ColorsName = new List<string>();
                var splited = viewModel.NameOfColor.Split(" , ");
                foreach (var color in splited)
                    viewModel.PrimaryColor = color;

                //viewModel.ColorsName = new List<string>();
                //var splited = viewModel.NameOfColor.Split(" , ");
                //foreach (var color in splited)
                //{
                //    var primary = color.Replace("#", "");
                //    viewModel.PrimaryColor = System.Drawing.ColorTranslator.FromHtml("#FF" + primary).Name;
                //    var _color = color.Replace("#", "");
                //    viewModel.ColorsName.Add(System.Drawing.ColorTranslator.FromHtml("#FF" + _color).Name);
                //}
            }
            return viewModels;
        }

        public async Task<List<ProductViewModel>> FindProductByIdAsync(string productId)
        {
            var nameOfBrand = "";
            var nameOfColor = "";
            var nameOfMaterial = "";
            var nameOfGuarantee = "";
            var nameOfCategories = "";
            var viewModels = new List<ProductViewModel>();

            var products = await (from p in ((from p in _context.Products
                                              where (p.Id == productId)
                                              select (new
                                              {
                                                  p.Id,
                                                  p.Name,
                                                  p.Size,
                                                  p.Stock,
                                                  p.Price,
                                                  p.Weight,
                                                  p.MadeIn,
                                                  p.States,
                                                  p.BrandId,
                                                  p.InsertTime,
                                                  p.Description,
                                                  p.GuaranteeId,
                                                  p.ExpirationDate,
                                              })).AsNoTracking())
                                  join c in _context.ProductCategories on p.Id equals c.ProductId into aa
                                  from productCategory in aa.DefaultIfEmpty()

                                  join pm in _context.ProductMaterials on p.Id equals pm.ProductId into bb
                                  from productMaterial in bb.DefaultIfEmpty()
                                  join m in _context.Materials on productMaterial.MaterialId equals m.Id into cc
                                  from material in cc.DefaultIfEmpty()

                                  join pc in _context.ProductColors on p.Id equals pc.ProductId into dd
                                  from productColor in dd.DefaultIfEmpty()
                                  join co in _context.Colors on productColor.ColorId equals co.Id into ee
                                  from color in ee.DefaultIfEmpty()

                                  join g in _context.Guarantees on p.GuaranteeId equals g.Id into ff
                                  from guarantee in ff.DefaultIfEmpty()

                                  join b in _context.Brands on p.BrandId equals b.Id into gg
                                  from brand in gg.DefaultIfEmpty()
                                  select (new ProductViewModel
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Size = p.Size,
                                      Stock = p.Stock,
                                      Price = p.Price,
                                      Weight = p.Weight,
                                      MadeIn = p.MadeIn,
                                      States = p.States,
                                      InsertTime = p.InsertTime,
                                      Description = p.Description,
                                      ExpirationDate = p.ExpirationDate,
                                      NameOfBrand = brand != null ? brand.Name : "",
                                      NameOfColor = color != null ? color.Name : "",
                                      NameOfMaterial = material != null ? material.Name : "",
                                      NameOfGuarantee = guarantee != null ? guarantee.Name : "",
                                      IdOfCategories = productCategory != null ? productCategory.CategoryId : 0,
                                      NameOfCategories = productCategory != null ? productCategory.Category.Name : "",
                                  })).AsNoTracking().ToListAsync();

            var productGroup = products.GroupBy(g => g.Id).Select(g => new { ProductId = g.Key, ProductGroup = g });
            foreach (var item in productGroup)
            {
                nameOfColor = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfColor).Distinct())
                {
                    if (nameOfColor == "")
                        nameOfColor = a;
                    else
                        nameOfColor = nameOfColor + "," + a;
                }

                nameOfMaterial = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfMaterial).Distinct())
                {
                    if (nameOfMaterial == "")
                        nameOfMaterial = a;
                    else
                        nameOfMaterial = nameOfMaterial + "," + a;
                }

                nameOfBrand = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfBrand).Distinct())
                {
                    if (nameOfBrand == "")
                        nameOfBrand = a;
                    else
                        nameOfBrand = nameOfBrand + " - " + a;
                }

                nameOfGuarantee = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfGuarantee).Distinct())
                {
                    if (nameOfGuarantee == "")
                        nameOfGuarantee = a;
                    else
                        nameOfGuarantee = nameOfGuarantee + " - " + a;
                }

                nameOfCategories = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfCategories).Distinct())
                {
                    if (nameOfCategories == "")
                        nameOfCategories = a;
                    else
                        nameOfCategories = nameOfCategories + " - " + a;
                }

                var product = new ProductViewModel()
                {
                    Id = item.ProductId,
                    NameOfBrand = nameOfBrand,
                    NameOfColor = nameOfColor,
                    NameOfMaterial = nameOfMaterial,
                    NameOfGuarantee = nameOfGuarantee,
                    NameOfCategories = nameOfCategories,
                    Name = item.ProductGroup.First().Name,
                    Size = item.ProductGroup.First().Size,
                    Stock = item.ProductGroup.First().Stock,
                    Price = item.ProductGroup.First().Price,
                    Weight = item.ProductGroup.First().Weight,
                    MadeIn = item.ProductGroup.First().MadeIn,
                    States = item.ProductGroup.First().States,
                    ImageName = item.ProductGroup.First().ImageName,
                    InsertTime = item.ProductGroup.First().InsertTime,
                    Description = item.ProductGroup.First().Description,
                    ExpirationDate = item.ProductGroup.First().ExpirationDate,
                    IdOfCategories = item.ProductGroup.First().IdOfCategories,
                    PercentDiscount = item.ProductGroup.First().PercentDiscount,
                    PersianInsertTime = item.ProductGroup.First().InsertTime == null ? "-" : item.ProductGroup.First().InsertTime > DateTime.Now ? DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().InsertTime, "yyyy/MM/dd") + " " + "(در دست تولید)" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().InsertTime, "yyyy/MM/dd"),
                    PersianExpirationDate = item.ProductGroup.First().ExpirationDate == null ? "-" : item.ProductGroup.First().ExpirationDate < DateTime.Now ? DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().ExpirationDate, "yyyy/MM/dd") + " " + "(منقضی شده)" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().ExpirationDate, "yyyy/MM/dd")
                };
                viewModels.Add(product);
            }
            foreach (var viewModel in viewModels)
            {
                await ChangeOfProductStateAsync(viewModel.Id);
                var discount = await DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await GetProductImage(viewModel.Id);
            }
            return viewModels;
        }

        public async Task<List<ProductInCategoriesDto>> GetProductInCategoryAsync(string categoryName)
        {
            string NameOfCategories = "";
            var productViewModel = new List<ProductInCategoriesDto>();

            var allProduct = await (from p in ((from p in _context.Products.Include(c => c.ProductCategories)
                                                where (p.RemoveTime == null && p.Stock != 0 && p.States == ProductState.Ready && p.IsPrefered == false && p.States != ProductState.CoomingSoon
                                                    && p.ProductCategories.Select(c => c.Category.Name).Contains(categoryName))
                                                select (new
                                                {
                                                    p.Id,
                                                    p.Name,
                                                    p.Price,
                                                    p.States,
                                                    p.BrandId,
                                                    p.InsertTime,
                                                    p.Description,
                                                })).AsNoTracking())
                                    join e in _context.ProductCategories on p.Id equals e.ProductId into bc
                                    from bct in bc.DefaultIfEmpty()
                                    join c in _context.Categories on bct.CategoryId equals c.Id into cg
                                    from category in cg.DefaultIfEmpty()

                                    join b in _context.Brands on p.BrandId equals b.Id into pb
                                    from brand in pb.DefaultIfEmpty()

                                    join d in _context.Discounts on p.Id equals d.ProductId into jj
                                    from discount in jj.DefaultIfEmpty()
                                    select (new ProductInCategoriesDto
                                    {
                                        Id = p.Id,
                                        Name = p.Name,
                                        Price = p.Price,
                                        InsertTime = p.InsertTime,
                                        Description = p.Description,
                                        PercentDiscount = discount.Percent,
                                        NameOfBrand = brand != null ? brand.Name : "",
                                        NameOfCategories = category != null ? category.Name : "",
                                    })).AsNoTracking().ToListAsync();

            var productGroup = allProduct.GroupBy(g => g.Id).Select(g => new { ProductId = g.Key, ProductGroup = g });
            foreach (var item in productGroup)
            {
                NameOfCategories = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfCategories).Distinct())
                {
                    if (NameOfCategories == "")
                        NameOfCategories = a;
                    else
                        NameOfCategories = NameOfCategories + " - " + a;
                }

                var product = new ProductInCategoriesDto()
                {
                    Id = item.ProductId,
                    Name = item.ProductGroup.First().Name,
                    Price = item.ProductGroup.First().Price,
                    Status = item.ProductGroup.First().Status,
                    Seller = item.ProductGroup.First().Seller,
                    ImageName = item.ProductGroup.First().ImageName,
                    InsertTime = item.ProductGroup.First().InsertTime,
                    Description = item.ProductGroup.First().Description,
                    NameOfBrand = item.ProductGroup.First().NameOfBrand,
                    NameOfCategories = item.ProductGroup.First().NameOfCategories,
                    PersianInsertTime = item.ProductGroup.First().InsertTime == null ? "-" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().InsertTime, "yyyy/MM/dd"),
                    PersianExpirationDate = item.ProductGroup.First().ExpirationDate == null ? "-" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().ExpirationDate, "yyyy/MM/dd"),
                };
                productViewModel.Add(product);
            }
            foreach (var viewModel in productViewModel)
            {
                await ChangeOfProductStateAsync(viewModel.Id);
                var discount = await DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await GetProductImage(viewModel.Id);
            }
            return productViewModel;
        }

        public async Task<ProductViewModel> GetProductByIdAsync(string productId, int userId)
        {
            var NameOfCategories = "";
            var productInfo = await (from p in _context.Products.Where(n => n.Id == productId).Include(v => v.Bookmarks).Include(v => v.Visits).Include(l => l.Comments).ThenInclude(u => u.Likes)
                                     where (p.SellerId != null && p.RemoveTime == null && p.States == ProductState.Ready)
                                     join e in _context.ProductCategories on p.Id equals e.ProductId into bc
                                     from bct in bc.DefaultIfEmpty()
                                     join c in _context.Categories on bct.CategoryId equals c.Id into cg
                                     from category in cg.DefaultIfEmpty()

                                     join b in _context.Brands on p.BrandId equals b.Id into pb
                                     from brand in pb.DefaultIfEmpty()

                                     join d in _context.Discounts on p.Id equals d.ProductId into jj
                                     from discount in jj.DefaultIfEmpty()
                                     select (new ProductViewModel
                                     {
                                         Id = p.Id,
                                         Name = p.Name,
                                         Price = p.Price,
                                         IsComplete = p.IsComplete,
                                         Description = p.Description,
                                         PercentDiscount = discount.Percent,
                                         NameOfBrand = brand != null ? brand.Name : "",
                                         NameOfCategories = category != null ? category.Name : "",
                                         NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum(),
                                         NumberOfComments = p.Comments.Where(c => c.IsComplete == true).Count(),
                                         InsertTime = p.InsertTime == null ? new DateTime(01, 01, 01) : p.InsertTime,
                                         PersianExpirationDate = p.ExpirationDate.ConvertMiladiToShamsi("yyyy/MM/dd"),
                                         NumberOfLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == true)).Count(),
                                         ExpirationDate = p.ExpirationDate == null ? new DateTime(01, 01, 01) : p.ExpirationDate,
                                         NumberOfDisLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == false)).Count(),
                                         IsBookmarked = p.Bookmarks.Any(b => b.UserId == userId && b.ProductId == productId && b.RemoveTime == null),
                                         PersianInsertTime = p.InsertTime == null ? "-" : p.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd"),
                                         DisplayDiscount =
                                            discount.DiscountCode != null ? "کد تخفیف" :
                                            discount.EndDate.Date <= DateTime.Now.Date ? "ندارد" :
                                            discount.EndDate.Date <= DateTime.Now.Date.AddDays(7) && discount.EndDate.Date > DateTime.Now.Date.AddDays(1) ? "هفتگی" :
                                            discount.EndDate.Date <= DateTime.Now.Date.AddMonths(3) && discount.EndDate.Date > DateTime.Now.Date.AddDays(7) ? "فصلی" :
                                            discount.EndDate.Date <= DateTime.Now.Date.AddDays(1) && discount.EndDate.Date > DateTime.Now.AddHours(6) ? "ویژه" : "ندارد",
                                            //discount.EndDate <= DateTime.Now.AddHours(6) ? "فوری",
                                     })).AsNoTracking().FirstOrDefaultAsync();

            if (productInfo != null)
            {
                if (NameOfCategories == "")
                    NameOfCategories = productInfo.NameOfCategories;
                else
                    NameOfCategories = NameOfCategories + " - " + productInfo.NameOfCategories;

                productInfo.NameOfCategories = NameOfCategories;

                if (productInfo.Description.HasValue())
                {
                    var description = productInfo.Description.Replace("<p>", "").Replace("</p>", "");
                    productInfo.Description = description;
                    productInfo.ShortName = description.Length >= 100 ? description.Substring(0, 100) + "..." : description;
                }
                await ChangeOfProductStateAsync(productInfo.Id);
                var discount = await DiscountManagerAsync(productInfo.Id);
                productInfo.ActionDiscount = discount.ToString().En2Fa();
                productInfo.ImageName = await GetProductImage(productInfo.Id);
                productInfo.ImageFiles = await GetProductImagesAsync(productInfo.Id, 0, 5);
            }
            return productInfo;
        }
       
        public async Task<List<ProductViewModel>> MostViewedProductAsync(int offset, int limit, string duration)
        {
            DateTime StartMiladiDate;
            string NameOfCategories = "";
            var EndMiladiDate = DateTime.Now;
            var viewModels = new List<ProductViewModel>();

            if (duration == "week")
            {
                int NumOfWeek = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dddd").GetNumOfWeek();
                StartMiladiDate = DateTime.Now.AddDays((-1) * NumOfWeek).Date + new TimeSpan(0, 0, 0);
            }
            else if (duration == "day")
                StartMiladiDate = DateTime.Now.Date + new TimeSpan(0, 0, 0);
            else
            {
                string DayOfMonth = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dd").Fa2En();
                StartMiladiDate = DateTime.Now.AddDays((-1) * (int.Parse(DayOfMonth) - 1)).Date + new TimeSpan(0, 0, 0);
            }

            var allProducts = await (from p in ((from p in _context.Products.Include(p => p.Visits).Include(p => p.Comments).ThenInclude(c => c.Likes)
                                                 where (p.States == ProductState.Ready && p.InsertTime <= EndMiladiDate && StartMiladiDate <= p.InsertTime)
                                                 select (new
                                                 {
                                                     p.Id,
                                                     p.Price,
                                                     NumberOfComments = p.Comments.Count(),
                                                     NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum(),
                                                     NumberOfLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == true)).Count(),
                                                     NumberOfDisLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == false)).Count(),
                                                     CreateDateTime = p.InsertTime == null ? new DateTime(01, 01, 01) : p.InsertTime,
                                                     ProductName = p.Name.Length > 60 ? p.Name.Substring(0, 60) + "..." : p.Name,
                                                 })).OrderBy("NumberOfVisit desc").Skip(offset).Take(limit))
                                     join e in _context.ProductCategories on p.Id equals e.ProductId into bc
                                     from bct in bc.DefaultIfEmpty()
                                     join c in _context.Categories on bct.CategoryId equals c.Id into cg
                                     from cog in cg.DefaultIfEmpty()
                                     select (new
                                     {
                                         p.Id,
                                         p.Price,
                                         p.ProductName,
                                         p.NumberOfLike,
                                         p.NumberOfVisit,
                                         p.CreateDateTime,
                                         p.NumberOfDisLike,
                                         p.NumberOfComments,
                                         Name = cog != null ? cog.Name : "",
                                     })).AsNoTracking().ToListAsync();

            var productsGroup = allProducts.GroupBy(g => g.Id).Select(g => new { ProductId = g.Key, ProductGroup = g });
            foreach (var item in productsGroup)
            {
                NameOfCategories = "";
                foreach (var a in item.ProductGroup.Select(a => a.Name).Distinct())
                {
                    if (NameOfCategories == "")
                        NameOfCategories = a;
                    else
                        NameOfCategories = NameOfCategories + " - " + a;
                }

                var viewModel = new ProductViewModel()
                {
                    Id = item.ProductId,
                    Name = item.ProductGroup.First().ProductName,
                    InsertTime = item.ProductGroup.First().CreateDateTime,
                    NumberOfLike = item.ProductGroup.First().NumberOfLike,
                    NumberOfVisit = item.ProductGroup.First().NumberOfVisit,
                    NumberOfDisLike = item.ProductGroup.First().NumberOfDisLike,
                    NameOfCategories = NameOfCategories,
                };
                viewModels.Add(viewModel);
            }
            foreach (var viewModel in viewModels)
            {
                await ChangeOfProductStateAsync(viewModel.Id);
                var discount = await DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await GetProductImage(viewModel.Id);
            }
            return viewModels;
        }

        public async Task<List<ProductViewModel>> MostTalkProduct(int offset, int limit, string duration)
        {
            DateTime StartMiladiDate;
            var EndMiladiDate = DateTime.Now;

            if (duration == "week")
            {
                int NumOfWeek = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dddd").GetNumOfWeek();
                StartMiladiDate = DateTime.Now.AddDays((-1) * NumOfWeek).Date + new TimeSpan(0, 0, 0);
            }
            else if (duration == "day")
                StartMiladiDate = DateTime.Now.Date + new TimeSpan(0, 0, 0);
            else
            {
                string DayOfMonth = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dd").Fa2En();
                StartMiladiDate = DateTime.Now.AddDays((-1) * (int.Parse(DayOfMonth) - 1)).Date + new TimeSpan(0, 0, 0);
            }

            var products = await (from p in _context.Products.Include(v => v.Visits).Include(l => l.Comments).ThenInclude(c => c.Likes)
                                  where (p.States == ProductState.Ready && p.InsertTime <= EndMiladiDate && StartMiladiDate <= p.InsertTime)
                                  select (new ProductViewModel
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      NumberOfComments = p.Comments.Count(),
                                      NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum(),
                                      NumberOfLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == true)).Count(),
                                      NumberOfDisLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == false)).Count(),
                                      InsertTime = p.InsertTime == null ? new DateTime(01, 01, 01) : p.InsertTime,
                                  })).OrderByDescending(o => o.NumberOfComments).Skip(offset).Take(limit).AsNoTracking().ToListAsync();

            foreach (var product in products)
            {
                await ChangeOfProductStateAsync(product.Id);
                var discount = await DiscountManagerAsync(product.Id);
                product.ActionDiscount = discount.ToString().En2Fa();
                product.ImageName = await GetProductImage(product.Id);
            }
            return products;
        }

        public async Task<List<ProductViewModel>> MostPopularProducts(int offset, int limit)
        {
            var countProduct = CountProducts();
            var average = _context.Products.Where(p => p.RemoveTime == null && p.NumberOfSale > countProduct && p.IsComplete != false).ToList();
            var products = await (from p in _context.Products.Include(p => p.Visits).Include(p => p.Comments).ThenInclude(c => c.Likes)
                                  where (p.States == ProductState.Ready/* && Convert.ToDouble(p.NumberOfSale) > average.Max().NumberOfSale*/)
                                  select (new ProductViewModel
                                  {
                                      Id = p.Id,
                                      Stock = p.Stock,
                                      Price = p.Price,
                                      NumberOfComments = p.Comments.Count(),
                                      NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum(),
                                      NumberOfLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == true)).Count(),
                                      NumberOfDisLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == false)).Count(),
                                      InsertTime = p.InsertTime == null ? new DateTime(01, 01, 01) : p.InsertTime,
                                      Name = p.Name.Length > 50 ? p.Name.Substring(0, 50) + "..." : p.Name,
                                  })).OrderByDescending(p => p.NumberOfVisit).Skip(offset).Take(limit).AsNoTracking().ToListAsync();

            foreach (var product in products)
            {
                await ChangeOfProductStateAsync(product.Id);
                var discount = await DiscountManagerAsync(product.Id);
                product.ActionDiscount = discount.ToString().En2Fa();
                product.ImageName = await GetProductImage(product.Id);
            }
            return products;
        }

        public async Task<List<ProductViewModel>> GetRelatedProductAsync(int number/*, List<string> tagIdList, string productId*/)
        {
            int randomRow;
            var recentRandomRow = 0;
            //tagIdList.Insert(0, productId);
            //var whereExpression = "NewsId!=@0 and (";
            //for (int i = 0; i < tagIdList.Count() - 1; i++)
            //whereExpression = whereExpression + (i + 1) + (i + 1 != tagIdList.Count - 1 ? " or " : ")");

            var viewModels = new List<ProductViewModel>();
            var productCount = (from p in _context.Products.Where(p => p.States <= ProductState.Ready) select p).Count();
            //.Where(whereExpression, tagIdList.ToArray()).Count();

            for (int i = 0; i < number && i < productCount; i++)
            {
                randomRow = CustomMethods.RandomNumber(1, productCount + 1);
                while (recentRandomRow == randomRow)
                    randomRow = CustomMethods.RandomNumber(1, productCount + 1);

                var product = await (from p in _context.Products
                                     .Where(p => p.States == ProductState.Ready)
                                     .Include(p => p.Visits).Include(p => p.Comments).ThenInclude(c => c.Likes)
                                     select new ProductViewModel
                                     {
                                         Id = p.Id,
                                         Name = p.Name,
                                         Stock = p.Stock,
                                         Price = p.Price,
                                         InsertTime = p.InsertTime,
                                         NumberOfComments = p.Comments.Count(),
                                         NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum(),
                                         NumberOfLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == true)).Count(),
                                         NumberOfDisLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == false)).Count(),
                                     }).Skip(randomRow - 1).Take(1).FirstOrDefaultAsync();

                if (product != null)
                    viewModels.Add(product);
                recentRandomRow = randomRow;
            }
            foreach (var viewModel in viewModels)
            {
                await ChangeOfProductStateAsync(viewModel.Id);
                var discount = await DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await GetProductImage(viewModel.Id);
            }
            return viewModels;
        }

        public async Task<List<ProductViewModel>> GetPreferedProductsAsync(int number)
        {
            int randomRow;
            var recentRandomRow = 0;
            var viewModels = new List<ProductViewModel>();
            var productCount = (from p in _context.Products.Where(p => p.IsComplete == true && p.States == ProductState.Ready) select p).Count();

            for (int i = 0; i < number && i < productCount; i++)
            {
                randomRow = CustomMethods.RandomNumber(1, productCount + 1);
                while (recentRandomRow == randomRow)
                    randomRow = CustomMethods.RandomNumber(1, productCount + 1);

                var product = await (from p in _context.Products
                                     .Where(p => p.States == ProductState.Ready && p.IsPrefered == true)
                                     .Include(p => p.Visits).Include(p => p.Comments).ThenInclude(c => c.Likes)
                                     select new ProductViewModel
                                     {
                                         Id = p.Id,
                                         Name = p.Name,
                                         Stock = p.Stock,
                                         Price = p.Price,
                                         InsertTime = p.InsertTime,
                                         NumberOfComments = p.Comments.Count(),
                                         NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum(),
                                         NumberOfLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == true)).Count(),
                                         NumberOfDisLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == false)).Count(),
                                     }).Skip(randomRow - 1).Take(1).FirstOrDefaultAsync();

                if (product != null)
                    viewModels.Add(product);
                recentRandomRow = randomRow;
            }
            foreach (var viewModel in viewModels)
            {
                await ChangeOfProductStateAsync(viewModel.Id);
                var discount = await DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await GetProductImage(viewModel.Id);
            }
            return viewModels;
        }

        public async Task<List<ProductViewModel>> GetUserBookmarksAsync(int userId)
        {
            return await (from b in _context.Bookmarks
                          join p in _context.Products on b.ProductId equals p.Id
                          where (b.UserId == userId && b.RemoveTime == null)
                          select new ProductViewModel
                          {
                              Id = p.Id,
                              Name = p.Name,
                              PersianInsertDate = p.InsertTime.ConvertMiladiToShamsi("dd MMMM yyyy ساعت HH:mm"),
                          }).ToListAsync();
        }

        public async Task InsertVisitOfUserAsync(string productId, Guid browserId, string ipAddress)
        {
            var visit = FindVisitAsIEnumerableAsync(v => v.ProductId == productId && v.BrowserId == browserId && v.IpAddress == ipAddress).Result.FirstOrDefault();
            if (visit != null && visit.LastVisitDateTime.Date != DateTime.Now.Date)
            {
                visit.NumberOfVisit = visit.NumberOfVisit + 1;
                visit.LastVisitDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            else if (visit == null)
            {
                visit = new Visit { IpAddress = ipAddress, LastVisitDateTime = DateTime.Now, ProductId = productId, NumberOfVisit = 1 };
                await _context.Visits.AddAsync(visit);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<IEnumerable<Visit>> FindVisitAsIEnumerableAsync(Expression<Func<Visit, bool>> filter = null, params Expression<Func<Visit, object>>[] includes)
        {
            IQueryable<Visit> query = _context.Set<Visit>();
            foreach (var include in includes)
                query = query.Include(include);
            if (filter != null)
                query = query.Where(filter);
            return await query.ToListAsync();
        }

        public async Task<string> GetWeeklyProductDiscountAsync(string url)
        {
            var content = "";
            var EndMiladiDate = DateTime.Now;
            var NumOfWeek = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dddd").GetNumOfWeek();
            var StartMiladiDate = DateTime.Now.AddDays((-1) * NumOfWeek).Date + new TimeSpan(0, 0, 0);

            var viewModels = await (from p in _context.Products.Include(v => v.Visits).Include(l => l.Comments).ThenInclude(c => c.Likes)
                                    where (p.InsertTime <= EndMiladiDate && StartMiladiDate <= p.InsertTime)
                                    select (new ProductViewModel
                                    {
                                        Id = p.Id,
                                        Stock = p.Stock,
                                        Price = p.Price,
                                        Name = p.Name.Length > 50 ? p.Name.Substring(0, 50) + "..." : p.Name,
                                    })).OrderByDescending(o => o.InsertTime).AsNoTracking().ToListAsync();

            foreach (var item in viewModels)
            {
                await ChangeOfProductStateAsync(item.Id);
                var discount = await DiscountManagerAsync(item.Id);
                item.ActionDiscount = discount.ToString().En2Fa();
                item.ImageName = await GetProductImage(item.Id);
                content = content +
                    $" <div style='direction:rtl;font-family:tahoma;text-align:center'>" +
                    $" <div class='row align-items-center'>" +
                    $" <div class='col-12 col-lg-6'>" +
                    $" <div class='post-thumbnail'>" +
                    $" <img src='{url + "/productImage/" + item.ImageName}' alt='{item.ImageName}'> " +
                    $" </div>" +
                    $" </div> <div class='col-12 col-lg-6'>" +
                    $" <div class='post-content mt-0'>" +
                    $" <h4 style='color:#878484;'>{item.Name}</h4>" +
                    $" <p> <a href='{url}'>[ادامه مطلب]</a> </p>" +
                    $" </div> </div> </div> </div><hr/>";
            }
            return content;
        }

        public async Task<int> InsertProductGuarantee(string guaranteeName)
        {
            int id;
            var guarantee = await _context.Guarantees.FirstOrDefaultAsync(b => b.Name == guaranteeName);
            if (guarantee != null)
            {
                id = guarantee.Id;
                guarantee.Name = guaranteeName;
                guarantee.InsertTime = DateTime.Now;
                _context.Guarantees.Update(guarantee);
            }
            else
            {
                var guarantees = _context.Guarantees.Where(b => b.RemoveTime == null).ToList();
                if (guarantees.Count() == 0)
                    id = 1;
                else
                    id = guarantees.OrderByDescending(c => c.Id).First().Id + 1;
                await _context.Guarantees.AddAsync(new Guarantee { Name = guaranteeName, Id = id, InsertTime = DateTime.Now });
            }
            await _context.SaveChangesAsync();
            return id;
        }

        public async Task<List<string>> GetProductImagesAsync(string productId, int offset, int limit)
        {
            var images = new List<string>();
            var fileStores = await _context.FileStores.Where(f => f.ProductId == productId)
                .OrderByDescending(f => f.InsertTime).Skip(offset).Take(limit).AsNoTracking().ToListAsync();
            foreach (var fileStore in fileStores)
                images.Add(fileStore.ImageName);
            return images;
        }

        public async Task ChangeOfProductStateAsync(string productId)
        {
            var save = false;
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product.InsertTime <= DateTime.Now && product.States == ProductState.CoomingSoon)
            {
                product.States = ProductState.Demo;
                save = true;
            }
            if (product.InsertTime > DateTime.Now && product.States != ProductState.CoomingSoon)
            {
                product.States = ProductState.CoomingSoon;
                save = true;
            }
            if (product.Stock <= 0)
            {
                product.Stock = 0;
                product.States = ProductState.EndOfStock;
                save = true;
            }
            if (product.ExpirationDate <= DateTime.Now)
            {
                product.States = ProductState.ExpirationEnd;
                save = true;
            }
            if (save)
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int?> DiscountManagerAsync(string productId, bool save = false)
        {
            var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.ProductId == productId);
            if (discount == null) return null;

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return null;
            if (product.Price <= 0) return 0;
            if (product.States != ProductState.Ready) return 0;
            var actionDiscount = product.Price * Convert.ToInt32(discount.Percent) / 100;
            if (save && discount.EndDate > DateTime.Now)
            {
                product.Price = actionDiscount;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            return actionDiscount;
        }

        public int CountProducts() => _context.Products.Where(p => p.RemoveTime == null && p.Stock != 0 && p.States != ProductState.CoomingSoon && p.IsComplete).Count();
        public int CountFutureProducts() => _context.Products.Where(p => p.InsertTime > DateTime.Now || p.States == ProductState.CoomingSoon).Count();
        public int CountProductPresentation() => _context.Products.Where(p => p.IsComplete == true && p.States == ProductState.Ready && p.InsertTime <= DateTime.Now).Count();
        public int CountProductPresentationOrDemo(bool isDemo) => _context.Products.Where(p => isDemo ? p.States == ProductState.Demo && p.IsComplete : p.InsertTime <= DateTime.Now && p.States == ProductState.CoomingSoon && !p.IsComplete).Count();

        public Product FindByProductName(string productName) => _context.Products.FirstOrDefault(c => c.Name.Contains(productName.TrimStart().TrimEnd()));

        private async Task<string> GetProductImage(string productId) => await _context.FileStores.FirstOrDefaultAsync(f => f.ProductId == productId) == null ? null : _context.FileStores.FirstOrDefaultAsync(f => f.ProductId == productId).Result.ImageName;
        //public string SearchInProducts(string searchText) => _context.Products.SingleOrDefault(p => (p.Name.Contains(searchText) == true || p.Description.Contains(searchText)) && p.States == ProductState.Ready).Id;
    }
}
