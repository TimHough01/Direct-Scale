using DirectScale.Disco.Extension;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using TM3ClientExtension.Models;
using TM3ClientExtension.Repositories;
using TM3ClientExtension.Services;

namespace TM3ClientExtension.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionImportController : ControllerBase
    {
        private readonly ICommissionImportService _commissionImportservice;
        private readonly ICustomLogRepository _customLogRepository;
        public CommissionImportController(ICommissionImportService commissionImportservice, ICustomLogRepository customLogRepository)
        {
            _commissionImportservice = commissionImportservice;
            _customLogRepository = customLogRepository;
        }
        [HttpGet]
        [Route("ManualBonus")]
        public IActionResult ImportCommission()
        {
            var failedRequest = new List<CommissionImportRequest>();
            try
            {
                string[] UserRole = { "suremember-business-consultant", "suremember-retail-customer", "suremember-preferred-customer" };
                var GetAllDsUsers = _commissionImportservice.GetCommissionDetails().GetAwaiter().GetResult();
                foreach (var role in UserRole)
                {
                    try
                    {
                        var users = _commissionImportservice.GetAllWPUsers(role).GetAwaiter().GetResult();
                        var MapTm3UsertoWP = new List<UnreleasedBonusReaponse>();
                        if (role == "suremember-business-consultant")
                        {
                            MapTm3UsertoWP = GetAllDsUsers.Where(x => x.associateRole == 1).ToList();
                        }
                        else if (role == "suremember-retail-customer")
                        {
                            MapTm3UsertoWP = GetAllDsUsers.Where(x => x.associateRole == 2).ToList();
                        }
                        else if (role == "suremember-preferred-customer")
                        {
                            MapTm3UsertoWP = GetAllDsUsers.Where(x => x.associateRole == 3 || x.associateRole == 4).ToList();
                        }
                        foreach (var tm3user in MapTm3UsertoWP)
                        {
                            CommissionImportRequest req = null;
                            try
                            {
                                var GetWPUserID = users.Where(x => tm3user.nodeId.ToString() == x.meta_data.FirstOrDefault(m => m.key == "tm3-customer-id")?.value.ToString()).ToList();
                                if (GetWPUserID.Count() > 0)
                                {
                                    req = new CommissionImportRequest
                                    {
                                        Date = tm3user.date,
                                        NodeIds = GetWPUserID.FirstOrDefault().id.ToString(),
                                        comment = tm3user.comment,
                                        amount = tm3user.amount
                                    };
                                    var res = _commissionImportservice.ManulaBonuses(req).GetAwaiter().GetResult();
                                }
                            }
                            catch
                            {
                                failedRequest.Add(req);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _customLogRepository.CustomErrorLog(0, 0, "ImportCommission-role-Error", ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                _customLogRepository.CustomErrorLog(0, 0, "ImportCommission-Final-Error", ex.Message);
            }
            _customLogRepository.CustomErrorLog(0, 0, "ImportCommission-Final", JsonConvert.SerializeObject(failedRequest));
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
