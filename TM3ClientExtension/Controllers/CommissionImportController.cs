using DirectScale.Disco.Extension;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
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
            catch (Exception ex)
            {
                _customLogRepository.CustomErrorLog(0, 0, "ImportCommission-Final-Error", ex.Message);
            }
            _customLogRepository.CustomErrorLog(0, 0, "ImportCommission-Final", JsonConvert.SerializeObject(failedRequest));
            return Ok();
        }
        [HttpPost]
        [Route("HistoricalBonuses")]
        public IActionResult HistoricalBonuses()
        {
            var GetAllDsUsers = _commissionImportservice.GetHistoricalManualBonus().GetAwaiter().GetResult();         
            string[] UserRole = { "suremember-business-consultant", "suremember-retail-customer", "suremember-preferred-customer" };
            int[] planId = { 220,243, 244 };
            foreach (var id in planId)
            {
                var GetPeriods = _commissionImportservice.GetPeriodsByCompensationPlanId(id).GetAwaiter().GetResult();
                foreach (var role in UserRole)
                {
                    var users = _commissionImportservice.GetAllWPUsers(role).GetAwaiter().GetResult();
                    var MapTm3UsertoWP = new List<GetHistoricalManualBonusdata>();
                    if (role == "suremember-business-consultant")
                    {
                        MapTm3UsertoWP = GetAllDsUsers.Where(x => x.AssociateRole == 1).ToList();
                    }
                    else if (role == "suremember-retail-customer")
                    {
                        MapTm3UsertoWP = GetAllDsUsers.Where(x => x.AssociateRole == 2).ToList();
                    }
                    else if (role == "suremember-preferred-customer")
                    {
                        MapTm3UsertoWP = GetAllDsUsers.Where(x => x.AssociateRole == 3 || x.AssociateRole == 4).ToList();
                    }
                    foreach (var tm3user in MapTm3UsertoWP)
                    {
                        var GetWPUserID = users.Where(x => tm3user.NodeId.ToString() == x.meta_data.FirstOrDefault(m => m.key == "tm3-customer-id")?.value.ToString());

                        var Periods = GetPeriods.Where(x => x.Begin.Date <= tm3user.BeginDate.Date && x.End.Date >= tm3user.EndDate.Date);
                        if (GetWPUserID.Count() > 0 && Periods.Count() > 0)
                        {
                            var req = new HistoricalBonusRequest
                            {
                                BonusId = tm3user.Comment,
                                PeriodId = Periods.FirstOrDefault().Id,
                                NodeId = GetWPUserID.FirstOrDefault().id.ToString(),
                                Amount = tm3user.Amount
                            };
                            var response = _commissionImportservice.PostHistoricalManualBonus(req).GetAwaiter().GetResult();
                        }
                    }


                }
            }

           
            return Ok();


        }
        [HttpPost]
        [Route("PostHistoricalValues")]
        public IActionResult PostHistoricalValues(int CompensationPlanId)
        {
            string[] UserRole = { "suremember-business-consultant", "suremember-retail-customer", "suremember-preferred-customer" };
            var HistoricalValuesKey = new HistoricalValuesKey();
            var GetAllDsUsers = _commissionImportservice.GetHistoricalValuesData(CompensationPlanId).GetAwaiter().GetResult();
            var GetPeriods = _commissionImportservice.GetPeriodsByCompensationPlanId(CompensationPlanId).GetAwaiter().GetResult();

            foreach (var role in UserRole)
            {
                var users = _commissionImportservice.GetAllWPUsers(role).GetAwaiter().GetResult();
                var MapTm3UsertoWP = new List<HistoricalValues>();
                if (role == "suremember-business-consultant")
                {
                    MapTm3UsertoWP = GetAllDsUsers.Where(x => x.AssociateType == 1).ToList();
                }
                else if (role == "suremember-retail-customer")
                {
                    MapTm3UsertoWP = GetAllDsUsers.Where(x => x.AssociateType == 2).ToList();
                }
                else if (role == "suremember-preferred-customer")
                {
                    MapTm3UsertoWP = GetAllDsUsers.Where(x => x.AssociateType == 3 || x.AssociateType == 4).ToList();
                }
                foreach (var tm3user in MapTm3UsertoWP)
                {
                    var GetWPUserID = users.Where(x => tm3user.AssociateId.ToString() == x.meta_data.FirstOrDefault(m => m.key == "tm3-customer-id")?.value.ToString());

                    var Periods = GetPeriods.Where(x => x.Begin.Date <= tm3user.BeginDate.Date && x.End.Date >= tm3user.EndDate.Date);

                    var CheckKey = HistoricalValuesKey.kpis.Where(x => x.Value == tm3user.Key);


                    if (GetWPUserID.Count() > 0 && Periods.Count() > 0 && CheckKey.Count() > 0)
                    {
                        var req = new HistoricalValuesRequest
                        {
                            key = CheckKey.FirstOrDefault().Key,
                            periodId = Periods.FirstOrDefault().Id,
                            nodeId = GetWPUserID.FirstOrDefault().id.ToString(),
                            sumValue = tm3user.SumValue,
                            postDate = tm3user.postDate

                        };
                        var response = _commissionImportservice.PostHistoricalData(req).Result;
                    }
                }


            }
            return Ok();

        }
        [HttpGet]
        [Route("ChangeSponsorID")]
        public IActionResult ChangeSponsorIDs()
        {
                var users = _commissionImportservice.GetAllWPUsers("").GetAwaiter().GetResult();
                var MapTm3UsertoWP = new List<HistoricalValues>();
                foreach (var user in users)
                {
                    var GetDSUserID = user.meta_data.FirstOrDefault(m => m.key == "sponsor-id")?.value.ToString();
                    var GetWPUseridByCustomField = users.Where(x => x.meta_data.Where(m => m.key == "tm3-customer-id").FirstOrDefault()?.value.ToString() == GetDSUserID).FirstOrDefault();
                    if (GetWPUseridByCustomField != null)
                    {
                        var updatedUser =  _commissionImportservice.UpdateSponsorDetailsIntoWordpress(user.id, GetWPUseridByCustomField.id).GetAwaiter().GetResult();
                    }

                }

                return Ok("Success");
        }
        [HttpPost]
        [Route("GetRewardPointsDetails")]
        public IActionResult GetRewardPointsDetails()
        {
            try
            {
                var RewardPointData = _commissionImportservice.GetRewardPointsData().GetAwaiter().GetResult();

                return Ok(RewardPointData);
            }
            catch (Exception)
            {
                throw;
            }

        }
        [HttpGet]
        [Route("GetPendingProductCreditKpiValues")]
        public IActionResult GetPendingProductCreditKpiValues(string Period)
        {
            try
            {
                var RewardPointData = _commissionImportservice.GetPendingProductValue().GetAwaiter().GetResult();

                return Ok(RewardPointData);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
