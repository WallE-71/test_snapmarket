using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SnapMarket.Entities;
using SnapMarket.Common.Api;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.Entities.Identity;
using SnapMarket.Services.Contracts;
using SnapMarket.Common.Api.Attributes;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [DisplayName("MessageApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class MessageApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        private readonly IApplicationUserManager _userManager;
        public MessageApiController(IUnitOfWork uw, IApplicationUserManager userManager)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
        }

        [HttpPost]
        public async Task<ApiResult<string>> SendMessage(string email, string description, int typeFeedBack)
        {
            if (email.HasValue() && description.HasValue() && typeFeedBack != 0)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Ok("ایمیل شما معتبر نیست!");
                if (user.IsActive == true && user.IsValidAccount == true)
                {
                    var messageUser = _uw.BaseRepository<MessageUser>().FindByConditionAsync(m => m.UserId == user.Id).Result.FirstOrDefault();
                    if (messageUser == null)
                    {
                        int maxId;
                        messageUser = new MessageUser();
                        var messageUsers = await _uw.BaseRepository<MessageUser>().FindAllAsync();
                        if (messageUsers.Count() != 0)
                            maxId = messageUsers.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
                        else
                            maxId = 1;
                        messageUser.Id = maxId;
                        messageUser.Email = email;
                        messageUser.UserId = user.Id;
                        messageUser.Description = description;
                        messageUser.Messages = (MessageType)typeFeedBack;
                        messageUser.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                        await _uw.BaseRepository<MessageUser>().CreateAsync(messageUser);
                    }
                    else
                    {
                        messageUser.UpdateTime = DateTime.Now;
                        messageUser.Description = description;
                        messageUser.Messages = (MessageType)typeFeedBack;
                        _uw.BaseRepository<MessageUser>().Update(messageUser);
                    }
                    await _uw.Commit();
                    return Ok(email);
                }
                else
                    return Ok("حساب کاربری شما غیر فعال است!");
            }
            return Ok("فیلدهای ورودی معتبر نیست!");
        }

        [HttpGet]
        public async Task<ApiResult<object>> ReciveAnswer(string email)
        {
            var customer = await _userManager.FindByEmailAsync(email);
            if (customer == null)
                return Ok();

            var messageUser = await _uw.BaseRepository<MessageUser>().FindByConditionAsync(m => m.UserId == customer.Id);
            if (messageUser.FirstOrDefault().Answer == null)
                return Ok("پاسخی داده نشده است");
            else
            {
                var author = await _uw.BaseRepository<User>().FindByConditionAsync(u => u.Id == messageUser.FirstOrDefault().User.Id);
                return Ok(new { answer = messageUser.FirstOrDefault().Answer, author = author.FirstOrDefault() } );
            }
        }
    }
}
