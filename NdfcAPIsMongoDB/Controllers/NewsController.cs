using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Models.DTO;
using NdfcAPIsMongoDB.Repository.NewsService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class NewsController : BaseController
    {
        private readonly INewsRepository _NewsRepository;

        public NewsController(INewsRepository NewsRepository, IMemoryCache cache, ILogger<BaseController> logger)
        : base(cache, logger)
        {
            _NewsRepository = NewsRepository;
        }

        /// <summary>
        /// get all news- no auth
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllNews(int pageNumber = 1, int pageSize = 10, string? searchTitle = null, string? sortField = "CreateOn", int sortOrder = 1)
        {
            var News = await _NewsRepository.GetAllNews(pageNumber, pageSize, searchTitle, sortField, sortOrder);
            return Ok(News);
        }

        /// <summary>
        /// get news by id - no auth
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewById(string id)
        {
            string cacheKey = $"New_{id}";

            var news = await GetFromCache(cacheKey, () => _NewsRepository.GetNewById(id));

            if (news == null)
            {
                return NotFound();
            }

            return Ok(news);
        }

        /// <summary>
        /// create news
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNew([FromForm] NewsDTO newsDTO)
        {
            if (newsDTO == null)
            {
                return BadRequest();
            }

            // tạo một classDTO không bao gồm ID để mongoDB tự tạo
            var news = new News
            {
                Title = newsDTO.Title,
                Description = newsDTO.Description,
                Detail = newsDTO.Detail,
                CreateOn = DateTime.Now,
                Status =1 
            };

            // Truyền giá trị host từ HttpContext.Request.Host.ToString()
            var host = HttpContext.Request.Host.ToString();
            await _NewsRepository.CreateNew(news, newsDTO.Image, host);

            return Ok(news);
        }

        /// <summary>
        /// delete news by id
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNew(string id)
        {
            var existingNew = await _NewsRepository.GetNewById(id);
            if (existingNew == null)
            {
                return NotFound();
            }

            var deletedNew = await _NewsRepository.DeleteNew(id);
            if (!deletedNew)
            {
                return StatusCode(500, "An error occurred while deleting the New.");
            }

            return NoContent();
        }

        /// <summary>
        /// delete many news by list id
        /// </summary>
        [HttpDelete("list-id")]
        public async Task<IActionResult> DeleteNews(List<string> ids)
        {
            if (ids == null || ids.Count == 0) 
            {
                return BadRequest("List of IDs is required.");
            }

            var deleted = await _NewsRepository.DeleteNews(ids);
            if (deleted)
            {
                return Ok("News deleted successfully.");
            }
            else
            {
                return StatusCode(500, "An error occurred while deleting News.");
            }
        }
    }
}
