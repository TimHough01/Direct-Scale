using Dapper;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TM3ClientExtension.Models;
using WebExtension.Helper;
using WebExtension.Models.GenericReports;
using MySql.Data.MySqlClient;
using static Dapper.SqlMapper;
using Org.BouncyCastle.Ocsp;

namespace TM3ClientExtension.Repositories
{
    public interface IUserRepository
    {
        Task<string> GetEmailByID(int sponsorid);
        Task<List<UnreleasedBonusReaponse>> GetCommissionDetails();
        Task<List<HistoricalValues>> GetHistoricalValuesData(int periodId);
        Task<List<RewardPoints>> GetRewardPointDetails();
        Task<List<Pendingproductvalue>> GetPendingProductValue();
        Task<List<GetHistoricalManualBonusdata>> GetHistoricalManualBonus();
        Task<List<WPUserTokens>> GetWPUserTokensData();
        Task<bool> SaveWPTokenDetails(WPUserTokens req);
        Task<List<AutoshipCardDetails>> GetUserCardDetails();
        Task<int> GetAssociateByEmail(string email);
        Task<int> UpdateDefaultCardForAutoship(bool isdefault, string token);
        Task<List<SendItAcademy_MatrixData>> GetSendItAcademy_MatrixData();

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
        public async Task<List<GetHistoricalManualBonusdata>> GetHistoricalManualBonus()
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                var sql = @$"SELECT 
                            d.recordnumber as nodeId,
                            cp.BeginDate as BeginDate, 
                            cp.enddate as EndDate, 
                            h.Amount as amount, 
                             CONCAT(h.[group], '_', h.recordnumber) as comment, 
                            d.associatetype as associateRole
                        FROM 
                            CRM_CommissionHistory h
                            INNER JOIN crm_commissionperiods cp ON cp.recordnumber = h.ComPeriodID
                            INNER JOIN CRM_Distributors d ON d.recordnumber = h.AssociateID
                        ORDER BY 
                            PostDate;";

                var Pendingproductvalues = await dbConnection.QueryAsync<GetHistoricalManualBonusdata>(sql);

                return Pendingproductvalues.ToList();
            }
        }
        //public async Task<List<WPUserTokens>> GetWPUserTokensData()
        //{
        //    using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
        //    {
        //        var sql = @$";WITH RankedPayments AS (
        //                        SELECT distinct
        //                            'nmi' AS gateway_id,
        //                            p.ExternalID AS token,
        //                            p.DistributorID AS user_id,
        //                            'CC' AS type,
        //                            ROW_NUMBER() OVER (PARTITION BY p.DistributorID ORDER BY p.DistributorID) AS rn,
        //                            COUNT(*) OVER (PARTITION BY p.DistributorID) AS user_count
        //                        FROM 
        //                            CRM_Payments p 		
		      //                      where p.ExternalID is not null and p.firstsix is not null
        //                    )
        //                    SELECT distinct
        //                        gateway_id,
        //                        token,
        //                        user_id,
        //                        type,
        //                        CASE 
        //                            WHEN user_count > 1 AND rn = 1 THEN 1
        //                            ELSE 0
        //                        END AS is_default
        //                    FROM 
        //                        RankedPayments;";

        //        var Pendingproductvalues = await dbConnection.QueryAsync<WPUserTokens>(sql);

        //        return Pendingproductvalues.ToList();
        //    }
        //}
        public async Task<List<WPUserTokens>> GetWPUserTokensData()
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                var sql = @$"select * from Client.WPTokensDetails";

                var Pendingproductvalues = await dbConnection.QueryAsync<WPUserTokens>(sql);

                return Pendingproductvalues.ToList();
            }
        }
        //public async Task<bool> SaveWPTokenDetails(WPUserTokens req)
        //{
        //    try
        //    {
        //        using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
        //        {
        //            var parameters = new
        //            {
        //                gateway_id = req.gateway_id,
        //                token = req.token,
        //                user_id = req.user_id,
        //                type = req.type,
        //                is_default = req.is_default
        //            };
        //            var sql = @$"IF NOT EXISTS (
        //                        SELECT * 
        //                        FROM INFORMATION_SCHEMA.TABLES 
        //                        WHERE TABLE_SCHEMA = 'Client' 
        //                        AND TABLE_NAME = 'WPTokensDetails'
        //                    )
        //                    BEGIN
        //                        CREATE TABLE Client.WPTokensDetails (
        //                            gateway_id NVARCHAR(255),
        //                            token NVARCHAR(max),
        //                            user_id NVARCHAR(255),
        //                            type NVARCHAR(255),
        //                            is_default BIT
        //                        );

        //                        INSERT INTO Client.WPTokensDetails (gateway_id, token, user_id, type, is_default)
        //                        VALUES (@gateway_id, @token, @user_id, @type, @is_default);
        //                    END
        //                    ELSE
        //                    BEGIN
        //                        INSERT INTO Client.WPTokensDetails (gateway_id, token, user_id, type, is_default)
        //                        VALUES (@gateway_id, @token, @user_id, @type, @is_default);
        //                    END";

        //            var Pendingproductvalues = dbConnection.Execute(sql, parameters);

        //            return true;
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        return false;
        //    }
        //}
        public async Task<bool> SaveWPTokenDetails(WPUserTokens req)
        {
            string connectionString = "Server=stg-mytm3-317.uw2.rapydapps.cloud;Port=8443;Database=wp_1222101;User ID=user-3445075;Password=ekOHaF1bI4;SslMode=Preferred;Connection Timeout=30;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    await conn.OpenAsync(); // Ensure the connection is opened asynchronously

                    var parameters = new
                    {
                        gateway_id = req.gateway_id,
                        token = req.token,
                        user_id = req.user_id,
                        type = req.type,
                        is_default = req.is_default
                    };
                    var sql = @$"INSERT INTO wp_woocommerce_payment_tokens (gateway_id, token, user_id, type, is_default) VALUES (@gateway_id, @token, @user_id, @type, @is_default)";

                    var pendingProductValues = await conn.ExecuteAsync(sql, parameters);

                    return pendingProductValues > 0;
                }
            }
            catch (Exception ex)
            {
                // Log exception details for further investigation
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
        public async Task<List<AutoshipCardDetails>> GetUserCardDetails()
        {
            try
            {
                using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
                {
                    var sql = @$"select user_id as UserId, first_name as FirstName,last_name as LastName,card_number_masked as CardNumberMasked, card_expiry_month as CardExpiryMonth,card_expiry_year as CardExpiryYear, nmi_customer_id as NmiCustomerId, card_last4 as CardLast4,card_type as CardType from [Client].[WordPress_Tokens]";

                    var pendingProductValues = await dbConnection.QueryAsync<AutoshipCardDetails>(sql);

                    return pendingProductValues.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<AutoshipCardDetails>();
            }
        }
        public async Task<int> GetAssociateByEmail(string email)
        {
            try
            {
                using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
                {
                    var sql = @$"select recordnumber as associateId from CRM_Distributors where EmailAddress = '{email}'";

                    var pendingProductValues = await dbConnection.QueryAsync<int>(sql);

                    return pendingProductValues.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int> UpdateDefaultCardForAutoship(bool isdefault,string token)
        {
            try
            {
                using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
                {
                    var parameters = new
                    {
                        is_default = isdefault,
                        token = token
                    };
                    var sql = @$"UPDATE [Client].[wp_woocommerce_payment_tokens]
                                SET is_default = @is_default
                                WHERE token = @token";

                    var pendingProductValues = await dbConnection.ExecuteAsync(sql, parameters);

                    return pendingProductValues;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int> GetUserDefaultCard(int associateID)
        {
            try
            {
                using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
                {
                    var parameters = new
                    {
                        associateID = associateID,
                       
                    };
                    var sql = @$"Select * from crm_Payments";

                    var pendingProductValues = await dbConnection.ExecuteAsync(sql, parameters);

                    return pendingProductValues;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<List<SendItAcademy_MatrixData>> GetSendItAcademy_MatrixData()
        {
            try
            {
                using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
                {
                    var sql = @$"select member as userID , sponsor as sponsorID, row_num as uplineLeg from [Client].[SendItAcademy_Matrix]";

                    var GetSendItAcademy_MatrixData = await dbConnection.QueryAsync<SendItAcademy_MatrixData>(sql);

                    return GetSendItAcademy_MatrixData.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<SendItAcademy_MatrixData>();
            }
        }
    }
}
