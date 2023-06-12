using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NdfcAPIsMongoDB.Repository.ReportService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class ReportController : BaseController
    {
        private readonly IReportRepository _ReportRepository;

        public ReportController(IReportRepository ReportRepository, IMemoryCache cache, ILogger<BaseController> logger)
        : base(cache, logger)
        {
            _ReportRepository = ReportRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllReport(int pageNumber = 1, int pageSize = 10, string? searchSeason = null)
        {
            var Report = await _ReportRepository.GetAllReports(pageNumber, pageSize, searchSeason);
            return Ok(Report);
        }
    }
}
