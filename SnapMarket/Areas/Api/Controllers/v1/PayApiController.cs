using System.ComponentModel;
using System.Threading.Tasks;
using Dto.Payment;
using ZarinPal.Class;
using Microsoft.AspNetCore.Mvc;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.ViewModels.Order;
using SnapMarket.Common.Extensions;
using SnapMarket.Common.Api.Attributes;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [DisplayName("PayApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class PayApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        private readonly Payment _payment;
        //private readonly Authority _authority;
        //private readonly Transactions _transactions;
        public PayApiController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));

            var expose = new Expose();
            _payment = expose.CreatePayment();
            //_authority = expose.CreateAuthority();
            //_transactions = expose.CreateTransactions();
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(int userId, string browserId, string discountCode, string transport)
        {
            var cart = await _uw.CartRepository.GetCartAsync(userId, browserId);
            if (cart.SumAmount > 80000)
            {
                var transportType = transport == "حمل و نقل رایگان" ? TransportType.Free : transport == "عادی" ? TransportType.Normal : transport == "استانی" ? TransportType.Province : TransportType.Special;                
                var requestPay = _uw.PaymentRepository.AddRequestPay(cart.SumAmount, userId, discountCode, transportType);                
                if (!requestPay.IsSuccess) return BadRequest();
                var result = await _payment.Request(new DtoRequest()
                {
                    Mobile = "09192232012",
                    Email = requestPay.Data.Email,
                    Amount = requestPay.Data.Amount,
                    MerchantId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                    Description = "پرداخت فاکتور شماره:" + requestPay.Data.Id,
                    CallbackUrl = $"https://localhost:44309/api/v1/PayApi/Verify?requestPayId={requestPay.Data.Id}&userId={userId}&browserId={browserId}",
                }, ZarinPal.Class.Payment.Mode.sandbox);
                //var transaction = _transactions.GetUnverified(new Dto.Other.DtoMerchant { MerchantId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX" }, Payment.Mode.sandbox);
                //var autority = _authority.Refresh(new DtoRefreshAuthority { Authority = result.Authority, MerchantId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX", ExpireIn = 1440 }, Payment.Mode.sandbox);
                return Ok(result.Authority);
            }
            else
                return BadRequest();
        }

        [HttpGet("Verify")]
        public async Task<IActionResult> Verify(string requestPayId, int userId, string browserId, string authority)
        {
            var requestPay = _uw.PaymentRepository.GetRequestPay(requestPayId);
            var verification = await _payment.Verification(new DtoVerification
            {
                Authority = authority,
                Amount = requestPay.Data.Amount,
                MerchantId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"
            }, Payment.Mode.sandbox);

            var cart = await _uw.CartRepository.GetCartAsync(userId, browserId);
            if (verification.Status == 100)
            {
                await _uw.OrderRepository.RequestAddNewOrderAsync(new RequestAddNewOrder
                {
                    UserId = userId,
                    CartId = cart.Id,
                    Authority = authority,
                    RefId = verification.RefId,
                    RequestPayId = requestPay.Data.Id,
                    DispatchNumber = verification.RefId.ToString()
                });
                return new RedirectResult("http://localhost:4200/order", true);
            }
            else
                return new RedirectResult("http://localhost:4200", false);
        }
    }
}
