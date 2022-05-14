using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SnapMarket.Entities;
using SnapMarket.ViewModels;
using SnapMarket.Data.Contracts;
using SnapMarket.ViewModels.Cart;

namespace SnapMarket.Data.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly SnapMarketDBContext _context;
        public CartRepository(SnapMarketDBContext context)
        {
            _context = context;
        }

        public async Task<List<CartViewModel>> GetPaginateCartsAsync(int offset, int limit, string orderBy, string searchText)
        {
            var carts = await _context.Carts.Include(c => c.CartItems).Include(c => c.User)
                                            .Where(c => (c.RemoveTime == null)
                                             || (c.User.FirstName.Contains(searchText)) || (c.User.LastName.Contains(searchText)))
                                            .OrderBy(orderBy).Skip(offset).Take(limit)
                                            .Select(c => new CartViewModel
                                            {
                                                Id = c.Id,
                                                ProductCount = c.CartItems.Count,
                                                SumAmount = c.CartItems.Sum(p => p.Price * p.Count),
                                                Customer = c.User.FirstName + " " + c.User.LastName
                                            }).AsNoTracking().ToListAsync();

            foreach (var item in carts)
                item.Row = ++offset;

            return carts;
        }

        public async Task<List<CartItemViewModel>> GetPaginateCartItemsAsync(int offset, int limit, string orderBy, string searchText, int cartId)
        {
            var cartItems = await _context.CartItems.Include(ci => ci.Product).ThenInclude(p => p.FileStores)
                                            .Where(p => (p.CartId == cartId) || (p.Product.Name == searchText))
                                            .OrderBy(orderBy).Skip(offset).Take(limit)
                                            .Select(ci => new CartItemViewModel
                                            {
                                                Id = ci.Id,
                                                Price = ci.Price,
                                                Count = ci.Count,
                                                ProductName = ci.Product.Name,
                                                Image = ci.Product.FileStores.FirstOrDefault(f => f.ProductId == ci.Product.Id).ImageName,
                                            }).AsNoTracking().ToListAsync();

            foreach (var item in cartItems)
                item.Row = ++offset;
            return cartItems;
        }

        public async Task<CartViewModel> GetCartAsync(int userId, string browserId)
        {
            var cart = await (from c in _context.Carts.Include(c => c.User).Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                                         where (c.User.Id == userId && c.BrowserId == browserId && c.IsComplete == false && c.RemoveTime == null)
                                         orderby(c.InsertTime)
                                         select (new CartViewModel
                                         {
                                             Id = c.Id,
                                             Customer = c.User.FirstName + c.User.LastName,
                                             ProductCount = c.CartItems.Where(ci => ci.RemoveTime == null).Count(),
                                             SumAmount = c.CartItems.Where(ci => ci.RemoveTime == null).Sum(ci => ci.Price * ci.Count)
                                         })).AsNoTracking().FirstOrDefaultAsync();

            if (cart != null)
                cart.CartItems = await BindCartItems(cart.Id);
            else
                cart = new CartViewModel();
            return cart;
        }

        public async Task<List<CartItemViewModel>> BindCartItems(int id)
        {
            var nameOfColor = "";
            var viewModels = new List<CartItemViewModel>();
            var cartItems = await (from ci in _context.CartItems
                                   where (ci.CartId == id && ci.RemoveTime == null)
                                   join pc in _context.ProductColors on ci.ProductId equals pc.ProductId into aa
                                   from productColor in aa.DefaultIfEmpty()
                                   join co in _context.Colors on productColor.ColorId equals co.Id into bb
                                   from color in bb.DefaultIfEmpty()
                                   select (new CartItemViewModel
                                   {
                                       Id = ci.Id,
                                       Count = ci.Count,
                                       Price = ci.Price,
                                       ProductId = ci.ProductId,
                                       ProductName = ci.Product.Name,
                                       NameOfColor = color != null ? color.Name : "",
                                   })).ToListAsync();

            var cartItemGroup = cartItems.GroupBy(g => g.Id).Select(g => new { Id = g.Key, CartItemGroup = g });
            foreach (var item in cartItemGroup)
            {
                nameOfColor = "";
                foreach (var a in item.CartItemGroup.Select(a => a.NameOfColor).Distinct())
                {
                    if (nameOfColor == "")
                        nameOfColor = a;
                    else
                        nameOfColor = nameOfColor + " , " + a;
                }

                var viewModel = new CartItemViewModel()
                {
                    Id = item.Id,
                    NameOfColor = nameOfColor,
                    Count = item.CartItemGroup.First().Count,
                    Price = item.CartItemGroup.First().Price,
                    Colors = item.CartItemGroup.First().Colors,
                    ProductId = item.CartItemGroup.First().ProductId,
                    ProductName = item.CartItemGroup.First().ProductName,
                };
                viewModels.Add(viewModel);
            }
            return viewModels;
        }

        public ResultViewModel<CartItem> AddToCart(string productId, string browserId)
        {
            try
            {
                var product = _context.Products.Find(productId);
                var cart = _context.Carts.Where(c => c.BrowserId == browserId && c.IsComplete == false && c.RemoveTime == null).FirstOrDefault();
                var cartItem = _context.CartItems.Where(ci => ci.ProductId == productId && ci.CartId == cart.Id && ci.RemoveTime == null).FirstOrDefault();
                if (cartItem != null)
                {
                    if (cartItem.RemoveTime != null)
                    {
                        cartItem.Count = 0;
                        cartItem.RemoveTime = null;
                    }
                    cartItem.Count++;
                }
                else
                {
                    var id = 0;
                    var cartItems = _context.CartItems.AsNoTracking().ToListAsync().Result;
                    if (cartItems.Count != 0)
                        id = cartItems.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
                    var newCartItem = new CartItem()
                    {
                        Count = 1,
                        Cart = cart,
                        CartId = cart.Id,
                        Product = product,
                        Price = product.Price,
                        ProductId = product.Id,
                        Id = cartItems.Count != 0 ? id : 1,
                    };
                    cartItem = newCartItem;
                    _context.CartItems.Add(newCartItem);
                }
                _context.SaveChanges();

                return new ResultViewModel<CartItem>()
                {
                    Data = cartItem,
                    IsSuccess = true,
                    Message = $"محصول  {product.Name} با موفقیت به سبد خرید شما اضافه شد ",
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<CartItem>()
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public ResultViewModel Increase(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            cartItem.Count++;
            _context.SaveChanges();
            return new ResultViewModel()
            {
                IsSuccess = true,
            };
        }

        public ResultViewModel Decrease(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem.Count <= 1)
            {
                return new ResultViewModel()
                {
                    IsSuccess = false,
                };
            }
            cartItem.Count--;
            _context.SaveChanges();

            return new ResultViewModel()
            {
                IsSuccess = true,
            };
        }

        public ResultViewModel RemoveFromCart(string productId, string browserId)
        {
            var cartitem = _context.CartItems.Where(c => c.ProductId == productId && c.Cart.BrowserId.ToString() == browserId && c.Cart.IsComplete == false).FirstOrDefault();
            if (cartitem != null)
            {
                cartitem.RemoveTime = DateTime.Now;
                _context.SaveChanges();

                return new ResultViewModel
                {
                    IsSuccess = true,
                    Message = "محصول از سبد خرید شما حذف شد"
                };
            }
            else
            {
                return new ResultViewModel
                {
                    IsSuccess = false,
                    Message = "محصول یافت نشد"
                };
            }
        }

        public ResultViewModel RemoveAllFromCart(string browserId)
        {
            var cart = _context.Carts.Include(c => c.CartItems).Where(c => c.BrowserId.ToString() == browserId && c.IsComplete == false).FirstOrDefault();
            if (cart != null)
            {
                var cartItems = cart.CartItems.Where(ci => ci.RemoveTime == null).ToList();
                foreach (var item in cartItems)
                    item.RemoveTime = DateTime.Now;

                cart.RemoveTime = DateTime.Now;
                _context.SaveChanges();

                return new ResultViewModel
                {
                    IsSuccess = true,
                    Message = "سبد خرید شما خالی شد"
                };
            }
            else
            {
                return new ResultViewModel
                {
                    IsSuccess = false,
                    Message = "سبد خرید یافت نشد"
                };
            }
        }
    }
}
