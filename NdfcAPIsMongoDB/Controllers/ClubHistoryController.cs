
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Repository.HistoryService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class ClubHistoryController : ControllerBase
    {
        private readonly IHistoryRepositorycs _historyRepository;

        public ClubHistoryController(IHistoryRepositorycs historyRepository)
        {
            _historyRepository = historyRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ClubHistory>> GetClubHistory()
        {
            var clubHistory = await _historyRepository.GetClubHistoryAsync();
            return Ok(clubHistory);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClubHistory([FromBody] ClubHistory clubHistory)
        {
            await _historyRepository.UpdateClubHistoryAsync(clubHistory);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteClubHistory()
        {
            await _historyRepository.DeleteClubHistoryAsync();
            return NoContent();
        }
    }
}
