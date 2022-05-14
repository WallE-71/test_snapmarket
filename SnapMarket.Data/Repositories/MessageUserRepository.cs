using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SnapMarket.Entities;
using SnapMarket.ViewModels;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;

namespace SnapMarket.Data.Repositories
{
    public class MessageUserRepository : IMessageUserRepository
    {
        private readonly SnapMarketDBContext _context;
        public MessageUserRepository(SnapMarketDBContext context)
        {
            _context = context;
        }

        public int CountUnAnsweredComments() => _context.MessageUsers.Where(m => m.Answer != null && m.IsComplete == true).Count();

        public async Task<List<MessageUsersViewModel>> GetPaginateMessagesAsync(int offset, int limit, string orderBy, string searchText, int userId, bool? isRegisterCode)
        {
            //bool? convertConfirm = Convert.ToBoolean(isRegisterCode);
            var getDateTimesForSearch = searchText.GetDateTimeForSearch();

            var messages = await _context.MessageUsers.Include(m => m.User)
                                   //.Where(n => (isRegisterCode == false) && n.UserId.ToString().Contains(userId.ToString()) && ((n.RegisterDateTime >= getDateTimesForSearch.First())))
                                   .OrderBy(orderBy).Skip(offset).Take(limit)
                                   .Select(m => new MessageUsersViewModel
                                   {
                                       Id = m.Id,
                                       Email = m.Email,
                                       Answer = m.Answer,
                                       UserId = m.User.Id,
                                       Messages = m.Messages,
                                       Description = m.Description,
                                       IsComplete = m.User.IsValidAccount == true ? m.IsComplete : !m.IsComplete,
                                       PersianInsertTime = m.InsertTime.DateTimeEn2Fa("yyyy/MM/dd ساعت HH:mm:ss"),
                                       Message = m.Messages == MessageType.Tiket ? "تیکت" : m.Messages == MessageType.RequestRegister ? "درخواست عضویت بعنوان فروشنده‌" : m.Messages == MessageType.FeedbackOffers ? "انتقادات و پیشنهادات" :
                                            m.Messages == MessageType.Questions ? "سوالات" : "سایر پیام‌ها",
                                   }).AsNoTracking().ToListAsync();

            foreach (var item in messages)
                item.Row = ++offset;

            return messages;
        }
    }
}
