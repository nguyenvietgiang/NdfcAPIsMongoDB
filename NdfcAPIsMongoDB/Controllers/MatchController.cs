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

        /// <summary>
        /// get a match - no auth
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMatch(int pageNumber = 1, int pageSize = 10, string? searchName = null, DateTime? searchDate = null)
        {
            var matches = await _matchRepository.GetAllMatch(pageNumber, pageSize, searchName, searchDate);
            return Ok(matches);
        }

        /// <summary>
        /// get detail match by id - no auth
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatchById(string id)
        {
            var match = await _matchRepository.GetMatchById(id);
            if (match == null)
            {
                return NotFound();
            }

            return Ok(match);
        }

        /// <summary>
        /// delete match
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMatch(string id)
        {
            var existingMatch = await _matchRepository.GetMatchById(id);
            if (existingMatch == null)
            {
                return NotFound();
            }

            var deletedMatch = await _matchRepository.DeleteMatch(id);
            if (!deletedMatch)
            {
                return StatusCode(500, "An error occurred while deleting the match.");
            }

            return NoContent();
        }
    }
}
