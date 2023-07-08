using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NdfcAPIsMongoDB.Repository.VideoService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class VideoController : BaseController
    {
        private readonly IVideoRepository _videoRepository;

        public VideoController(IVideoRepository VideoRepository, IMemoryCache cache, ILogger<BaseController> logger)
        : base(cache, logger)
        {
            _videoRepository = VideoRepository;
        }

        /// <summary>
        /// get all video - no auth
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllVideo(int pageNumber = 1, int pageSize = 10, string? searchTitle = null)
        {
            var Video = await _videoRepository.GetAllVideos(pageNumber, pageSize, searchTitle);
            return Ok(Video);
        }

        /// <summary>
        /// get video by id - no auth
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideoById(string id)
        {
            var Video = await _videoRepository.GetVideoById(id);

            if (Video == null)
            {
                return NotFound();
            }

            // Trả về kết quả thành công
            return Ok(Video);
        }

        /// <summary>
        /// delete video
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVideo(string id)
        {
            var existingVideo = await _videoRepository.GetVideoById(id);
            if (existingVideo == null)
            {
                return NotFound();
            }

            var deletedVideo = await _videoRepository.DeleteVideo(id);
            if (!deletedVideo)
            {
                return StatusCode(500, "An error occurred while deleting the Video.");
            }

            return NoContent();
        }
    }
}
