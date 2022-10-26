using System;
using System.Threading.Tasks;
namespace TM3ClientExtension.Services
{
    public interface IAssociateUpgradeService
    {
        Task<Boolean> UpgradeAssociate(int associateId);
    }
}

