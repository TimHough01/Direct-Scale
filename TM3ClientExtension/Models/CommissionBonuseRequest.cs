using System.Collections.Generic;
using System;

namespace TM3ClientExtension.Models
{
    public class CommissionBonuseRequest
    {
        public DateTime Date { get; set; }

        public List<string> NodeIds { get; set; }

        public string NodeId { get; set; }

        public int Offset { get; set; }

        public int Count { get; set; }
    }
    public class CommissionImportRequest
    {
        public DateTime Date { get; set; }

        public string NodeIds { get; set; }

        public string comment { get; set; }

        public double amount { get; set; }
    }
}
