using DirectScale.Disco.Extension;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
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
        //Task<List<UnreleasedBonusReaponse>> GetBonuses(CommissionBonuseRequest req);
        Task<dynamic> ManulaBonuses(CommissionImportRequest req);
        //Task<dynamic> updateuserImage(int userID);
        Task<string> GetEmailByID(int sponsorid);
        Task<dynamic> GetCompensationPlans();
        Task<List<UnreleasedBonusReaponse>> GetCommissionDetails();
        Task<dynamic> PostHistoricalData(HistoricalValuesRequest req);
        Task<List<Periods>> GetPeriodsByCompensationPlanId(int compensationPlanId);
        Task<List<HistoricalValues>> GetHistoricalValuesData();
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
            int perPage = 100;
            bool hasMoreUsers = true;

            while (hasMoreUsers)
            {
                var users = await GetWPUsers(page, perPage, UserRole);
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
       

        public async Task<List<UnreleasedBonusReaponse>> GetCommissionDetails()
        {
            return await _userRepository.GetCommissionDetails();
        }
        public async Task<List<HistoricalValues>> GetHistoricalValuesData()
        {
            return await _userRepository.GetHistoricalValuesData();
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
                lastValue = req.lastValue,
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



    }
}
