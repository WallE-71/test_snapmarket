using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnapMarket.Common.Api;
using SnapMarket.Common.Api.Attributes;
using SnapMarket.Data.Contracts;
using SnapMarket.Entities.Identity;
using SnapMarket.Services.Contracts;
using SnapMarket.ViewModels.Api;
using SnapMarket.ViewModels.DynamicAccess;

namespace SnapMarket.Areas.Api.Controllers
{
    [ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")/*, ApiExplorerSettings(IgnoreApi = false)*/]
    public class DynamicAccessController : Controller
    {
        private readonly IUnitOfWork _uw;
        public readonly IApplicationUserManager _userManager;
        public readonly IMvcActionsDiscoveryService _mvcActionsDiscovery;
        public DynamicAccessController(IUnitOfWork uw, IApplicationUserManager userManager, IMvcActionsDiscoveryService mvcActionsDiscovery)
        {
            _uw = uw;
            _userManager = userManager;
            _mvcActionsDiscovery = mvcActionsDiscovery;
        }


        [HttpGet("{userId}")/*, JwtAuthentication(Policy = ConstantPolicies.DynamicPermission)*/]
        public async Task<ApiResult<object>> Get(int userId)
        {
            if (userId == 0)
                return NotFound();

            var user = await _userManager.FindClaimsInUser(userId);
            if (user == null)
                return NotFound();

            var claims = await _userManager.GetClaimsAsync(user);
            return Ok(claims);
        }


        [HttpPost/*, JwtAuthentication(Policy = ConstantPolicies.DynamicPermission)*/]
        public async Task<ApiResult<object>> Add([FromBody] DynamicAccessAndClaimViewModel viewModel)
        {
            var user = await _userManager.FindClaimsInUser(viewModel.UserId);
            if (user == null)
                return BadRequest("کاربری پیدا نشد.");

            var claims = await _userManager.GetClaimsAsync(user);
            Claim claim = claims.Where(x => x.Type == viewModel.UserClaimType && x.Value == viewModel.UserClaimValue).FirstOrDefault();
            if (claim == null)
            {
                claim = new Claim(viewModel.UserClaimType, viewModel.UserClaimValue, ClaimValueTypes.String);
                IdentityResult result = await _userManager.AddClaimAsync(user, claim);

                if (result.Succeeded)
                    return BadRequest("درج کلیم جدید با موفقیت انجام شد.");
                else
                    BadRequest(result);
            }
            return BadRequest();
        }


        [HttpDelete/*, JwtAuthentication(Policy = ConstantPolicies.DynamicPermission)*/]
        public async Task<ApiResult<object>> Delete([FromBody] DynamicAccessAndClaimViewModel viewModel)
        {
            var user = await _userManager.FindClaimsInUser(viewModel.UserId);
            if (user == null)
                return BadRequest("کاربری پیدا نشد.");

            var claims = await _userManager.GetClaimsAsync(user);
            Claim claim = claims.Where(x => x.Type == viewModel.UserClaimType && x.Value == viewModel.UserClaimValue).FirstOrDefault();
            if (claim != null)
            {
                await _userManager.RemoveClaimAsync(user, claim);
                return BadRequest("حذف کلیم با موفقیت انجام شد.");
            }
            else
                return BadRequest();
        }
    }
}
