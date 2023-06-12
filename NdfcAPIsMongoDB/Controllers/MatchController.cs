using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NdfcAPIsMongoDB.Repository.MatchService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class MatchController : BaseController
    {
        private readonly IMatchRepository _matchRepository;

        public MatchController(IMatchRepository matchRepository, IMemoryCache cache, ILogger<BaseController> logger)
        : base(cache, logger)
        {
            _matchRepository = matchRepository;
        }
        // Các phương thức xử lý yêu cầu HTTP ở đây
        [HttpGet]
        public async Task<IActionResult> GetAllMatch(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var match = await _matchRepository.GetAllMatch(pageNumber, pageSize, searchName);
            return Ok(match);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMatch(string id)
        {
            var existingMatch = await _matchRepository.GetMatchById(id);
            if (existingMatch == null)
            {
                return NotFound();
            }

            var deletedPlayer = await _matchRepository.DeleteMatch(id);
            if (!deletedPlayer)
            {
                return StatusCode(500, "An error occurred while deleting the player.");
            }

            return NoContent();
        }
    }
}
