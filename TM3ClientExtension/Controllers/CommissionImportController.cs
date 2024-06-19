using DirectScale.Disco.Extension;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
            
            foreach (var item in UserRole)
            {
                var users = _commissionImportservice.GetWPUsers(item).Result;

                foreach (var user in users)
                {
                    var Bonuses = _commissionImportservice.GetCommissionDetails(user.id).Result;

                    if (Bonuses != null)
                    {
                            var req = new CommissionImportRequest
                            {
                                Date = Bonuses.date,
                                NodeIds = user.id.ToString(),
                                comment = Bonuses.comment,
                                amount = Bonuses.amount

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
        [HttpGet]
        [Route("ChangeSponsorID")]
        public IActionResult ChangeSponsorIDs()
        {
            var users = _commissionImportservice.GetWPUsers("").Result;
            foreach (var user in users)
            {
              var sponsorID  = user.meta_data.Where(x => x.key == "sponsor-id").Select(x => x.value).FirstOrDefault();

                if (sponsorID != null && int.TryParse(sponsorID.ToString(), out int sponsorId))
                {
                    var getSponsorDetailsInDS = _commissionImportservice.GetEmailByID(sponsorId).Result;

                }

            }

            return Ok(users);
        }

    }
}
