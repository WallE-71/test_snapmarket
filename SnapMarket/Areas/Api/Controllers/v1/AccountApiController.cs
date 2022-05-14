using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using SnapMarket.Entities;
using SnapMarket.Common.Api;
using SnapMarket.ViewModels;
using SnapMarket.Data.Contracts;
using SnapMarket.Entities.Identity;
using SnapMarket.Common.Extensions;
using SnapMarket.Services.Contracts;
using SnapMarket.ViewModels.Api.Users;
using SnapMarket.ViewModels.Api.SignIn;
using SnapMarket.Common.Api.Attributes;
using SnapMarket.Services.Api.Contract;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [DisplayName("AccountApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class AccountApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IJwtService _jwtService;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _accessor;
        private readonly ISignInOption<User> _signInOption;
        private readonly SignInManager<User> _signInManager;
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationRoleManager _roleManager;
        private readonly ILogger<AccountApiController> _logger;
        public AccountApiController(IUnitOfWork uw, SignInManager<User> signInManager, IHttpContextAccessor accessor, IApplicationUserManager userManager, IApplicationRoleManager roleManager, IEmailSender emailSender, IMapper mapper, ILogger<AccountApiController> logger, IJwtService jwtService, ISignInOption<User> signInOption)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _logger = logger;
            _logger.CheckArgumentIsNull(nameof(_logger));
            _accessor = accessor;
            _accessor.CheckArgumentIsNull(nameof(_accessor));
            _jwtService = jwtService;
            _jwtService.CheckArgumentIsNull(nameof(_jwtService));
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
            _roleManager = roleManager;
            _roleManager.CheckArgumentIsNull(nameof(_roleManager));
            _emailSender = emailSender;
            _emailSender.CheckArgumentIsNull(nameof(_emailSender));
            _signInOption = signInOption;
            _signInOption.CheckArgumentIsNull(nameof(_signInOption));
            _signInManager = signInManager;
            _signInManager.CheckArgumentIsNull(nameof(_signInManager));
        }

        [HttpPost("SendCode")]
        public async Task<ApiResult<string>> SendCode(string email)
        {
            if (!email.HasValue() && !email.IsValidEmail())
                return BadRequest();

            var code = StringExtensions.GenerateId(5);
            var Message = "<p style='direction:rtl; font-size:14px; font-family:tahoma'>کد اعتبارسنجی شما :" + code + "</p>";
            await _emailSender.SendEmailAsync(email, "کد اعتبارسنجی", Message);// need: google -> Allow less secure apps: ON    // link => https://myaccount.google.com/lesssecureapps?pli=1&rapt=AEjHL4Omcm86-NTHT7zOzqXU4d5yoQk4FYQLORKtApUfpDvz4ybLOtJz8nthJeMNi8FwDIcZv6AB_QPuZN8E4V4viZDE-Z-OdA

            return Ok(code);
        }

        //[Post] api/v1/AccountApi
        [HttpPost]
        public async Task<ApiResult<UserDto>> RegisterOrSignIn(string phoneNumber, string browserId)
        {
            if (!phoneNumber.HasValue() && !phoneNumber.IsValidPhoneNumber())
                return BadRequest();

            var guid = new Guid();
            if (browserId == "null")
            {
                var value = Guid.NewGuid().ToString();
                Guid.TryParse(value, out guid);
            }

            var findCustomer = await _userManager.FindByPhoneNumberAsync(phoneNumber);
            if (findCustomer != null) // SignIn, Login
            {
                if (findCustomer.IsActive)
                {
                    var result = await _signInOption.PhoneNumberSignInAsync(findCustomer, phoneNumber, true);
                    if (result.Succeeded)
                    {
                        var token = _jwtService.GenerateTokenAsync(findCustomer);
                        if (token != null)
                            return new UserDto { token = token, browserId = browserId != "null" ? browserId : guid.ToString() };
                        else return null;
                    }
                    else if (result.IsLockedOut)
                        return BadRequest("حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش های ناموفق قفل شد.");
                    else
                    {
                        _logger.LogWarning($"The user attempts to login with the IP address({_accessor.HttpContext?.Connection?.RemoteIpAddress.ToString()}) and phoneNumber ({phoneNumber}).");
                        return NotFound("شماره همراه شما صحیح نمی باشد.");
                    }
                }
                else
                    return BadRequest("حساب کاربری شما غیرفعال است.");
            }
            else // SignUp, Register
            {
                var customer = new User { PhoneNumber = phoneNumber, UserName = phoneNumber, Email = phoneNumber + "@snapmarket.ir", FirstName = "", LastName = "", Address = "", InsertTime = DateTime.Now, IsActive = true };
                IdentityResult result = await _userManager.CreateAsync(customer, phoneNumber);
                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByNameAsync("مشتری");
                    if (role == null)
                        await _roleManager.CreateAsync(new Role("مشتری"));

                    result = await _userManager.AddToRoleAsync(customer, "مشتری");
                    if (result.Succeeded)
                    {
                        customer.EmailConfirmed = true;
                        customer.LockoutEnabled = false;
                        customer.TwoFactorEnabled = true;
                        customer.PhoneNumberConfirmed = true;
                        await _userManager.UpdateAsync(customer);
                        var token = _jwtService.GenerateTokenAsync(await _userManager.FindByIdAsync(customer.Id.ToString()));
                        if (token != null)
                            return new UserDto { token = token, browserId = guid.ToString() };
                    }
                }
                foreach (var item in result.Errors)
                    return BadRequest(item.Description);
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<ApiResult<object>> EditInfo(string phoneNumber, string email, string firstName, string lastName, string address)
        {
            IdentityResult result = null;
            var users = await _uw.BaseRepository<User>().FindByConditionAsync(u => u.PhoneNumber == phoneNumber);
            if (users.Count() != 0)
            {
                users.FirstOrDefault().Email = email;
                users.FirstOrDefault().Address = address;
                users.FirstOrDefault().LastName = lastName;
                users.FirstOrDefault().FirstName = firstName;
                users.FirstOrDefault().PhoneNumber = phoneNumber;
                result = await _userManager.UpdateAsync(users.FirstOrDefault());
            }
            else
                return NotFound();

            if (result.Succeeded)
            {
                var newToken = _jwtService.GenerateTokenAsync(users.FirstOrDefault());
                if (newToken != null)
                    return new UserDto
                    {
                        token = newToken,
                        browserId = null
                    };
            }
            return BadRequest();
        }

        [HttpPost("CreditCart")]
        public async Task<IActionResult> CreditCart(string phoneNumber, int credit, string nationalId, string bankCode, bool getCart)
        {
            var user = await _userManager.FindByPhoneNumberAsync(phoneNumber);
            var cart = await _uw.BaseRepository<CreditCart>().FindByConditionAsync(u => u.UserId == user.Id);
            if (cart.Count() != 0)
            {
                if (getCart) return Ok(cart.FirstOrDefault());
                cart.FirstOrDefault().Credit = credit;
                cart.FirstOrDefault().UserId = user.Id;
                cart.FirstOrDefault().BankCode = bankCode;
                cart.FirstOrDefault().Owner = user.UserName;
                cart.FirstOrDefault().NationalId = nationalId;
                cart.FirstOrDefault().UpdateTime = DateTime.Now;
                _uw.BaseRepository<CreditCart>().Update(cart.FirstOrDefault());
            }
            else
            {
                if (getCart) return NotFound();
                var newCart = new CreditCart();
                newCart.Credit = credit;
                newCart.UserId = user.Id;
                newCart.BankCode = bankCode;
                newCart.Owner = user.UserName;
                newCart.NationalId = nationalId;
                newCart.InsertTime = DateTime.Now;
                await _uw.BaseRepository<CreditCart>().CreateAsync(newCart);
            }
            await _uw.Commit();
            return Ok(credit);
        }

        public class UserDto
        {
            public string browserId { get; set; }
            public AuthenticationDto token { get; set; }
        }

        //[HttpGet]
        //public virtual async Task<ApiResult<object>> GetUserByJWT()
        //{
        //    //var publicKey = @"{""a"":""1""}""{""b"":""2""}""{""c"":""3""}""{""d"":""4""}""{""e"":""5""}""{""f"":""6""}";

        //    var publicKey = @"-----BEGIN RSA PUBLIC KEY-----
        //                    MIIBCgKCAQEAn4XOc6lV0LZ5j+dBCRH2eiDj6fGlzMIJ7gmSUBF++xLLLAP/Espq
        //                    uIMpTSRJFgrg29euExYNVA+DKDn45ckAXnWar/1JLQdWfz+8ybdUH8mAt9omZStv
        //                    jfVbqS1/kyBBOymo2LZ3BZCuVRR/kiZ3xuwY06VhgKOcCJR8YQjW5hX+U9Ovl0fL
        //                    lE4C1a32GBGkcNU7GTrS4aBlciAtALmRLbU+0rr+XJECYWb7/SFfYaM0qAa9kw6F
        //                    YCfatXclHm2qLaOo8mwlsAdQPpCVyW7R/RrdLgLLkkmzeJacLgjFTLyb894t0Y9/
        //                    4fHy+L+FAmC+Rceka9ZpCb+/V6IcAZDj+QIDAQAB
        //                      -----END RSA PUBLIC KEY-----";

        //    var claims = new ClaimsPrincipal();
        //    var handler = new JwtSecurityTokenHandler();
        //    var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
        //    using (RSA rsa = RSA.Create())
        //    {
        //        try
        //        {
        //            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        //            {
        //                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        //            };

        //            var signature = Convert.FromBase64String(publicKey);
        //            rsa.ImportSubjectPublicKeyInfo(signature, out _);
        //            var validations = new TokenValidationParameters
        //            {
        //                ValidateIssuer = true,
        //                ValidateAudience = true,
        //                ValidateLifetime = true,
        //                ValidateIssuerSigningKey = true,
        //                ValidIssuer = "http://localhost:4200",
        //                ValidAudience = "http://localhost:4200",
        //                IssuerSigningKey = new RsaSecurityKey(rsa),
        //                CryptoProviderFactory = new CryptoProviderFactory()
        //                {
        //                    CacheSignatureProviders = false
        //                },
        //            };

        //            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
        //            var test = new
        //            {
        //                FirstName = jwtToken.Claims.First(claim => claim.Type == "FirstName").Value,
        //                LastName = jwtToken.Claims.First(claim => claim.Type == "LastName").Value,
        //                Email = jwtToken.Claims.First(claim => claim.Type == "Email").Value
        //            };

        //            claims = handler.ValidateToken(token, validations, out var validatedSecurityToken);
        //        }
        //        catch (Exception e)
        //        {
        //            return e.Message;
        //        }

        //        return claims.Identity.Name;
        //    }
        //}
    }
}
