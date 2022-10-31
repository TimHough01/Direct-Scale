using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks.Commissions;
using System.Collections.Generic;
using TM3ClientExtension.ThirdParty.ZiplingoEngagement.Model;

namespace TM3ClientExtension.ThirdParty.ZiplingoEngagement.Interfaces
{
    public interface IZiplingoEngagementService
    {
        void CallOrderZiplingoEngagementTrigger(Order order, string eventKey, bool FailedAutoship);
        void CreateEnrollContact(Order order);
        void CreateContact(Application req, ApplicationResponse response);
        void UpdateContact(Associate req);
        void ResetSettings(CommandRequest commandRequest);
        void SendOrderShippedEmail(int packageId, string trackingNumber);
        void AssociateBirthDateTrigger();
        void AssociateWorkAnniversaryTrigger();
        EmailOnNotificationEvent OnNotificationEvent(NotificationEvent notification);
        void FiveDayRunTrigger(List<AutoshipInfo> autoships);
        void AssociateStatusChangeTrigger(int associateId, int oldStatusId, int newStatusId);
        void ExpirationCardTrigger(List<CardInfo> cardinfo);
        LogRealtimeRankAdvanceHookResponse LogRealtimeRankAdvanceEvent(LogRealtimeRankAdvanceHookRequest req);
        void UpdateAssociateType(int associateId, string oldAssociateType, string newAssociateType, int newAssociateTypeId);
        void ExecuteCommissionEarned();
        void CreateAutoshipTrigger(Autoship autoshipInfo);
        void SentNotificationOnServiceExpiryBefore2Weeks();
        void AssociateStatusSync(List<GetAssociateStatusModel> associateStatuses);
        void UpdateAutoshipTrigger(Autoship updatedAutoshipInfo);
    }
}
