﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM3ClientExtension.ThirdParty.ZiplingoEngagement.Model;

namespace TM3ClientExtension.ThirdParty.ZiplingoEngagement.Model
{
    public class ZiplingoEngagementSettingsRequest : CommandRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiUrl { get; set; }
        public string LogoUrl { get; set; }
        public string CompanyName { get; set; }
    }
}
