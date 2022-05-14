using SnapMarket.ViewModels.DynamicAccess;
using System.Collections.Generic;

namespace SnapMarket.Services.Contracts
{
    public interface IMvcActionsDiscoveryService
    {
        ICollection<ControllerViewModel> GetAllSecuredControllerActionsWithPolicy(string policyName);
    }
}
