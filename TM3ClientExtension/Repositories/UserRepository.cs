using Dapper;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TM3ClientExtension.Models;
using WebExtension.Helper;
using WebExtension.Models.GenericReports;

namespace TM3ClientExtension.Repositories
{
    public interface IUserRepository
    {
        Task<string> GetEmailByID(int sponsorid);
        Task<List<UnreleasedBonusReaponse>> GetCommissionDetails();
        Task<List<HistoricalValues>> GetHistoricalValuesData();

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
        public async Task<List<UnreleasedBonusReaponse>> GetCommissionDetails()
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                
                var sql = @$"SELECT
	                    d.recordnumber as nodeId, h.PostDate as date, h.Amount as amount, 
	                    IIF(LEN(H.Comment) > 0, H.Comment + ' (' + cp.PeriodName + ' ' +FORMAT(cp.BeginDate, 'MM/dd/yyyy') + '-' + FORMAT(cp.EndDate, 'MM/dd/yyyy') + ')', cp.PeriodName) as comment, d.associatetype as associateRole
                    FROM CRM_CommissionHistory h
	                    inner join crm_commissionperiods cp on cp.recordnumber=h.ComPeriodID
	                    inner join CRM_Distributors d on d.recordnumber=h.AssociateID
                    ORDER BY PostDate";

                var Email = await dbConnection.QueryAsync<UnreleasedBonusReaponse>(sql);

                return Email.ToList();
            }
        }
        public async Task<List<HistoricalValues>> GetHistoricalValuesData()
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {

                var sql = @$"";

                var Email = await dbConnection.QueryAsync<HistoricalValues>(sql);

                return Email.ToList();
            }
        }
    }
}
