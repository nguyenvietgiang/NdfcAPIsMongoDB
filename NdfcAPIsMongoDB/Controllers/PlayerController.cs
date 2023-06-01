using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Repository;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerController(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }
        // Các phương thức xử lý yêu cầu HTTP ở đây
        [HttpGet]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _playerRepository.GetAllPlayers();
            return Ok(players);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayerById(string id)
        {
            // Gọi phương thức FindPlayerById từ PlayerRepository để tìm cầu thủ theo ID
            var player = await _playerRepository.GetPlayerById(id);

            if (player == null)
            {
                return NotFound();
            }

            // Trả về kết quả thành công
            return Ok(player);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] PlayerDto playerDto)
        {
            if (playerDto == null)
            {
                return BadRequest();
            }
            // tạo một classDTO không bao gồm ID để mongoDB tự tạo
            var player = new Player
            {
                Name = playerDto.sName,
                Age = playerDto.iAge,
                Role = playerDto.sRole,
                Position = playerDto.sPosition
            };

            await _playerRepository.CreatePlayer(player);

            return Ok(player);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(string id, [FromBody] Player player)
        {
            if (player == null)
            {
                return BadRequest();
            }

            var existingPlayer = await _playerRepository.GetPlayerById(id);
            if (existingPlayer == null)
            {
                return NotFound();
            }

            existingPlayer.Name = player.Name;
            existingPlayer.Age = player.Age;
            existingPlayer.Role = player.Role;
            existingPlayer.Position = player.Position;

            var updatedPlayer = await _playerRepository.UpdatePlayer(id, existingPlayer);
            if (!updatedPlayer)
            {
                return StatusCode(500, "An error occurred while updating the player.");
            }

            return Ok(existingPlayer);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(string id)
        {
            var existingPlayer = await _playerRepository.GetPlayerById(id);
            if (existingPlayer == null)
            {
                return NotFound();
            }

            var deletedPlayer = await _playerRepository.DeletePlayer(id);
            if (!deletedPlayer)
            {
                return StatusCode(500, "An error occurred while deleting the player.");
            }

            return NoContent();
        }

    }

}
