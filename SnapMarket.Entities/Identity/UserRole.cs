using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SnapMarket.Entities.Identity
{
    public class UserRole : IdentityUserRole<int>
    {
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
