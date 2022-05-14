using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SnapMarket.Entities;
using SnapMarket.Common.Api;
using SnapMarket.ViewModels;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.Services.Contracts;
using SnapMarket.Common.Api.Attributes;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [DisplayName("RateApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class RateApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        private readonly IApplicationUserManager _userManager;
        public RateApiController(IUnitOfWork uw, IApplicationUserManager userManager)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
        }

        // [Post]  api/v1/RateApi
        [HttpPost]
        public virtual async Task<bool> RateToSeller(int sellerId, string productId, string userId, int rate)
        {
            if (productId.HasValue() && userId != null)
            {
                int id;
                Int32.TryParse(userId, out id);
                var currentRate = await _uw.BaseRepository<Rating>().FindByConditionAsync(r => r.SellerId == sellerId && r.UserId == id);

                var order = await _uw.BaseRepository<Order>().FindByConditionAsync(o => o.UserId == id && o.RemoveTime == null &&
                                            o.OrderDetails.Select(od => od.ProductId == productId).FirstOrDefault(),
                                            o => o.OrderBy(s => s.InsertTime));
                if (order.Count() != 0)
                {
                    if (currentRate.Count() == 0)
                    {
                        var rating = new Rating();
                        rating.UserId = id;
                        rating.UserVote = rate;
                        rating.SellerId = sellerId;
                        rating.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                        await _uw.BaseRepository<Rating>().CreateAsync(rating);
                    }
                    else
                    {
                        currentRate.FirstOrDefault().UserVote = rate;
                        currentRate.FirstOrDefault().InsertTime = DateTime.Now;
                    }

                    await _uw.Commit();
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
