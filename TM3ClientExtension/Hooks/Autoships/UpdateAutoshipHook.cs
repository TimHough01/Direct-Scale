using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectScale.Disco.Extension.Services;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Autoships;
using DirectScale.Disco.Extension;
using TM3ClientExtension.Services;
using TM3ClientExtension.ThirdParty.ZiplingoEngagement.Interfaces;

namespace ClientExtension.Hooks.Autoships
{
    public class UpdateAutoshipHook : IHook<UpdateAutoshipHookRequest, UpdateAutoshipHookResponse>
    {

        private IAssociateUpgradeService _associateUpgradeService;

        private IZiplingoEngagementService _ziplingoEngagementService;
        private IAssociateService _associateService;
        private IAutoshipService _autoshipService;

        public UpdateAutoshipHook(IAssociateUpgradeService associateUpgradeService, IZiplingoEngagementService ziplingoEngagementService, IAssociateService associateService, IAutoshipService autoshipService)
        {
            _associateUpgradeService = associateUpgradeService;
            _ziplingoEngagementService = ziplingoEngagementService ?? throw new ArgumentNullException(nameof(ziplingoEngagementService));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _autoshipService = autoshipService ?? throw new ArgumentNullException(nameof(autoshipService));
        }

        public async Task<UpdateAutoshipHookResponse> Invoke(UpdateAutoshipHookRequest request, Func<UpdateAutoshipHookRequest, Task<UpdateAutoshipHookResponse>> func)
        {
            var response = await func(request);
            // Removed the logic to upgrade
            //await this._associateUpgradeService.UpgradeAssociate(request.AutoshipInfo.AssociateId);

            var updatedAutoshipInfo = await _autoshipService.GetAutoship(request.AutoshipInfo.AutoshipId);
            _ziplingoEngagementService.UpdateAutoshipTrigger(updatedAutoshipInfo);
            var associateInfo = await _associateService.GetAssociate(request.AutoshipInfo.AssociateId);
            _ziplingoEngagementService.UpdateContact(associateInfo);

            return response;

        }
    }
}