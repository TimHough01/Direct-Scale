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
        [Route("Importcommission")]
        public IActionResult ImportCommission()
        {
            var users = _commissionImportservice.GetWPUsers("").Result;

            //var nodeids = users.Select(x => x.ID.ToString()).ToList();
            //string[] nodeid = { "1640", "305", "2" };
            //var request = new CommissionBonuseRequest
            //{
            //    Date = DateTime.Now,
            //    NodeIds = nodeid.ToList(),
            //    Offset = 1,
            //    Count = 10
            //};

            //var Bonuses = _commissionImportservice.GetBonuses(request).Result;

            //if (Bonuses != null)
            //{
            //    foreach (var bonus in Bonuses)
            //    {
            //        var req = new CommissionImportRequest
            //        {
            //            Date = DateTime.Now,
            //            NodeIds = bonus.NodeId,
            //            comment = bonus.Description,
            //            amount = bonus.Amount

            //        };

            //        _commissionImportservice.ManulaBonuses(req);
            //    }

               
            //}


            return Ok(users);
        }
        [HttpPut]
        [Route("updateimages")]
        public IActionResult updateimages(int userid)
        {
           var response =  _commissionImportservice.updateuserImage(userid);
            return Ok(response);
        }

        [HttpGet]
        [Route("ChangeSponsorID")]
        public IActionResult ChangeSponsorID()
        {
            var users = _commissionImportservice.GetWPUsers("").Result;
            foreach (var user in users)
            {
              var sponsorID  = user.meta_data.Where(x => x.key == "sponsor-id").Select(x => x.value).FirstOrDefault();

                if (sponsorID != null && int.TryParse(sponsorID.ToString(), out int sponsorId))
                {
                    var getSponsorDetailsInDS = _commissionImportservice.GetEmailByID(sponsorId).Result;

                    var GetWPuserByEmail = _commissionImportservice.GetWPUsers(getSponsorDetailsInDS).Result;



                }


            }

            


            return Ok(users);
        }

    }
}
