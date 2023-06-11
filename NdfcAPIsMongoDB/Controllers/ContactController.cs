using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Repository.ContactService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContact _contactRepository;

        public ContactController(IContact contactRepository)
        {
            _contactRepository = contactRepository;
        }
        // Các phương thức xử lý yêu cầu HTTP ở đây
        [HttpGet]
        public async Task<IActionResult> GetAllContact(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var respaging = await _contactRepository.GetAllContact(pageNumber, pageSize, searchName);
            return Ok(respaging);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] ContactDTO contactDto)
        {
            if (contactDto == null)
            {
                return BadRequest();
            }
            // tạo một classDTO không bao gồm ID để mongoDB tự tạo
            var contact = new Contact
            {
                Name = contactDto.Name,
                Email = contactDto.Email,
                Topic = contactDto.Topic,
                Detail = contactDto.Detail,
            };

            await _contactRepository.CreateContact(contact);

            return Ok(contact);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(string id)
        {
            // Gọi phương thức FindPlayerById từ PlayerRepository để tìm cầu thủ theo ID
            var contact = await _contactRepository.GetContactById(id);

            if (contact == null)
            {
                return NotFound();
            }

            // Trả về kết quả thành công
            return Ok(contact);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteContact(string id)
        {
            var existingContact = await _contactRepository.GetContactById(id);
            if (existingContact == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
