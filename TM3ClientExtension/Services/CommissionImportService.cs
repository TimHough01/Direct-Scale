using DirectScale.Disco.Extension;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TM3ClientExtension.Models;

namespace TM3ClientExtension.Services
{
    public interface ICommissionImportService
    {
        Task<List<Users>> GetWPUsers();
        Task<List<UnreleasedBonusReaponse>> GetBonuses(CommissionBonuseRequest req);
        Task<dynamic> ManulaBonuses(CommissionImportRequest req);
    }
    public class CommissionImportService : ICommissionImportService
    {
        private static readonly HttpClient client = new HttpClient();
        public CommissionImportService()
        {

        }
        public async Task<List<Users>> GetWPUsers()
        {
            
            client.BaseAddress = new Uri("https://stg-mytm3-317.uw2.rapydapps.cloud/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("mahaveer@ziplingo.com:Clover@#69"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            try
            {
                HttpResponseMessage response = await client.GetAsync($"wp-admin/wp/v2/users?per_page=10");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                var users = JsonConvert.DeserializeObject<List<Users>>(responseBody);
                return users;
               
            }
            catch (HttpRequestException e)
            {
                return new List<Users>();
            }
        }
        public async Task<List<UnreleasedBonusReaponse>> GetBonuses(CommissionBonuseRequest req)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.pillarshub.com/api/v1/Bonuses/Unreleased?date={req.Date}&nodeIds={req.NodeIds}&offset={req.Offset}&count={req.Count}");
            request.Headers.Add("Authorization", "Bearer ");
            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<UnreleasedBonusReaponse>>(result);
        }
        public async Task<dynamic> ManulaBonuses(CommissionImportRequest req)
        {
            using var client = new HttpClient();

            var url = "https://api.pillarshub.com/api/v1/Bonuses/Manual";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", "Bearer ");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("content-type", "application/*+json");

            var body = new
            {
                date = req.Date,
                nodeId = req.NodeIds,
                comment = req.comment,
                amount = req.amount
            };

            string jsonBody = JsonConvert.SerializeObject(body);

            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<dynamic>(result);
        }
    }
}
