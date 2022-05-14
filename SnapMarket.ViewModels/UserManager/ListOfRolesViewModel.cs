using System.Collections.Generic;
using SnapMarket.Entities.Identity;

namespace SnapMarket.ViewModels
{
    public class ListOfRolesViewModel
    {
        public ListOfRolesViewModel(List<Role> roles, int[] roleIds)
        {
            Roles = roles;
            RoleIds = roleIds;
        }

        public int[] RoleIds { get; set; }
        public List<Role> Roles { get; set; }
    }
}
