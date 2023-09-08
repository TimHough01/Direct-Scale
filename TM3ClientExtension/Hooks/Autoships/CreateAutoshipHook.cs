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
    public class CreateAutoshipHook : IHook<CreateAutoshipHookRequest, CreateAutoshipHookResponse>
    {

        private IAssociateUpgradeService _associateUpgradeService;
        private IZiplingoEngagementService _ziplingoEngagementService;
        private IAssociateService _associateService;
        private readonly IAutoshipService _autoshipService;

        public CreateAutoshipHook(IAssociateUpgradeService associateUpgradeService, IZiplingoEngagementService ziplingoEngagementService, IAssociateService associateService, IAutoshipService autoshipService)
        {
            _associateUpgradeService = associateUpgradeService;
            _ziplingoEngagementService = ziplingoEngagementService ?? throw new ArgumentNullException(nameof(ziplingoEngagementService));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _autoshipService = autoshipService;
        }

        public async Task<CreateAutoshipHookResponse> Invoke(CreateAutoshipHookRequest request, Func<CreateAutoshipHookRequest, Task<CreateAutoshipHookResponse>> func)
        {
            var response = await func(request);
            // Removed the logic to upgrade
            //await this._associateUpgradeService.UpgradeAssociate(request.AutoshipInfo.AssociateId);

            var autoshipInfo = await _autoshipService.GetAutoship(response.AutoshipId);
            _ziplingoEngagementService.CreateAutoshipTrigger(autoshipInfo);
            var associateInfo = await _associateService.GetAssociate(autoshipInfo.AssociateId);
            _ziplingoEngagementService.UpdateContact(associateInfo);

            return response;
        }
    }
}