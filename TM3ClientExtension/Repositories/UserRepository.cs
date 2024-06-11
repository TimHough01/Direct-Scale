using Dapper;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebExtension.Helper;
using WebExtension.Models.GenericReports;

namespace TM3ClientExtension.Repositories
{
    public interface IUserRepository
    {
        Task<string> GetEmailByID(int sponsorid);
       
    }
    public class UserRepository : IUserRepository
    {
        private readonly IDataService _dataService;
        public UserRepository(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

       public async Task<string> GetEmailByID(int sponsorid)
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                var parameters = new
                {                   
                    recordnumber = sponsorid
                };
                var sql = @"select EmailAddress from CRM_Distributors where recordnumber = @recordnumber";

                var Email =  await dbConnection.QueryFirstAsync<string>(sql, parameters);

                return Email;
            }
        }
    }
}
