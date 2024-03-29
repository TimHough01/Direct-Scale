﻿using DirectScale.Disco.Extension;
using System;

namespace TM3ClientExtension.ThirdParty.ZiplingoEngagement.Model
{
    public class AutoshipInfoMap
    {
        public double TotalQV { get; set; }
        public double TotalCV { get; set; }
        public string FrequencyString { get; set; }
        public LineItem[] LineItems { get; set; }
        public string AutoshipType { get; set; }
        public CustomFields Custom { get; set; }
        public int PaymentMerchantId { get; set; }
        public string PaymentMethodId { get; set; }
        public string CurrencyCode { get; set; }
        public int ShipMethodId { get; set; }
        public double LastChargeAmount { get; set; }
        public DateTime NextProcessDate { get; set; }
        public DateTime LastProcessDate { get; set; }
        public string Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public Address ShipAddress { get; set; }
        public int AssociateId { get; set; }
        public int AutoshipId { get; set; }
        public string Status { get; set; }
        public double SubTotal { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string ProductNames { get; set; }
	}
}
