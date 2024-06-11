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
        Task<List<Users>> GetWPUsers(string email);
        Task<List<UnreleasedBonusReaponse>> GetBonuses(CommissionBonuseRequest req);
        Task<dynamic> ManulaBonuses(CommissionImportRequest req);
        Task<dynamic> updateuserImage(int userID);
        Task<string> GetEmailByID(int sponsorid);
    }
    public class CommissionImportService : ICommissionImportService
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly IUserRepository _userRepository;
        public CommissionImportService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<dynamic> updateuserImage(int userID)
        {
            using var client = new HttpClient();

            var url = "https://stg-mytm3-317.uw2.rapydapps.cloud/wp-json/wc/v3/customers/2122";

            var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Headers.Add("Authorization", "basic Y2tfYWFlNWRmMThjMTFhNDRhZjRhZGNkNzczZTljZjdiYmYzNjBhNjNlZDpjc182NTU1MzgzNWVlMGFlMjgyZTc5Y2NiMTNlNDY0YjJhY2FmNWFjZDFm");
            request.Headers.Add("accept", "application/json");

            var body = new
            {
                avatar_url = "https://tm3united.corpadmin.directscale.com/BackOffice/ProfileImage?id=2"
            };

            string jsonBody = JsonConvert.SerializeObject(body);

            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        public async Task<List<Users>> GetWPUsers(string email)
        {
            var customers = email != "" ? "?email=" + email : "";
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://stg-mytm3-317.uw2.rapydapps.cloud/wp-json/wc/v3/customers"+ customers);
            request.Headers.Add("Authorization", "Basic Y2tfYWFlNWRmMThjMTFhNDRhZjRhZGNkNzczZTljZjdiYmYzNjBhNjNlZDpjc182NTU1MzgzNWVlMGFlMjgyZTc5Y2NiMTNlNDY0YjJhY2FmNWFjZDFm");
            var response = await client.SendAsync(request);
              response.EnsureSuccessStatusCode();


            var users = JsonConvert.DeserializeObject<List<Users>>(await response.Content.ReadAsStringAsync());
            return users;

           
        }
        public async Task<List<UnreleasedBonusReaponse>> GetBonuses(CommissionBonuseRequest req)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.pillarshub.com/api/v1/Bonuses/Unreleased?date={req.Date}&nodeIds={req.NodeIds}&offset={req.Offset}&count={req.Count}");
            request.Headers.Add("Authorization", "Bearer cGF1bGordG0zQHRhdmFoYXR6LmNvbTp0bTNQaWxsYXI1");
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
            request.Headers.Add("Authorization", "Basic cGF1bGordG0zQHRhdmFoYXR6LmNvbTp0bTNQaWxsYXI1");
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
        public Task<string> GetEmailByID(int sponsorid)
        {
            return _userRepository.GetEmailByID(sponsorid);
        }

    }
}
