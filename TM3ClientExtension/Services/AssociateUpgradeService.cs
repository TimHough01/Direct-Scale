using System;
using System.Threading.Tasks;
using DirectScale.Disco.Extension.Services;
using System.Linq;
namespace TM3ClientExtension.Services
{
    public class AssociateUpgradeService : IAssociateUpgradeService
    {

        private IAssociateService _associateService;
        private IAutoshipService _autoshipService;

        public const int RETAIL_CUSTOMER = 2;
        public const int PREFERRED_CUSTOMER1 = 3;
        public const int PREFERRED_CUSTOMER2 = 4;

        public AssociateUpgradeService(IAssociateService associateService, IAutoshipService autoshipService)
        {
            _associateService = associateService;
            _autoshipService = autoshipService;
        }

        public async Task<bool> UpgradeAssociate(int associateId)
        {
            var associate = await this._associateService.GetAssociate(associateId);
            var associateType = associate.AssociateType;

            // If the user role is not in upgradeable role list
            if (!this.IsUpgradeableType(associateType))
            {
                return false;
            }

            var numOfAutoshipItems = await this.NumberOfAutoshipItems(associateId);
            var newType = this.AssociateNewType(associateType, numOfAutoshipItems);

            // If no upgrade/downgrade is needed
            if(newType == 0)
            {
                return false;
            }

            associate.AssociateBaseType = newType;
            await this._associateService.UpdateAssociate(associate);

            return true;
        }

        private Boolean IsUpgradeableType(int type)
        {
            if (type == AssociateUpgradeService.RETAIL_CUSTOMER || type == AssociateUpgradeService.PREFERRED_CUSTOMER1 || type == AssociateUpgradeService.PREFERRED_CUSTOMER2)
            {
                return true;
            }
            return false;
        }

        private int AssociateNewType(int type, int numOfAutoshipItems)
        {
            var newType = 0;
            if(type == AssociateUpgradeService.RETAIL_CUSTOMER)
            {
                if(numOfAutoshipItems == 1)
                {
                    newType = AssociateUpgradeService.PREFERRED_CUSTOMER1;
                }else if(numOfAutoshipItems > 1)
                {
                    newType = AssociateUpgradeService.PREFERRED_CUSTOMER2;
                }
            }else if (type == AssociateUpgradeService.PREFERRED_CUSTOMER1)
            {
                if (numOfAutoshipItems == 0)
                {
                    newType = AssociateUpgradeService.RETAIL_CUSTOMER; // Downdrage
                }
                else if (numOfAutoshipItems > 1)
                {
                    newType = AssociateUpgradeService.PREFERRED_CUSTOMER2;
                }
            }
            else if (type == AssociateUpgradeService.PREFERRED_CUSTOMER2)
            {
                if (numOfAutoshipItems == 0)
                {
                    newType = AssociateUpgradeService.RETAIL_CUSTOMER; // Downdrage
                }
                else if (numOfAutoshipItems == 1)
                {
                    newType = AssociateUpgradeService.PREFERRED_CUSTOMER1; // Downdrage
                }
            }

            
            return newType;
        }

        private async Task<int> NumberOfAutoshipItems(int associateId)
        {
            var autoships = await this._autoshipService.GetAutoships(associateId, false);
            
            var allLineItems = from autoship in autoships select autoship.LineItems.Sum(a => a.Quantity);
            return (int)Math.Round(allLineItems.Sum());
        }


    }

}

