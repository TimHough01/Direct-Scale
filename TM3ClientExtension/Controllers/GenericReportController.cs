using Microsoft.AspNetCore.Mvc;
using System.Text;
using System;
using WebExtension.Helper;
using WebExtension.Services;
using Microsoft.Extensions.Logging;
using WebExtension.Models.Client_Requests;
using WebExtension.Models.GenericReports;
using DirectScale.Disco.Extension.Services;

namespace WebExtension.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
    public class GenericReportController : ControllerBase
    {
        private readonly IGenericReportService _genericReportService;
        private readonly ILogger<GenericReportController> _logger;
        private readonly IDataService _dataService;
        public GenericReportController(IGenericReportService genericReportService, ILogger<GenericReportController> logger, IDataService dataService)
        {
            _genericReportService = genericReportService ?? throw new ArgumentNullException(nameof(genericReportService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataService = dataService;
        }
        [HttpPost]
        [Route("GetGenericReportDetails")]
        public IActionResult GetGenericReport([FromBody] GetGenericReportRequest request)
        {
            var logMessage = new StringBuilder("GetGenericReport method called\n");
            try
            {
                string replaceChars = "";
                if (request.ReplaceChars == null)
                    replaceChars = "";
                else
                    replaceChars = request.ReplaceChars;

                var reportDetailItems = _genericReportService.GetReportDetails(request.ReportId, replaceChars);
                return new Responses().OkResult(reportDetailItems);
            }
            catch (Exception e)
            {
                var model = new QueryResult();
                logMessage.AppendLine("Failed calling _genericReportService.GetReportDetails(" + request.ReportId.ToString() + ")");
                logMessage.AppendLine(e.Message);
                logMessage.AppendLine(e.StackTrace);
                _logger.LogError(logMessage.ToString(), null);
                return new Responses().BadRequestResult(model);
            }
        }
        [HttpGet]
        [Route("GetDatabaseConnection")]
        public IActionResult GetDatabaseConnection()
        {
            var dbConnection = new System.Data.SqlClient.SqlConnection(_dataService.GetClientConnectionString().Result);
            return Ok(dbConnection.ConnectionString);
        }
    }
}
