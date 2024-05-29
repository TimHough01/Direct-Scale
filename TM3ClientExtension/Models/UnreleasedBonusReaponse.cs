using System;

namespace TM3ClientExtension.Models
{
    public class UnreleasedBonusReaponse
    {
        public string BonusId { get; set; }
        public string BonusTitle { get; set; }
        public string NodeId { get; set; }
        public long PeriodId { get; set; }
        public long CompensationPlanId { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public double Volume { get; set; }
        public double Percent { get; set; }
        public double Released { get; set; }
        public int Level { get; set; }
        public DateTime CommissionDate { get; set; }
        public bool IsFirstTimeBonus { get; set; }
        public string Rank { get; set; }
        public string CustomerType { get; set; }
    }
}
