using DirectScale.Disco.Extension;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using TM3ClientExtension.Models;
using TM3ClientExtension.Services;

namespace TM3ClientExtension.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionImportController : ControllerBase
    {
        private readonly ICommissionImportService _commissionImportservice;
        public CommissionImportController(ICommissionImportService commissionImportservice)
        {
            _commissionImportservice = commissionImportservice;
        }
        [HttpGet]
        [Route("ManualBonus")]
        public IActionResult ImportCommission()
        {
            string[] UserRole = { "suremember-business-consultant", "suremember-retail-customer", "suremember-preferred-customer" };

            var GetAllDsUsers = _commissionImportservice.GetCommissionDetails().Result;
            var GetRetailUsers = GetAllDsUsers.Where(x => x.associateRole == 2).ToList();
            var GetBusinessConsultantUsers = GetAllDsUsers.Where(x => x.associateRole == 1).ToList();
            var GetpreferredCustomerUsers = GetAllDsUsers.Where(x => x.associateRole == 3 || x.associateRole == 4).ToList();

            foreach (var role in UserRole)
            {
                var users = _commissionImportservice.GetAllWPUsers(role).Result;
                var MapTm3UsertoWP = new List<UnreleasedBonusReaponse>();
                if (role == "suremember-business-consultant")
                {
                    MapTm3UsertoWP = GetBusinessConsultantUsers;
                }
                else if (role == "suremember-retail-customer")
                {
                    MapTm3UsertoWP = GetRetailUsers;
                }
                else if (role == "suremember-preferred-customer")
                {
                    MapTm3UsertoWP = GetpreferredCustomerUsers;
                }

                foreach (var tm3user in MapTm3UsertoWP)
                {
                    var GetWPUserID = users.Where(x => tm3user.nodeId.ToString() == x.meta_data.FirstOrDefault(m => m.key == "tm3-customer-id")?.value.ToString());


                    if (GetWPUserID.Count() > 0)
                    {
                            var req = new CommissionImportRequest
                            {
                                Date = tm3user.date,
                                NodeIds = GetWPUserID.FirstOrDefault().id.ToString(),
                                comment = tm3user.comment,
                                amount = tm3user.amount

                            };
                            _commissionImportservice.ManulaBonuses(req);
                    }
                }

               
            }
            return Ok();
        }
        [HttpPost]
        [Route("ManualBonusTest")]
        public IActionResult ManualBonusTest()
        {
            var req = new CommissionImportRequest
            {
                Date = DateTime.Now,
                NodeIds = "806",
                comment = "test",
                amount = 10

            };
           var response =  _commissionImportservice.ManulaBonuses(req).Result; 
            
            return Ok(response);

        }
        //[HttpGet]
        //[Route("ChangeSponsorID")]
        //public IActionResult ChangeSponsorIDs()
        //{
        //    var users = _commissionImportservice.GetWPUsers("").Result;
        //    foreach (var user in users)
        //    {
        //      var sponsorID  = user.meta_data.Where(x => x.key == "sponsor-id").Select(x => x.value).FirstOrDefault();

        //        if (sponsorID != null && int.TryParse(sponsorID.ToString(), out int sponsorId))
        //        {
        //            var getSponsorDetailsInDS = _commissionImportservice.GetEmailByID(sponsorId).Result;

        //        }

        //    }

        //    return Ok(users);
        //}

    }
}
