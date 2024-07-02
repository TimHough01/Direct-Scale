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
        Task<List<HistoricalValues>> GetHistoricalValuesData(int periodId);
        Task<List<RewardPoints>> GetRewardPointDetails();
        Task<List<Pendingproductvalue>> GetPendingProductValue();

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
	                    h.[group] as comment, d.associatetype as associateRole
                    FROM CRM_CommissionHistory h
	                    inner join crm_commissionperiods cp on cp.recordnumber=h.ComPeriodID
	                    inner join CRM_Distributors d on d.recordnumber = h.AssociateID
                    ORDER BY PostDate";

                var Email = await dbConnection.QueryAsync<UnreleasedBonusReaponse>(sql);

                return Email.ToList();
            }
        }
        public async Task<List<HistoricalValues>> GetHistoricalValuesData(int periodId)
        {
            var periodName = "";
            if (periodId == 220)
            {
                periodName = "Weekly";
            }
            else if (periodId == 243)
            {
                periodName = "FourWeek";
            }
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {

                var sql = @$"select  d.recordnumber as AssociateId,
                            cp.BeginDate as BeginDate,
                            cp.EndDate as EndDate,
                            cp.CommitDate as postDate,
                            ISNULL(cav.Value,0) as SumValue,
                            d.AssociateType as AssociateType,
                            CASE cp.PeriodName 
                            WHEN 'FourWeek' THEN '4Week'
                            ELSE cp.PeriodName 
                            END as PeriodName ,
                            cav.OptionID as 'Key',
                            rn.Rank as Lastvalue
                            from CRM_CommissionAssociateValues_COV cav
                            inner join CRM_CommissionAssociateValues av ON av.AssociateID=cav.AssociateID and av.ComPeriodID=cav.ComPeriodID
                            INNER JOIN CRM_RANKS rn on rn.RankID = av.HighRank
                             inner join crm_commissionperiods cp on cp.recordnumber=cav.ComPeriodID
                            inner join CRM_Distributors d on d.recordnumber=cav.AssociateID
                            where cp.CommitDate is not null and cp.PeriodName = '{periodName}'
                            and cav.OptionID in ('BC','RC','R50','R60','R70','R80','R90','R100','R110','R120','R130','R140','COMACT','GVKPI') 
                            group by d.recordnumber,cp.PeriodName,cav.OptionID,cav.Value,d.AssociateType,cp.BeginDate,cp.EndDate,cp.CommitDate,rn.Rank";

                var Email = await dbConnection.QueryAsync<HistoricalValues>(sql);

                return Email.ToList();
            }
        }
        public async Task<List<RewardPoints>> GetRewardPointDetails()
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                var sql = @$"select Distinct d.recordnumber as AssociateID,
                            d.BackofficeID as BackofficeID,
                            ISNULL(d.LegalFirstName,d.FirstName) as FirstName,
                            ISNULL(d.LegalLastName,d.LastName) as LastName,
                            ISNULL(d.EmailAddress,'') as EmailAddress,
                            SUM(ISNULL(rp.RemainingBalance,0)) as RewardPoint
                            from CRM_Distributors d
                            JOIN CRM_RewardPoints rp on rp.associateID = d.recordnumber
                            group by d.recordnumber,d.BackofficeID,d.LegalFirstName,d.FirstName,d.LegalLastName,d.LastName,d.EmailAddress";

                var RewardPointData = await dbConnection.QueryAsync<RewardPoints>(sql);

                return RewardPointData.ToList();
            }
        }
        public async Task<List<Pendingproductvalue>> GetPendingProductValue()
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                var sql = @$"select d.recordnumber as AssociateID,
                            d.BackofficeID as BackofficeID,
                            ISNULL(d.LegalFirstName,d.FirstName) as FirstName,
                            ISNULL(d.LegalLastName, d.LastName) as LastName,
                            ISNULL(d.EmailAddress,'') as EmailAddress,
                            ISNULL((select value from CRM_Stats_StatValues where associateid = d.recordnumber and Stat = 'PPCKPI' and PeriodKey like '%fourweek%'),0) as PPCKPI
                            from CRM_Distributors d
                            JOIN CRM_Stats_StatValues st on st.associateid = d.recordnumber
                            group by d.recordnumber,d.BackofficeID,d.LegalFirstName,d.FirstName,d.LegalLastName,d.LastName,d.EmailAddress";

                var Pendingproductvalues = await dbConnection.QueryAsync<Pendingproductvalue>(sql);

                return Pendingproductvalues.ToList();
            }
        }
    }
}
