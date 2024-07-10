using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Repository.LogService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly LogService _logService;

        public LogsController(LogService logService)
        {
            _logService = logService;
        }


        /// <summary>
        /// get logs from elasticsearch (run elasticsearch first)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLogs(string query = "*", int page = 1, int pageSize = 10)
        {
            var logs = await _logService.GetLogsAsync(query, page, pageSize);
            return Ok(logs.Documents);
        }
    }
}
