using Microsoft.AspNetCore.Identity;

namespace SnapMarket.Entities.Identity
{
    public class UserClaim : IdentityUserClaim<int>
    {
        public virtual User User { get; set; }
    }

    public class RoleClaim : IdentityRoleClaim<int>
    {
        public virtual Role Role { get; set; }
    }
}
