using Microsoft.AspNetCore.JsonPatch;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.PlayerService
{
    public interface IPlayerRepository
    {
        Task<Respaging<Player>> GetAllPlayers(int pageNumber = 1, int pageSize = 10, string? searchName = null);
        Task<Player> GetPlayerById(string id);
        Task<Player> CreatePlayer(Player player, IFormFile image, string host);
        string SaveImage(IFormFile image, string host);
        Task<bool> UpdatePlayer(string id, Player player, IFormFile image, string host);
        Task<bool> DeletePlayer(string id);
        Task<bool> DeletePlayers(List<string> ids);
        Task<bool> PatchPlayer(string id, JsonPatchDocument<Player> playerPatch);
        Task<Player> GetRandomPlayer();
    }
}
