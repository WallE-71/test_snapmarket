using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SnapMarket.Entities;
using SnapMarket.Common.Api;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.ViewModels.Api.Home;
using SnapMarket.Common.Api.Attributes;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [DisplayName("ProductApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class ProductApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        private readonly IHttpContextAccessor _accessor;
        public ProductApiController(IUnitOfWork uw, IHttpContextAccessor accessor)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _accessor = accessor;
            _accessor.CheckArgumentIsNull(nameof(_accessor));
        }

        // [GET]  api/v1/ProductApi/Get/{categoryName}
        [HttpGet("GetProductInCategory")]
        public virtual async Task<ApiResult<List<ProductInCategoriesDto>>> GetProductInCategory(string categoryName)
        {
            var productInCategory = await _uw.ProductRepository.GetProductInCategoryAsync(categoryName);
            if (productInCategory.Count == 0)
                return NotFound();
            else
                return Ok(productInCategory);
        }

        //[GET] api/v1/ProductApi

        [HttpGet("Index")]
        public virtual async Task<IActionResult> Index(string duration, string TypeOfNews)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax && TypeOfNews == "MostViewedNews")
                return Ok(/*"_MostViewedNews",*/ await _uw.ProductRepository.MostViewedProductAsync(0, 3, duration));
            else if (isAjax && TypeOfNews == "MostTalkNews")
                return Ok(/*"_MostTalkNews",*/ await _uw.ProductRepository.MostTalkProduct(0, 5, duration));
            else
            {
                int countProductPresentation = _uw.ProductRepository.CountProductPresentation();
                var mostViewedProduct = await _uw.ProductRepository.MostViewedProductAsync(0, 5, "day");
                var mostTalkProduct = await _uw.ProductRepository.MostTalkProduct(0, 5, "day");
                var mostPopularProducts = await _uw.ProductRepository.MostPopularProducts(0, 5);
                var homeViewModel = new { mostViewedProduct, mostTalkProduct, mostPopularProducts, countProductPresentation };
                return Ok(homeViewModel);
            }
        }

        [HttpGet("ProductDetails")]
        public async Task<IActionResult> ProductDetails(string productId, Guid browserId, int userId)
        {
            if (!productId.HasValue())
                return NotFound();
            else
            {
                var existProducts = await _uw.BaseRepository<Product>().FindByIdAsync(productId);
                if (existProducts == null)
                    return NotFound();
                else
                {
                    if (browserId.ToString().HasValue() && browserId != new Guid())
                    {
                        var IpAddress = _accessor.HttpContext?.Connection?.RemoteIpAddress.ToString();
                        await _uw.ProductRepository.InsertVisitOfUserAsync(productId, browserId, IpAddress);
                    }

                    var product = await _uw.ProductRepository.GetProductByIdAsync(productId, userId != 0 ? userId : 0);
                    var seller = await _uw.SellerRepository.GetSellerForProductAsync(productId);
                    var productsRelated = await _uw.ProductRepository.GetRelatedProductAsync(5);
                    var productsPrefered = await _uw.ProductRepository.GetPreferedProductsAsync(5);
                    var productComments = await _uw.CommentRepository.GetProductCommentsAsync(productId);
                    var productDetailsViewModel = new { product, seller, productsRelated, productsPrefered, productComments };
                    return Ok(productDetailsViewModel);
                }
            }
        }

        [HttpGet("Bookmark")]
        public async Task<IActionResult> BookmarkProduct(string productId, string userId)
        {
            if (productId.HasValue() && userId != "null")
            {
                int id;
                Int32.TryParse(userId, out id);
                var bookmark = _uw.BaseRepository<Bookmark>().FindByConditionAsync(b => b.UserId == id && b.ProductId == productId).Result.FirstOrDefault();
                if (bookmark == null)
                {
                    bookmark = new Bookmark { ProductId = productId, UserId = id, InsertTime = DateTime.Now };
                    await _uw.BaseRepository<Bookmark>().CreateAsync(bookmark);
                    await _uw.Commit();
                    return Ok(true);
                }
                else
                {
                    var result = false;
                    if (bookmark.RemoveTime == null)
                        bookmark.RemoveTime = DateTime.Now;
                    else
                    {
                        bookmark.RemoveTime = null;
                        bookmark.InsertTime = DateTime.Now;
                        result = true;
                    }
                    _uw.BaseRepository<Bookmark>().Update(bookmark);
                    await _uw.Commit();
                    return Ok(result);
                }
            }
            else
                return NotFound();
        }


        [HttpGet("Search")]
        public ApiResult<string> Search(string searchText)
        {
            try
            {
                return Ok(_uw.Context.Products.Single(p => (p.Name.Contains(searchText) == true || p.Description.Contains(searchText))
                                                                     && p.States == ProductState.Ready).Id);
            }
            catch
            {
                return NotFound("محصول درخواستی یافت نشد!");
            }
            //return Ok( _uw.ProductRepository.SearchInProducts(searchText));
        }

        [HttpGet]
        public IActionResult Error()
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult Error404()
        {
            return Ok();
        }
    }
}
