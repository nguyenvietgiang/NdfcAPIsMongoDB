using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Repository;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoRepository _videoRepository;

        public VideoController(IVideoRepository VideoRepository)
        {
            _videoRepository = VideoRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllVideo(int pageNumber = 1, int pageSize = 10, string? searchTitle = null)
        {
            var Video = await _videoRepository.GetAllVideos(pageNumber, pageSize, searchTitle);
            return Ok(Video);
        }
    }
}
