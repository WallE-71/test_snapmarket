using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SnapMarket.Common.Api;
using SnapMarket.ViewModels;
using SnapMarket.Data.Contracts;
using SnapMarket.ViewModels.Order;
using SnapMarket.Common.Extensions;
using SnapMarket.Services.Contracts;
using SnapMarket.Common.Api.Attributes;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [DisplayName("OrderApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class OrderApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        public OrderApiController(IUnitOfWork uw, IApplicationUserManager userManager)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        // [GET]  api/v1/OrderApi?userId
        [HttpGet]
        public virtual ApiResult<List<UserOrderViewModel>> GetOrders(int userId, int offset, int limit)
        {
            if (userId != 0)
                return Ok(_uw.OrderRepository.GetUserOrdersAsync(userId, offset, limit).Result.Data);
            return null;
        }

        // [Post]  api/v1/OrderApi?orderId=&userId=
        [HttpPost]
        public virtual ResultViewModel RemoveOrder(int orderId, int userId)
        {
            if (userId != 0 && orderId != 0)
                return _uw.OrderRepository.RemoveOrder(orderId, userId);
            return null;
        }
    }
}
