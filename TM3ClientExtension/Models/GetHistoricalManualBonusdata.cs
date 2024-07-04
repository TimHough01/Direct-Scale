using System;

namespace TM3ClientExtension.Models
{
    public class GetHistoricalManualBonusdata
    {
        public string NodeId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Amount { get; set; }
        public string Comment { get; set; }
        public int AssociateRole { get; set; }
    }
}
