using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SnapMarket.Common.Extensions;
using SnapMarket.Entities.Identity;
using SnapMarket.Services.Contracts;
using SnapMarket.ViewModels.Manage;

namespace SnapMarket.Areas.Admin.Controllers
{
    public class ManageController : BaseController
    {
        private readonly IApplicationRoleManager _roleManager;
        private readonly IApplicationUserManager _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ManageController> _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public ManageController(IApplicationRoleManager roleManager,
            IApplicationUserManager userManager,
            SignInManager<User> signInManager,
            ILogger<ManageController> logger,
            IHttpContextAccessor accessor,
            IMapper mapper,
            IWebHostEnvironment env)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _accessor = accessor;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult SignIn(string ReturnUrl = null)
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByNameAsync(ViewModel.UserName);

                if (User != null)
                {
                    if (User.IsActive)
                    {
                        var result = await _signInManager.PasswordSignInAsync(ViewModel.UserName, ViewModel.Password, ViewModel.RememberMe, true);
                        if (result.Succeeded)
                            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                        else if (result.IsLockedOut)
                            ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش های ناموفق قفل شد.");
                        else if (result.RequiresTwoFactor)
                            return RedirectToAction("SendCode", new { RememberMe = ViewModel.RememberMe });
                        else
                        {
                            ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور شما صحیح نمی باشد.");
                            _logger.LogWarning($"The user attempts to login with the IP address({_accessor.HttpContext?.Connection?.RemoteIpAddress.ToString()}) and username ({ViewModel.UserName}) and password ({ViewModel.Password}).");
                        }
                    }
                    else
                        ModelState.AddModelError(string.Empty, "حساب کابری شما غیرفعال است.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور شما صحیح نمی باشد.");
                    _logger.LogWarning($"The user attempts to login with the IP address({_accessor.HttpContext?.Connection?.RemoteIpAddress.ToString()}) and username ({ViewModel.UserName}) and password ({ViewModel.Password}).");
                }
            }
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut(string ReturnUrl = null)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn", "Manage", new { area = "Admin" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = viewModel.UserName, Email = viewModel.Email, InsertTime = DateTime.Now, FirstName = "", LastName = "" };
                IdentityResult result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByNameAsync("کاربر");
                    if (role == null)
                        await _roleManager.CreateAsync(new Role("کاربر"));

                    result = await _userManager.AddToRoleAsync(user, "کاربر");
                    if (result.Succeeded)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", values: new { userId = user.Id, code = code }, protocol: Request.Scheme);
                        TempData["notification"] = $" ایمیل فعال سازی حساب کاربری به {viewModel.Email} ارسال شد. ";
                    }
                }
                ModelState.AddErrorsFromResult(result);
            }
            return PartialView("_Register");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User); // اطلاعات کاربری که لاگین کرده
            if (user == null)
                return NotFound();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel ViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                var changePassResult = await _userManager.ChangePasswordAsync(user, ViewModel.OldPassword, ViewModel.NewPassword);
                if (changePassResult.Succeeded)
                    ViewBag.Alert = "کلمه عبور شما با موفقیت تغییر یافت.";
                else
                    ModelState.AddErrorsFromResult(changePassResult);
            }
            return View(ViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int? userId)
        {
            var loginUserId = User.Identity.GetUserId<int>();
            if (loginUserId == userId)
            {
                var profileViewModel = new ProfileViewModel();
                if (userId == null)
                    return NotFound();
                else
                {
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    if (user == null)
                        return NotFound();
                    else
                    {
                        profileViewModel = _mapper.Map<ProfileViewModel>(user);
                        profileViewModel.PersianBirthDate = profileViewModel.BirthDate.ConvertMiladiToShamsi("yyyy/MM/dd");
                    }
                }
                return View(profileViewModel);
            }
            else
                return View("AccessDenied");
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel viewModel)
        {
            if (viewModel.Id == null)
                return NotFound();
            else
            {
                var user = await _userManager.FindByIdAsync(viewModel.Id.ToString());
                if (user == null)
                    return NotFound();
                else
                {
                    if (viewModel.ImageFile != null)
                    {
                        viewModel.Image = viewModel.ImageFile.FileName;
                        await viewModel.ImageFile.UploadFileAsync($"{_env.WebRootPath}/avatars/{viewModel.Image}");
                    }
                    else
                        viewModel.Image = user.Image;

                    viewModel.BirthDate = viewModel.PersianBirthDate.ConvertShamsiToMiladi();
                    var result = await _userManager.UpdateAsync(_mapper.Map(viewModel, user));

                    if (result.Succeeded)
                        ViewBag.Alert = EditSuccess;
                    else
                        ModelState.AddErrorsFromResult(result);
                }
                return View(viewModel);
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetExternalLoginProvider(string provider)
        {
            //if (provider == "Yahoo")
            //{
            //    var client_id = "dj0yJmk9aWxnZVZNTGVwVXhWJnM9Y29uc3VtZXJzZWNyZXQmc3Y9MCZ4PWQz";

            //    //for host
            //    //var redirectUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/signin-yahoo";
            //    return Redirect($"https://api.login.yahoo.com/oauth2/request_auth?client_id={client_id}&redirect_uri=https://c4aedefa.ngrok.io/yahoo-signin&response_type=code&language=en-us");
            //}
            //if (provider == "Google")
            //{
            //    var client_id = "192698895411-0p7k76u2rbehj2hpvvae1a2ddda91e3m.apps.googleusercontent.com";

            //    //for host
            //    //var redirectUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/signin-yahoo";
            //    return Redirect($"https://api.login.yahoo.com/oauth2/request_auth?client_id={client_id}&redirect_uri=https://c4aedefa.ngrok.io/yahoo-signin&response_type=code&language=en-us");
            //}
            var RedirectUrl = Url.Action("GetCallBackAsync", "Account");
            var Properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, RedirectUrl);
            return Challenge(Properties, provider);
        }

        public async Task<IActionResult> GetCallBackAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            var Result = await ExternalLoginAsync(info);
            if (Result == "success")
                return RedirectToAction("Index", "Home");
            else if (Result == "requiresTwoFactor")
                return RedirectToAction("SendCode");
            else
            {
                ModelState.AddModelError(string.Empty, Result);
                return View("SignIn");
            }
        }

        public async Task<string> ExternalLoginAsync(ExternalLoginInfo info)
        {
            if (info == null)
                return $"در عملیات ورود به سایت از طریق حساب {info.ProviderDisplayName} خطایی رخ داده است. ";

            var UserEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(UserEmail);
            if (user == null)
                return "شما عضو سایت نیستید برای ورود به سایت ابتدا باید عضو سایت شوید.";
            else
            {
                var Result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
                if (Result.Succeeded)
                    return "success";
                else if (Result.IsLockedOut)
                    return "حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش های ناموفق قفل شد.";
                else if (Result.RequiresTwoFactor)
                    return "requiresTwoFactor";
                else
                {
                    var ExternalResult = await _userManager.AddLoginAsync(user, info);
                    if (ExternalResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return "success";
                    }
                    else
                        return $"در عملیات ورود به سایت از طریق حساب {info.ProviderDisplayName} خطایی رخ داده است. ";
                }
            }
        }
    }
}
