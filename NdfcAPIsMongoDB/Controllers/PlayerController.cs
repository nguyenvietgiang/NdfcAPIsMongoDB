using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Models.DTO;
using NdfcAPIsMongoDB.Repository.PlayerService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class PlayerController : BaseController
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerController(IPlayerRepository playerRepository, IMemoryCache cache, ILogger<BaseController> logger)
        : base(cache, logger)
        {
            _playerRepository = playerRepository;
        }
        // Các phương thức xử lý yêu cầu HTTP ở đây
        [HttpGet]
        public async Task<IActionResult> GetAllPlayers(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var respaging = await _playerRepository.GetAllPlayers(pageNumber, pageSize, searchName);
            return Ok(respaging);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePlayer([FromForm] PlayerDto playerDto)
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
                Position = playerDto.sPosition,
                Status = "Bình thường",
                Scrored =0,
                RedCard =0
            };

            // Truyền giá trị host từ HttpContext.Request.Host.ToString()
            var host = HttpContext.Request.Host.ToString();
            await _playerRepository.CreatePlayer(player, playerDto.Image, host);

            return Ok(player);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(string id, [FromForm] PlayerDto playerDto)
        {
            if (playerDto == null)
            {
                return BadRequest();
            }

            var existingPlayer = await _playerRepository.GetPlayerById(id);
            if (existingPlayer == null)
            {
                return NotFound();
            }

            existingPlayer.Name = playerDto.sName;
            existingPlayer.Age = playerDto.iAge;
            existingPlayer.Role = playerDto.sRole;
            existingPlayer.Position = playerDto.sPosition;

            var host = HttpContext.Request.Host.ToString();
            var updatedPlayer = await _playerRepository.UpdatePlayer(id, existingPlayer, playerDto.Image, host);
            if (!updatedPlayer)
            {
                return StatusCode(500, "An error occurred while updating the player.");
            }

            return Ok(existingPlayer);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchPlayer(string id, [FromBody] JsonPatchDocument<Player> playerPatch)
        {
            var player = await _playerRepository.GetPlayerById(id);
            if (player == null)
            {
                return NotFound("Player not found.");
            }

            var success = await _playerRepository.PatchPlayer(id, playerPatch);
            if (success)
            {
                var patchedPlayer = await _playerRepository.GetPlayerById(id); // Lấy bản ghi đã được áp dụng các thay đổi PATCH
                return Ok(patchedPlayer);
            }
            else
            {
                return BadRequest("Failed to apply patch to the player.");
            }
        }


        [HttpDelete("{id}")]
        [Authorize]
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

        [HttpDelete("list-id")]
        public async Task<IActionResult> DeletePlayers(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("List of IDs is required.");
            }

            var deleted = await _playerRepository.DeletePlayers(ids);
            if (deleted)
            {
                return Ok("Players deleted successfully.");
            }
            else
            {
                return StatusCode(500, "An error occurred while deleting players.");
            }
        }

    }

}
