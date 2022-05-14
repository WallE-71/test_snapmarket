using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SnapMarket.Entities;
using SnapMarket.ViewModels;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using static SnapMarket.ViewModels.RequestPay.RequestPayViewModel;

namespace SnapMarket.Data.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly SnapMarketDBContext _context;
        public PaymentRepository(SnapMarketDBContext context)
        {
            _context = context;
        }

        public async Task<List<ItemRequestPay>> GetPaginateRequestPaysAsync(int offset, int limit, bool? sortAsc, string searchText)
        {
            List<ItemRequestPay> requestPays;
            //var getDateTimesForSearch = searchText.GetDateTimeForSearch();
            requestPays = await _context.RequestPays
                                //.Where(r => r.InsertTime == null ? r.InsertTime >= getDateTimesForSearch.First() && r.InsertTime <= getDateTimesForSearch.Last() : false)
                                .Select(r => new ItemRequestPay
                                {
                                    Id = r.Id,
                                    IsComplete = r.IsComplete,
                                    DisplayAmount = r.Amount.ToString().En2Fa(),
                                    PersianUpdateTime = r.UpdateTime.DateTimeEn2Fa("yyyy/MM/dd ساعت HH:mm:ss"),
                                }).Skip(offset).Take(limit).AsNoTracking().ToListAsync();

            if (sortAsc != null)
                requestPays = requestPays.OrderBy(t => (sortAsc == true && sortAsc != null) ? t.Amount : 0).OrderByDescending(t => (sortAsc == false && sortAsc != null) ? t.Amount : 0).ToList();

            foreach (var item in requestPays)
                item.Row = ++offset;
            return requestPays;
        }

        public ResultViewModel<ItemRequestPay> GetRequestPay(string requestPayId)
        {
            var requestPay = _context.RequestPays.Where(p => p.Id == requestPayId).FirstOrDefault();
            if (requestPay != null)
            {
                return new ResultViewModel<ItemRequestPay>()
                {
                    Data = new ItemRequestPay()
                    {
                        Id = requestPay.Id,
                        Amount = requestPay.Amount,
                        DisplayAmount = requestPay.Amount.ToString().En2Fa(),
                    }
                };
            }
            else
                throw new Exception("request pay not found");
        }

        public ResultViewModel<ResultRequestPay> AddRequestPay(int amount, int userId, string discountCode, TransportType transportType)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var discount = _context.Discounts.FirstOrDefault(p => p.DiscountCode == discountCode);
            var transportPrice = transportType == TransportType.Free ? 0 : transportType == TransportType.Normal ? 10000 : transportType == TransportType.Province ? 30000 : 50000;

            double _amount, removeDecimal;
            double.TryParse(amount.ToString(), out _amount);
            var truncate = Math.Truncate(_amount / 9);
            double.TryParse(truncate.ToString(), out removeDecimal);
            amount += Convert.ToInt32(removeDecimal) + transportPrice;
            amount *= (discount == null ? 1 : (int)discount.Percent/100);
            var findRequestPay = _context.RequestPays.Where(r => r.UserId == userId && r.IsComplete == false).FirstOrDefault();
            if (findRequestPay != null)
            {
                findRequestPay.Tax = 9;
                findRequestPay.Amount = amount;
                findRequestPay.UpdateTime = findRequestPay.IsComplete ? findRequestPay.UpdateTime = DateTime.Now : null;
                _context.RequestPays.Update(findRequestPay);
                _context.SaveChanges();
                return new ResultViewModel<ResultRequestPay>()
                {
                    Data = new ResultRequestPay
                    {
                        Email = user.Email,
                        Id = findRequestPay.Id,
                        Amount = findRequestPay.Amount,
                    },
                    IsSuccess = true,
                };
            }
            var requestPay = new RequestPay()
            {
                Tax = 9,
                User = user,
                Amount = amount,
                Transports = transportType,
                Id = StringExtensions.GenerateId(10),
                UseDiscount = discount == null ? false : true,
            };
            _context.RequestPays.Add(requestPay);
            _context.SaveChanges();

            return new ResultViewModel<ResultRequestPay>()
            {
                Data = new ResultRequestPay
                {
                    Id = requestPay.Id,
                    Email = user.Email,
                    Transport = transportType,
                    Amount = requestPay.Amount,
                },
                IsSuccess = true,
            };
        }
    }
}
