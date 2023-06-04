using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Repository;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsRepository _NewsRepository;

        public NewsController(INewsRepository NewsRepository)
        {
            _NewsRepository = NewsRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllNews(int pageNumber = 1, int pageSize = 10, string? searchTitle = null)
        {
            var News = await _NewsRepository.GetAllNews(pageNumber, pageSize, searchTitle);
            return Ok(News);
        }
    }
}
