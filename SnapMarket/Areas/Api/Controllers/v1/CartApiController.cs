using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SnapMarket.Entities;
using SnapMarket.Common.Api;
using SnapMarket.Data.Contracts;
using System.Collections.Generic;
using SnapMarket.ViewModels.Cart;
using SnapMarket.Common.Extensions;
using SnapMarket.Services.Contracts;
using SnapMarket.Common.Api.Attributes;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [DisplayName("CartApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class CartApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IApplicationUserManager _userManager;
        public CartApiController(IUnitOfWork uw, IMapper mapper, IApplicationUserManager userManager)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
        }

        // [GET]  api/v1/CartApi
        [HttpGet]
        public async Task<ApiResult<CartViewModel>> Cart(string phoneNumber, string browserId)
        {
            var user = await _userManager.FindByPhoneNumberAsync(phoneNumber);
            if (user == null)
                return Ok();

            var viewModel = new CartViewModel();
            var result = await _uw.CartRepository.GetCartAsync(user.Id, browserId);
            if (result.Id == 0)
            {
                var id = 0;
                var carts = await _uw.BaseRepository<Cart>().FindAllAsync();
                if (carts.Count() != 0)
                    id = carts.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
                var cart = new Cart
                {
                    UserId = user.Id,
                    IsComplete = false,
                    BrowserId = browserId,
                    InsertTime = DateTime.Now,
                    Id = carts.Count() == 0 ? 1 : id,
                    CartItems = new List<CartItem>(),
                };
                await _uw.BaseRepository<Cart>().CreateAsync(cart);
                await _uw.Commit();
                return Ok(result);
            }
            else
            {
                viewModel = new CartViewModel
                {
                    SumAmount = result.SumAmount,
                    CartItems = result.CartItems,
                    ProductCount = result.ProductCount,
                };

                foreach (var item in viewModel.CartItems)
                {
                    item.Colors = new List<string>();
                    foreach (var color in item.NameOfColor.Trim().Replace(" ", "").Split(","))
                        if (color != "")
                            item.Colors.Add(color);
                    item.Image = await _uw.FileRepository.FindImageAsync(item.ProductId, null, null);
                }
                return Ok(viewModel);
            }
        }

        // [Post]  api/v1/CartApi/{productId}
        [HttpPost]
        public virtual ApiResult<object> Post(string productId, string browserId)
        {
            var result = _uw.CartRepository.AddToCart(productId, browserId);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

        // [Put]  api/v1/CartApi/{cartItemId}
        [HttpPut]
        public virtual ApiResult<IActionResult> Increase(int cartItemId)
        {
            var result = _uw.CartRepository.Increase(cartItemId);
            if (result.IsSuccess == true)
                return Ok();
            else
                return BadRequest();
        }

        // [Put]  api/v1/CartApi/{cartItemId}
        [HttpPut("Decrease")]
        public virtual ApiResult<IActionResult> Decrease(int cartItemId)
        {
            var result = _uw.CartRepository.Decrease(cartItemId);
            if (result.IsSuccess == true)
                return Ok();
            else
                return BadRequest();
        }

        // [DELETE]  api/v1/CartApi/{productId}
        [HttpDelete("Delete")]
        public virtual ApiResult<object> Delete(string productId, string browserId)
        {
            var result = _uw.CartRepository.RemoveFromCart(productId, browserId);
            if (result.IsSuccess)
                return Ok();
            else
                return BadRequest();
        }

        // [DELETE]  api/v1/CartApi
        [HttpDelete("DeleteAll")]
        public virtual ApiResult<object> DeleteAll(string browserId)
        {
            var result = _uw.CartRepository.RemoveAllFromCart(browserId);
            if (result.IsSuccess)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }
    }
}
