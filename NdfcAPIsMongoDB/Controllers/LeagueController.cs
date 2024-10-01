using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Models.DTO;
using NdfcAPIsMongoDB.Repository.LeagueService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class LeagueController : BaseController
    {
        private readonly ILeagueRepository _leagueRepository;

        public LeagueController(ILeagueRepository leagueRepository, IMemoryCache cache, ILogger<BaseController> logger)
        : base(cache, logger)
        {
            _leagueRepository = leagueRepository;
        }

        /// <summary>
        /// get all list of tournaments the team participates in - no auth
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllLeague(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var league = await _leagueRepository.GetAllLeague(pageNumber, pageSize, searchName);
            return Ok(league);
        }

        /// <summary>
        /// get detail of league
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")] // Yêu cầu xác thực token để truy cập
        public async Task<IActionResult> GetLeagueById(string id)
        {
            // Kiểm tra thông tin tài khoản từ context.Items
            var accountId = HttpContext.Items["AccountId"]?.ToString();
            var email = HttpContext.Items["Email"]?.ToString();

            // Thực hiện các kiểm tra bổ sung với thông tin tài khoản (accountId, email)

            string cacheKey = $"League_{id}";

            var league = await GetFromCache(cacheKey, () => _leagueRepository.GetLeagueById(id));

            if (league == null)
            {
                return NotFound();
            }

            return Ok(league);
        }

        /// <summary>
        /// create new league
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateLeague(LeagueDTO leagueDto)
        {
            if (leagueDto == null)
            {
                return BadRequest();
            }
            // tạo một classDTO không bao gồm ID để mongoDB tự tạo
            var league = new League
            {
                Name = leagueDto.Name,
                Reward = leagueDto.Reward,
                Year = leagueDto.Year,
                Status = 1,
            };

            await _leagueRepository.CreateLeague(league);

            return Ok(league);
        }
    }
}
