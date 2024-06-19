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
        Task<UnreleasedBonusReaponse> GetCommissionDetails(int associateId);


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
        public async Task<UnreleasedBonusReaponse> GetCommissionDetails(int associateId)
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                var parameters = new
                {
                    associateId = associateId
                };
                var sql = @$"select  d.recordnumber as nodeId,Max(cp.EndDate) as date ,SUM(h.Amount) as amount,cp.PeriodName as comment  from CRM_CommissionHistory h
								 inner join crm_commissionperiods cp on cp.recordnumber=h.ComPeriodID
								 inner join CRM_Distributors d on d.recordnumber=h.AssociateID
								 where d.recordnumber={associateId}
								 group by d.recordnumber,cp.PeriodName";

                var Email = await dbConnection.QueryFirstAsync<UnreleasedBonusReaponse>(sql, parameters);

                return Email;
            }
        }
    }
}
