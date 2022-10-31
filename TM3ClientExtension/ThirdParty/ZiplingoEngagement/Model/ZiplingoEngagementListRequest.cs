using System;
using System.Collections.Generic;

namespace TM3ClientExtension.ThirdParty.ZiplingoEngagement.Model
{
    public class ZiplingoEngagementListRequest
    {
        public string eventKey { get; set; }
        public string companyname { get; set; }
        public List<AssociateDetail> dataList { get; set; }
    }
}
