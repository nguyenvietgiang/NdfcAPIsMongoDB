using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Models.DTO;
using NdfcAPIsMongoDB.Repository.TiketService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        public TicketController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        /// <summary>
        /// get all ticket
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tickets>>> GetTickets()
        {
            var tickets = await _ticketRepository.GetTickets();
            return Ok(tickets);
        }

        /// <summary>
        /// find one ticket
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Tickets>> GetTicketById(string id)
        {
            var ticket = await _ticketRepository.GetTicketById(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }

        /// <summary>
        /// user create a ticket
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Tickets>> AddTicket([FromBody] TicketDTO ticketDTO)
        {
            // Lấy thông tin người dùng từ HttpContext.User
            var userIdClaim = User.FindFirst("accountId");
            // Tạo một đối tượng Ticket từ DTO
            var newTicket = new Tickets
            {
                SeatId = ticketDTO.SeatId,
                UserId = userIdClaim.Value,
                price = 50000, // Giá mặc định
                Status = true // Trạng thái thanh toán mặc định
            };

            // Thêm vé mới và cập nhật trạng thái ghế
            var addedTicket = await _ticketRepository.AddTicket(newTicket);

            return CreatedAtAction(nameof(GetTicketById), new { id = addedTicket.Id }, addedTicket);
        }

        /// <summary>
        /// Delete a ticket
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTicket(string id)
        {
            var isDeleted = await _ticketRepository.DeleteTicket(id);
            if (isDeleted)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
