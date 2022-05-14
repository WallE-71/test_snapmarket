using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SnapMarket.Entities;
using SnapMarket.Common.Api;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.Services.Contracts;
using SnapMarket.Common.Api.Attributes;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [DisplayName("NewsLetterApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class NewsLetterApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        private readonly IApplicationUserManager _userManager;
        public NewsLetterApiController(IUnitOfWork uw, IApplicationUserManager userManager)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
        }

        [HttpPost]
        public async Task<ApiResult<string>> SendMessage(string email)
        {
            if (email.HasValue())
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return BadRequest("ایمیل شما معتبر نیست, ابتدا وارد شوید!");
                if (user.IsActive == true)
                {
                    int maxId;
                    var newsletter = _uw.Context.Newsletters.Where(n => n.Email == email).FirstOrDefault();
                    if (newsletter == null)
                        newsletter = new Newsletter();
                    else
                    {
                        newsletter.UpdateTime = DateTime.Now;
                        _uw.BaseRepository<Newsletter>().Update(newsletter);
                        await _uw.Commit();
                        return Ok("شما عضو خبرنامه هستید.");
                    }

                    var newsletters = await _uw.BaseRepository<Newsletter>().FindAllAsync();
                    if (newsletters.Count() != 0)
                        maxId = newsletters.OrderByDescending(c => c.Id).First().Id + 1;
                    else
                        maxId = 1;
                    newsletter.Id = maxId;
                    newsletter.Email = email;
                    newsletter.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd");
                    await _uw.BaseRepository<Newsletter>().CreateAsync(newsletter);
                    await _uw.Commit();
                    return Ok("عضویت شما در خبرنامه با موفقیت انجام شد.");
                }
                else
                    return BadRequest("حساب کاربری شما غیر فعال است!");
            }
            return BadRequest();
        }
    }
}
