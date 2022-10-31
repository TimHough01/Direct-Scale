using System.Collections.Generic;
using TM3ClientExtension.ThirdParty.ZiplingoEngagement.Model;

namespace TM3ClientExtension.ThirdParty.ZiplingoEngagement.Interfaces
{
    public interface IZiplingoEngagementRepository
    {
        ZiplingoEngagementSettings GetSettings();
        void UpdateSettings(ZiplingoEngagementSettingsRequest settings);
        void ResetSettings();
        ShipInfo GetOrderNumber(int packageId);
        List<AssociateInfo> AssociateBirthdayWishesInfo();
        List<AssociateInfo> AssociateWorkAnniversaryInfo();
        AutoshipFromOrderInfo GetAutoshipFromOrder(int OrderNumber);
        string GetUsernameById(string associateId);
        string GetLastFoutDegitByOrderNumber(int orderId);
        string GetStatusById(int statusId);
        List<ZiplingoEventSettings> GetEventSettingsList();
        ZiplingoEventSettings GetEventSettingDetail(string eventKey);
        void UpdateEventSetting(ZiplingoEventSettingRequest request);
        List<ServiceInfo> GetServiceExpirationInfoBefore2Weeks();
    }
}
