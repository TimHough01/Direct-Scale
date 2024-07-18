using DirectScale.Disco.Extension;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TM3ClientExtension.Models;
using TM3ClientExtension.Repositories;
using static Dapper.SqlMapper;

namespace TM3ClientExtension.Services
{
    public interface ICommissionImportService
    {
        Task<List<Users>> GetAllWPUsers(string UserRole);
        Task<List<Users>> GetAllWPUsersSendItAcadamy(string UserRole);
        //Task<List<UnreleasedBonusReaponse>> GetBonuses(CommissionBonuseRequest req);
        Task<dynamic> ManulaBonuses(CommissionImportRequest req);
        //Task<dynamic> updateuserImage(int userID);
        Task<string> GetEmailByID(int sponsorid);
        Task<dynamic> GetCompensationPlans();
        Task<List<UnreleasedBonusReaponse>> GetCommissionDetails();
        Task<dynamic> PostHistoricalData(HistoricalValuesRequest req);
        Task<List<Periods>> GetPeriodsByCompensationPlanId(int compensationPlanId);
        Task<List<HistoricalValues>> GetHistoricalValuesData(int PeriodId);
        Task<Users> UpdateSponsorDetailsIntoWordpress(int CustomerId, int WPUplineID);
        Task<Users> UpdateSponsorDetailsIntoWordpressForSendItAcadamy(int CustomerId, int WPUplineID);
        Task<List<RewardPoints>> GetRewardPointsData();
        Task<List<Pendingproductvalue>> GetPendingProductValue();
        Task<HistoricalBonusResponse> PostHistoricalManualBonus(HistoricalBonusRequest req);
        Task<List<GetHistoricalManualBonusdata>> GetHistoricalManualBonus();
        Task<List<WPUserTokens>> GetWPUserTokensData();
        Task<bool> SaveWPTokenDetails(WPUserTokens req);
        Task<List<AutoshipCardDetails>> GetUserCardDetails();
        Task<int> GetAssociateByEmail(string email);
        Task<int> UpdateDefaultCardForAutoship(bool isdefault, string token);
    }
    public class CommissionImportService : ICommissionImportService
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly IUserRepository _userRepository;
        public CommissionImportService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<Users>> GetAllWPUsers(string UserRole)
        {
            var allUsers = new List<Users>();
            int page = 1;
            var users = new List<Users>();
            int perPage = 100;
            bool hasMoreUsers = true;
            if (UserRole == "")
            {
                string[] UserRoles = { "suremember-business-consultant", "suremember-retail-customer", "suremember-preferred-customer" };

                foreach (var item in UserRoles)
                {
                    page = 1;
                    hasMoreUsers = true;
                    while (hasMoreUsers)
                    {
                        users = await GetWPUsers(page, perPage, item);
                        if (users.Count > 0)
                        {
                            allUsers.AddRange(users);
                            page++;
                            if (users.Count < 100)
                            {
                                hasMoreUsers = false;
                            }
                        }
                        else
                        {
                            hasMoreUsers = false;
                        }
                    }
                }
            }
            else
            {
                while (hasMoreUsers)
                {
                    users = await GetWPUsers(page, perPage, UserRole);
                    if (users.Count > 0)
                    {
                        allUsers.AddRange(users);
                        page++;
                        if (users.Count < 100)
                        {
                            hasMoreUsers = false;
                        }
                    }
                    else
                    {
                        hasMoreUsers = false;
                    }
                }
            }

          
            return allUsers;
        }

        public async Task<List<Users>> GetWPUsers(int page, int per_page ,string UserRole )
        {           
                var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://stg-mytm3-317.uw2.rapydapps.cloud/wp-json/wc/v3/customers?page={page}&per_page={per_page}&role={UserRole}");
            request.Headers.Add("Authorization", "Basic Y2tfYWFlNWRmMThjMTFhNDRhZjRhZGNkNzczZTljZjdiYmYzNjBhNjNlZDpjc182NTU1MzgzNWVlMGFlMjgyZTc5Y2NiMTNlNDY0YjJhY2FmNWFjZDFm");

            var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var users = JsonConvert.DeserializeObject<List<Users>>(await response.Content.ReadAsStringAsync());
                return users;
            
        }

        public async Task<List<Users>> GetAllWPUsersSendItAcadamy(string UserRole)
        {
            var allUsers = new List<Users>();
            int page = 1;
            var users = new List<Users>();
            int perPage = 100;
            bool hasMoreUsers = true;
            if (UserRole == "")
            {
                string[] UserRoles = {"customer" };
                foreach (var item in UserRoles)
                {
                    page = 1;
                    hasMoreUsers = true;
                    while (hasMoreUsers)
                    {
                        users = await GetWPUsersSendItAcadamy(page, perPage, item);
                        if (users.Count > 0)
                        {
                            allUsers.AddRange(users);
                            page++;
                            if (users.Count < 100)
                            {
                                hasMoreUsers = false;
                            }
                        }
                        else
                        {
                            hasMoreUsers = false;
                        }
                    }
                }
            }
            else
            {
                while (hasMoreUsers)
                {
                    users = await GetWPUsersSendItAcadamy(page, perPage, UserRole);
                    if (users.Count > 0)
                    {
                        allUsers.AddRange(users);
                        page++;
                        if (users.Count < 100)
                        {
                            hasMoreUsers = false;
                        }
                    }
                    else
                    {
                        hasMoreUsers = false;
                    }
                }
            }


            return allUsers;
        }

        public async Task<List<Users>> GetWPUsersSendItAcadamy(int page, int per_page, string UserRole)
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://ringaln.com/wp-json/wc/v3/customers?page={page}&per_page={per_page}&role={UserRole}&consumer_key=ck_90088e8eb5366467e4f11a49ab5a31dab7e9e647&consumer_secret=cs_4479c1ad2746b3037da08a01210721077319a9d3");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var users = JsonConvert.DeserializeObject<List<Users>>(await response.Content.ReadAsStringAsync());
            return users;

        }

        public async Task<List<UnreleasedBonusReaponse>> GetCommissionDetails()
        {
            return await _userRepository.GetCommissionDetails();
        }
        public async Task<List<HistoricalValues>> GetHistoricalValuesData(int PeriodId)
        {
            return await _userRepository.GetHistoricalValuesData(PeriodId);
        }

        public async Task<dynamic> ManulaBonuses(CommissionImportRequest req)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.pillarshub.com/api/v1/Bonuses/Manual");
            request.Headers.Add("Authorization", "Ufgorzw1hP6SPVkr1oP9ZiNWNU1FnNivylxcPONgkWp9");
            request.Headers.Add("accept", "application/json");
            var contentJson = new
            {
                date = req.Date,
                nodeId = req.NodeIds,
                comment = req.comment,
                amount = req.amount
            };
            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(contentJson),
                System.Text.Encoding.UTF8,
                "application/+json"
            );
            request.Content = content;
            var response = client.Send(request);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public Task<string> GetEmailByID(int sponsorid)
        {
            return _userRepository.GetEmailByID(sponsorid);
        }
        public async Task<dynamic> GetCompensationPlans()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.pillarshub.com/api/v1/CompensationPlans");
            request.Headers.Add("Authorization", "Ufgorzw1hP6SPVkr1oP9ZiNWNU1FnNivylxcPONgkWp9");
            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

        }
        public async Task<List<Periods>> GetPeriodsByCompensationPlanId(int compensationPlanId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.pillarshub.com/api/v1/CompensationPlans/{compensationPlanId}/Periods");
            request.Headers.Add("Authorization", "Ufgorzw1hP6SPVkr1oP9ZiNWNU1FnNivylxcPONgkWp9");
            request.Headers.Add("accept", "application/json");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var Period = JsonConvert.DeserializeObject<List<Periods>>(await response.Content.ReadAsStringAsync());

            return Period;

        }

        public async Task<dynamic> PostHistoricalData(HistoricalValuesRequest req)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.pillarshub.com/api/v1/HistoricalValues");
            request.Headers.Add("Authorization", "Ufgorzw1hP6SPVkr1oP9ZiNWNU1FnNivylxcPONgkWp9");
            request.Headers.Add("accept", "application/json");
            var contentJson = new
            {
                key = req.key,
                periodId = req.periodId,
                nodeId = req.nodeId,
                sumValue = req.sumValue,
                postDate = req.postDate
            };
            var content = new StringContent(
                JsonConvert.SerializeObject(contentJson),
                Encoding.UTF8,
                "application/+json"
            );
          
            request.Content = content;
            var response = client.Send(request);
            var result = await response.Content.ReadAsStringAsync();
            return result;

        }
        public async Task<Users> UpdateSponsorDetailsIntoWordpress(int UserId, int WPUplineID)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Put, $"https://stg-mytm3-317.uw2.rapydapps.cloud/wp-json/wc/v3/customers/{UserId}");
            request.Headers.Add("Authorization", "Basic Y2tfYWFlNWRmMThjMTFhNDRhZjRhZGNkNzczZTljZjdiYmYzNjBhNjNlZDpjc182NTU1MzgzNWVlMGFlMjgyZTc5Y2NiMTNlNDY0YjJhY2FmNWFjZDFm");
                    var metaData = new
                    {
                        meta_data = new[]
                        {
                            new
                            {
                                key = "sponsor-id",
                                value = WPUplineID
                            }
                        }
                    };

            string jsonString = JsonConvert.SerializeObject(metaData);

            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var user = JsonConvert.DeserializeObject<Users>(await response.Content.ReadAsStringAsync());

            return user;
        }

        public async Task<Users> UpdateSponsorDetailsIntoWordpressForSendItAcadamy(int UserId, int WPUplineID)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Put, $"https://ringaln.com/wp-json/wc/v3/customers/{UserId}");
            request.Headers.Add("Authorization", "Basic Y2tfOTAwODhlOGViNTM2NjQ2N2U0ZjExYTQ5YWI1YTMxZGFiN2U5ZTY0Nzpjc180NDc5YzFhZDI3NDZiMzAzN2RhMDhhMDEyMTA3MjEwNzczMTlhOWQz");

            var metaData = new
            {
                meta_data = new[]
                {
                            new
                            {
                                key = "uplineId",
                                value = WPUplineID
                            },
                            new
                            {
                                key = "sponsor-id",
                                value = WPUplineID
                            }
                        }
            };

            string jsonString = JsonConvert.SerializeObject(metaData);

            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var user = JsonConvert.DeserializeObject<Users>(await response.Content.ReadAsStringAsync());

            return user;
        }

        public async Task<List<RewardPoints>> GetRewardPointsData()
        {
            return  await _userRepository.GetRewardPointDetails();
        }
        public async Task<List<Pendingproductvalue>> GetPendingProductValue()
        {
            return await _userRepository.GetPendingProductValue();
        }
        public async Task<HistoricalBonusResponse> PostHistoricalManualBonus(HistoricalBonusRequest req)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.pillarshub.com/api/v1/HistoricalBonuses");
            request.Headers.Add("Authorization", "Ufgorzw1hP6SPVkr1oP9ZiNWNU1FnNivylxcPONgkWp9");
            request.Headers.Add("accept", "application/json");
            var contentJson = new
            {
                bonusId = req.BonusId,
                periodId = req.PeriodId,
                nodeId = req.NodeId,
                amount = req.Amount
            };
            var content = new StringContent(
                 JsonConvert.SerializeObject(contentJson),
                 Encoding.UTF8,
                 "application/+json"
             );
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var Response = JsonConvert.DeserializeObject<HistoricalBonusResponse>(await response.Content.ReadAsStringAsync());

            return Response;
        }
        public async Task<List<GetHistoricalManualBonusdata>> GetHistoricalManualBonus()
        {
            return await _userRepository.GetHistoricalManualBonus();
        }

        public async Task<List<WPUserTokens>> GetWPUserTokensData()
        {
            return await _userRepository.GetWPUserTokensData();
        }
        public async Task<bool> SaveWPTokenDetails(WPUserTokens req)
        {
            return await _userRepository.SaveWPTokenDetails(req);
        }
        public async Task<List<AutoshipCardDetails>> GetUserCardDetails()
        {
            return await _userRepository.GetUserCardDetails();
        }
        public async Task<int> GetAssociateByEmail(string email)
        {
            return await _userRepository.GetAssociateByEmail(email);
        }
        public async Task<int> UpdateDefaultCardForAutoship(bool isdefault, string token)
        {
            return await _userRepository.UpdateDefaultCardForAutoship(isdefault, token);
        }
    }
}
