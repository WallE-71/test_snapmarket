using System.Collections.Generic;
using SnapMarket.Entities.Identity;

namespace SnapMarket.ViewModels.DynamicAccess
{
    public class DynamicAccessIndexViewModel
    {
        //public DynamicAccessIndexViewModel(User userIncludeUserClaims, ICollection<ControllerViewModel> securedControllerActions)
        //{
        //    UserIncludeUserClaims = userIncludeUserClaims;
        //    SecuredControllerActions = securedControllerActions;
        //}


        public string ActionIds { set; get; }
        public int UserId { set; get; }

        public User UserIncludeUserClaims { set; get; }
        public ICollection<ControllerViewModel> SecuredControllerActions { set; get; }
    }
}
