using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.ViewModels;

namespace SnapMarket.Data.Contracts
{
    public interface IMessageUserRepository
    {
        int CountUnAnsweredComments();
        Task<List<MessageUsersViewModel>> GetPaginateMessagesAsync(int offset, int limit, string orderBy, string searchText, int userId, bool? isRegisterCode);
    }
}
