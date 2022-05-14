using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.ViewModels.Newsletter;

namespace SnapMarket.Data.Repositories
{
    public class NewsletterRepository : INewsletterRepository
    {
        private readonly SnapMarketDBContext _context;
        public NewsletterRepository(SnapMarketDBContext context)
        {
            _context = context;
        }

        public async Task<List<NewsletterViewModel>> GetPaginateNewsletterAsync(int offset, int limit, string orderBy, string searchText)
        {
            var getDateTimesForSearch = searchText.GetDateTimeForSearch();

            List<NewsletterViewModel> newsletter = await _context.Newsletters.Where(c => c.Email.Contains(searchText)
                                                            || (c.InsertTime >= getDateTimesForSearch.First() && c.InsertTime <= getDateTimesForSearch.Last()))
                                                          .OrderBy(orderBy).Skip(offset).Take(limit) // Dynamic Linq
                                                          .Select(l => new NewsletterViewModel
                                                          {
                                                              Email = l.Email,
                                                              IsComplete = l.IsComplete,
                                                              PersianInsertTime = l.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت HH:mm:ss")
                                                          }).AsNoTracking().ToListAsync();

            foreach (var item in newsletter)
                item.Row = ++offset;

            return newsletter;
        }

        public async Task<Newsletter> FindByEmailAsync(string email) => await _context.Newsletters.Where(n => n.Email == email).FirstAsync();
    }
}
