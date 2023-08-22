using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NdfcAPIsMongoDB.Common.ElasticSearch;
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
        private readonly IElasticsearchService _elasticsearchService;
        public PlayerController(IPlayerRepository playerRepository, IMemoryCache cache, ILogger<BaseController> logger, IElasticsearchService elasticsearchService)
        : base(cache, logger)
        {
            _playerRepository = playerRepository;
            _elasticsearchService = elasticsearchService;
        }

        /// <summary>
        /// get all list of player - no auth
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllPlayers(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var respaging = await _playerRepository.GetAllPlayers(pageNumber, pageSize, searchName);
            return Ok(respaging);
        }

        /// <summary>
        /// get player by id - no auth
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayerById(string id)
        {
            string cacheKey = $"Player_{id}";

            var player = await GetFromCache(cacheKey, () => _playerRepository.GetPlayerById(id));

            if (player == null)
            {
                return NotFound();
            }

            return Ok(player);
        }

        /// <summary>
        /// admin create new player profile
        /// </summary>
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

        /// <summary>
        /// update player profile
        /// </summary>
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

        /// <summary>
        /// patch player profile
        /// </summary>
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

        /// <summary>
        /// admin delete player
        /// </summary>
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

        /// <summary>
        /// delete many player by list id
        /// </summary>
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

        [HttpGet("elastic-search")]
        public IActionResult SearchPlayers(string query)
        {
            var client = _elasticsearchService.GetClient();

            var searchResponse = client.Search<Player>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(query)
                    )
                )
            );

            return Ok(searchResponse.Documents);
        }

    }

}
