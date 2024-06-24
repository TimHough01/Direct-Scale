using System;

namespace TM3ClientExtension.Models
{
    public class HistoricalValuesRequest
    {
        public string key {  get; set; }
        public int periodId { get; set; }
        public string nodeId { get; set; }
        public double sumValue { get; set; }
        public string lastValue { get; set; }
        public DateTime postDate { get; set; }
    }
    public class Periods
    {
        public int Id { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public int CompensationPlanId { get; set; }
        public string Status { get; set; }
        public int? SnapshotId { get; set; }
    }
    public class HistoricalValues
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public double SumValue { get; set; }
        public int AssociateId { get; set; }
        public string LastValue { get; set; } = string.Empty;
        public string Key { get; set; }
        public int AssociateType { get; set; }
        public DateTime postDate { get; set; }
        public string PeriodName { get; set; }

    }
}
