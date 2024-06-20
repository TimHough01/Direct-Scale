using System;

namespace TM3ClientExtension.Models
{
    public class UnreleasedBonusReaponse
    {
        public DateTime date { get; set; }
        public int nodeId { get; set; }
        public string comment { get; set; }
        public double amount { get; set; }
        public int associateRole { get; set; }
    }
}
