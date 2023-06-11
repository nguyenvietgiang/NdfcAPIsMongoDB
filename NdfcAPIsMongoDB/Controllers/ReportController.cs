using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Repository.ReportService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _ReportRepository;

        public ReportController(IReportRepository ReportRepository)
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
