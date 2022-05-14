using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.Services.Contracts;

namespace SnapMarket.Services
{
    public class SendWeeklyNewsLetter : IInvocable
    {
        private IUnitOfWork _uw;
        private IEmailSender _emailSender;
        public SendWeeklyNewsLetter(IEmailSender emailSender, IUnitOfWork uw)
        {
            _uw = uw;
            _emailSender = emailSender;
        }

        public async Task Invoke()
        {
            var users = _uw.BaseRepository<Newsletter>().FindByConditionAsync(l => l.IsComplete == true).Result.ToList();
            var emailContent = await _uw.ProductRepository.GetWeeklyProductDiscountAsync(string.Format("/{0}:://{1}"));

            if (emailContent != "")
                foreach (var item in users)
                    await _emailSender.SendEmailAsync(item.Email, "اطلاعیه جدیدترین محصولات", emailContent);
        }
    }
}
