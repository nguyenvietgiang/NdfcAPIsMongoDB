using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Repository.HistoryService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class ClubHistoryController : BaseController
    {
        private readonly IHistoryRepositorycs _historyRepository;

        public ClubHistoryController(IHistoryRepositorycs historyRepository, IMemoryCache cache, ILogger<BaseController> logger)
        : base(cache, logger)
        {
            _historyRepository = historyRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ClubHistory>>> GetClubHistory()
        {
            try
            {
                var clubHistory = await _historyRepository.GetClubHistoryAsync();
                return ApiOk(clubHistory);
            }
            catch (Exception ex)
            {
                return ApiException(ex);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<object>>> UpdateClubHistory([FromBody] ClubHistory clubHistory)
        {
            try
            {
                await _historyRepository.UpdateClubHistoryAsync(clubHistory);
                return ApiOk<object>(null, "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return ApiException(ex);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<object>>> DeleteClubHistory()
        {
            try
            {
                await _historyRepository.DeleteClubHistoryAsync();
                return ApiOk<object>(null, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return ApiException(ex);
            }
        }
    }

}
