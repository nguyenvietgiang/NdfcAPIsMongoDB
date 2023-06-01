using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Repository;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAllLeague() 
        {
            var league = await _leagueRepository.GetAllLeague();
            return Ok(league);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeagueById(string id)
        {
            var player = await _leagueRepository.GetLeagueById(id);

            if (player == null)
            {
                return NotFound();
            }

            // Trả về kết quả thành công
            return Ok(player);
        }

    }
}
