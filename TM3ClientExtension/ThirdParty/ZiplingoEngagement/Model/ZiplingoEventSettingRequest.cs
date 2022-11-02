using System;
using TM3ClientExtension.ThirdParty.ZiplingoEngagement.Model;

namespace TM3ClientExtension.ThirdParty.ZiplingoEngagement.Model
{
    public class ZiplingoEventSettingRequest : CommandRequest
    {
        public string eventKey { get; set; }
        public bool Status { get; set; }
    }
}
