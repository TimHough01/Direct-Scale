using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectScale.Disco.Extension.Services;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Autoships;
using DirectScale.Disco.Extension;
using TM3ClientExtension.Services;

namespace ClientExtension.Hooks.Autoships
{
    public class CancelAutoshipHook : IHook<CancelAutoshipHookRequest, CancelAutoshipHookResponse>
    {

        private IAssociateUpgradeService _associateUpgradeService;

        public CancelAutoshipHook(IAssociateUpgradeService associateUpgradeService)
        {
            _associateUpgradeService = associateUpgradeService;
        }

        public async Task<CancelAutoshipHookResponse> Invoke(CancelAutoshipHookRequest request, Func<CancelAutoshipHookRequest, Task<CancelAutoshipHookResponse>> func)
        {
            var response = await func(request);
            // Removed the logic to upgrade
            //await this._associateUpgradeService.UpgradeAssociate(request.AutoshipInfo.AssociateId);

            return response;
        }

    }
}