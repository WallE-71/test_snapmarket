using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SnapMarket.Entities.Identity;
using SnapMarket.Common.Extensions;
using SnapMarket.Services.Contracts;
using SnapMarket.ViewModels.UserManager;

namespace SnapMarket.Services.Identity
{
    public class ApplicationUserManager : UserManager<User>, IApplicationUserManager
    {
        private readonly IMapper _mapper;
        private readonly IServiceProvider _services;
        private readonly IUserStore<User> _userStore;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly IOptions<IdentityOptions> _options;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<ApplicationUserManager> _logger;
        private readonly ApplicationIdentityErrorDescriber _errors;
        private readonly IEnumerable<IUserValidator<User>> _userValidators;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        public ApplicationUserManager(
            ApplicationIdentityErrorDescriber errors,
            ILookupNormalizer keyNormalizer,
            ILogger<ApplicationUserManager> logger,
            IOptions<IdentityOptions> options,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            IServiceProvider services,
            IUserStore<User> userStore,
            IEnumerable<IUserValidator<User>> userValidators,
            IMapper mapper)
            : base(userStore, options, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _mapper = mapper;
            _errors = errors;
            _logger = logger;
            _options = options;
            _services = services;
            _userStore = userStore;
            _keyNormalizer = keyNormalizer;
            _passwordHasher = passwordHasher;
            _userValidators = userValidators;
            _passwordValidators = passwordValidators;
        }

        public async Task<UsersViewModel> FindUserWithRolesByIdAsync(int userId)
        {
            return await Users.Where(u => u.Id == userId).Select(user => new UsersViewModel
            {
                Id = user.Id,
                Image = user.Image,
                Email = user.Email,
                Roles = user.Roles,
                Gender = user.Gender,
                CityId = user.CityId,
                Address = user.Address,
                UserName = user.UserName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                BirthDate = user.BirthDate,
                FirstName = user.FirstName,
                LockoutEnd = user.LockoutEnd,
                InsertTime = user.InsertTime,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                TwoFactorEnabled = user.TwoFactorEnabled,
                AccessFailedCount = user.AccessFailedCount,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                _rolesId = user.Roles.Select(ur => ur.Role.Id).ToList(),
                _rolesName = user.Roles.Select(ur => ur.Role.Name).ToList(),
                RoleId = user.Roles.Select(ur => ur.Role.Id).FirstOrDefault(),
                RoleName = user.Roles.Select(ur => ur.Role.Name).FirstOrDefault(),
            }).FirstOrDefaultAsync();
        }

        public async Task<List<UsersViewModel>> GetPaginateUsersAsync(int offset, int limit, string orderBy, string searchText)
        {
            var getDateTimesForSearch = searchText.GetDateTimeForSearch();
            var users = await Users.Include(u => u.Roles)
                  .Where(t => t.FirstName.Contains(searchText) || t.LastName.Contains(searchText)
                  || t.Email.Contains(searchText) || t.UserName.Contains(searchText)
                  || (t.InsertTime >= getDateTimesForSearch.First() && t.InsertTime <= getDateTimesForSearch.Last()))
                  .OrderBy(orderBy).Skip(offset).Take(limit)
                  .Select(user => new UsersViewModel
                  {
                      Id = user.Id,
                      Bio = user.Bio,
                      Email = user.Email,
                      Image = user.Image,
                      Roles = user.Roles,
                      CityId = user.CityId,
                      Address = user.Address,
                      UserName = user.UserName,
                      LastName = user.LastName,
                      IsActive = user.IsActive,
                      FirstName = user.FirstName,
                      PhoneNumber = user.PhoneNumber.En2Fa(),
                      GenderName = user.Gender == GenderType.Male ? "مرد" : "زن",
                      _rolesId = user.Roles.Select(ur => ur.Role.Id).ToList(),
                      _rolesName = user.Roles.Select(ur => ur.Role.Name).ToList(),
                      RoleId = user.Roles.Select(ur => ur.Role.Id).FirstOrDefault(),
                      RoleName = user.Roles.Select(ur => ur.Role.Name).FirstOrDefault(),
                      PersianBirthDate = user.BirthDate.DateTimeEn2Fa("yyyy/MM/dd"),
                      PersianInsertTime = user.InsertTime.DateTimeEn2Fa("yyyy/MM/dd ساعت HH:mm:ss"),
                  }).AsNoTracking().ToListAsync();

            foreach (var item in users)
                item.Row = ++offset;

            return users;
        }

        public string CheckAvatarFileName(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName);
            int fileNameCount = Users.Where(f => f.Image == fileName).Count();
            int j = 1;

            while (fileNameCount != 0)
            {
                fileName = fileName.Replace(fileExtension, "") + j + fileExtension;
                fileNameCount = Users.Where(f => f.Image == fileName).Count();
                j++;
            }

            return fileName;
        }

        //public async Task<User> FindByPhoneNumberAsync(string phoneNumber) => await Users.Where(u => u.PhoneNumber == phoneNumber).FirstOrDefaultAsync();

        public async Task<User> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await Users.Where(u => u.PhoneNumber == phoneNumber).Select(user => new User
            {
                Id = user.Id,
                Bio = user.Bio,
                Image = user.Image,
                Email = user.Email,
                Claims = user.Claims,
                Gender = user.Gender,
                Orders = user.Orders,
                Address = user.Address,
                UserName = user.UserName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                BirthDate = user.BirthDate,
                FirstName = user.FirstName,
                Roles = user.Roles,
                PhoneNumber = user.PhoneNumber,
                SecurityStamp = user.SecurityStamp,
                InsertTime = user.InsertTime,
                PasswordHash = user.PasswordHash,
                NormalizedEmail = user.NormalizedEmail,
                ConcurrencyStamp = user.ConcurrencyStamp,
                NormalizedUserName = user.NormalizedUserName,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                AccessFailedCount = user.AccessFailedCount,
                TwoFactorEnabled = user.TwoFactorEnabled,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
            }).FirstOrDefaultAsync();
        }

        public async Task<IdentityResult> AddOrUpdateClaimsAsync(int userId, string userClaimType, IList<string> selectedUserClaimValues)
        {
            var user = await FindClaimsInUser(userId);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "NotFound",
                    Description = "کاربر مورد نظر یافت نشد.",
                });
            }

            var CurrentUserClaimValues = user.Claims.Where(r => r.ClaimType == userClaimType).Select(r => r.ClaimValue).ToList();

            if (selectedUserClaimValues == null)
                selectedUserClaimValues = new List<string>();

            var newClaimValuesToAdd = selectedUserClaimValues.Except(CurrentUserClaimValues).ToList(); // بدست آوردن کلیم هایی که باید به کاربر اضافه شوند
            foreach (var claim in newClaimValuesToAdd)
            {
                user.Claims.Add(new UserClaim
                {
                    UserId = userId,
                    ClaimType = userClaimType,
                    ClaimValue = claim,
                });
            }

            var removedClaimValues = CurrentUserClaimValues.Except(selectedUserClaimValues).ToList(); // بدیت آوردن کلیم هایی که باید از کاربر حذف شوند
            foreach (var claim in removedClaimValues)
            {
                var roleClaim = user.Claims.SingleOrDefault(r => r.ClaimValue == claim && r.ClaimType == userClaimType);

                if (roleClaim != null)
                    user.Claims.Remove(roleClaim);
            }
            return await UpdateAsync(user);
        }

        public bool CheckPhoneNumber(string phoneNumber) => Users.Any(u => u.PhoneNumber == phoneNumber);

        public Task<User> FindClaimsInUser(int userId) => Users.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == userId);
    }
}
