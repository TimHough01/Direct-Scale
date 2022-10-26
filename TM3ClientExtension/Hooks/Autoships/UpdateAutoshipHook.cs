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
    public class UpdateAutoshipHook : IHook<UpdateAutoshipHookRequest, UpdateAutoshipHookResponse>
    {

        private IAssociateUpgradeService _associateUpgradeService;

        public UpdateAutoshipHook(IAssociateUpgradeService associateUpgradeService)
        {
            _associateUpgradeService = associateUpgradeService;
        }

        public async Task<UpdateAutoshipHookResponse> Invoke(UpdateAutoshipHookRequest request, Func<UpdateAutoshipHookRequest, Task<UpdateAutoshipHookResponse>> func)
        {
            var response = await func(request);
            await this._associateUpgradeService.UpgradeAssociate(request.AutoshipInfo.AssociateId);
            return response;

        }
    }
}