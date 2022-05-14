using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SnapMarket.ViewModels;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;

namespace SnapMarket.Data.Repositories
{
    public class SellerRepository : ISellerRepository
    {
        private readonly SnapMarketDBContext _context;
        public SellerRepository(SnapMarketDBContext context)
        {
            _context = context;
        }

        public async Task<List<SellerViewModel>> GetPaginateSellersAsync(int offset, int limit, string orderBy, string searchText)
        {
            var getDateTimesForSearch = searchText.GetDateTimeForSearch();
            var viewModels = await _context.Sellers.Include(s => s.Ratings).Include(s => s.Products)
                                    .Where(s => s.RemoveTime != null || s.Brand.Contains(searchText) || s.RegisterDate >= getDateTimesForSearch.First() && s.RegisterDate <= getDateTimesForSearch.Last())
                                    .OrderBy(orderBy).Skip(offset).Take(limit)
                                    .Select(s => new SellerViewModel
                                    {
                                        Id = s.Id,
                                        Name = s.Name,
                                        Brand = s.Brand,
                                        Email = s.Email,
                                        SurName = s.SurName,
                                        WebSite = s.WebSite,
                                        NationalId = s.NationalId,
                                        InsertTime = s.InsertTime,
                                        IsComplete = s.IsComplete, //register in website?
                                        PhonNumber = s.PhonNumber,
                                        Description = s.Description,
                                        RegisterDate = s.RegisterDate,
                                        ActivityType = s.ActivityType,
                                        PersianInsertTime = s.InsertTime.DateTimeEn2Fa("yyyy/MM/dd"),
                                        PersianRegisterDate = s.RegisterDate.DateTimeEn2Fa("yyyy/MM/dd"),
                                        NumberOfProducts = s.Products.Where(p => p.SellerId == s.Id).Count(),
                                        UserVote = s.Ratings.FirstOrDefault(p => p.SellerId == s.Id).UserVote,
                                        Store = _context.Stores.FirstOrDefault(p => p.SellerId == s.Id).Name,
                                        Address = _context.Stores.FirstOrDefault(p => p.SellerId == s.Id).Address,
                                        TelNumber = _context.Stores.FirstOrDefault(p => p.SellerId == s.Id).TelNumber,
                                        PostalCode = _context.Stores.FirstOrDefault(p => p.SellerId == s.Id).PostalCode,
                                        EstablishmentDate = _context.Stores.FirstOrDefault(p => p.SellerId == s.Id).EstablishmentDate,
                                        PersianEstablishmentDate = _context.Stores.FirstOrDefault(p => p.SellerId == s.Id).EstablishmentDate.ConvertMiladiToShamsi("yyyy/MM/dd"),
                                    }).AsNoTracking().ToListAsync();

            foreach (var item in viewModels)
                item.Row = ++offset;
            return viewModels;
        }

        public async Task<SellerViewModel> GetSellerForProductAsync(string productId)
        {
            var viewModel = await (from s in _context.Sellers.Include(s => s.Ratings)
                                   join product in _context.Products on s.Id equals product.SellerId
                                   where (product.Id == productId)
                                   select (new SellerViewModel
                                   {
                                       Id = s.Id,
                                       Name = s.Name,
                                       InsertTime = s.InsertTime,
                                       IsComplete = s.IsComplete,
                                       Description = s.Description,
                                       RegisterDate = s.RegisterDate,
                                       PersianInsertTime = s.InsertTime.DateTimeEn2Fa("yyyy/MM/dd"),
                                       PersianRegisterDate = s.RegisterDate.DateTimeEn2Fa("yyyy/MM/dd"),
                                       NumberOfProducts = s.Products.Where(p => p.SellerId == s.Id).Count(),
                                       UserVote = s.Ratings.FirstOrDefault(r => r.SellerId == s.Id).UserVote,
                                   })).AsNoTracking().FirstOrDefaultAsync();

            var image = await _context.FileStores.FirstOrDefaultAsync(f => f.SellerId == viewModel.Id);
            if (image != null)
                viewModel.SellerImage = image.ImageName;
            return viewModel;
        }
    }
}
