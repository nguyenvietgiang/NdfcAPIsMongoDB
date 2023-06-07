using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Repository;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly ILeagueRepository _leagueRepository;

        public LeagueController(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }
        // Các phương thức xử lý yêu cầu HTTP ở đây
        [HttpGet]
        public async Task<IActionResult> GetAllLeague(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var league = await _leagueRepository.GetAllLeague(pageNumber, pageSize, searchName);
            return Ok(league);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")] // Yêu cầu xác thực token để truy cập
        public async Task<IActionResult> GetLeagueById(string id)
        {
            // Kiểm tra thông tin tài khoản từ context.Items
            var accountId = HttpContext.Items["AccountId"]?.ToString();
            var email = HttpContext.Items["Email"]?.ToString();

            // Thực hiện các kiểm tra bổ sung với thông tin tài khoản (accountId, email)

            var league = await _leagueRepository.GetLeagueById(id);

            if (league == null)
            {
                return NotFound();
            }

            return Ok(league);
        }

    }
}
