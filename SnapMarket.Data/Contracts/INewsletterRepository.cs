using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.ViewModels.Newsletter;
using SnapMarket.Entities;

namespace SnapMarket.Data.Contracts
{
    public interface INewsletterRepository
    {
        Task<List<NewsletterViewModel>> GetPaginateNewsletterAsync(int offset, int limit, string orderBy, string searchText);
        Task<Newsletter> FindByEmailAsync(string email);
    }
}
