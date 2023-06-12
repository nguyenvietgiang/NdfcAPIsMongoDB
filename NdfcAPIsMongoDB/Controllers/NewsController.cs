using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Models.DTO;
using NdfcAPIsMongoDB.Repository.NewsService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewById(string id)
        {
            var news = await _NewsRepository.GetNewById(id);

            if (news == null)
            {
                return NotFound();
            }

            // Trả về kết quả thành công
            return Ok(news);
        }

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
