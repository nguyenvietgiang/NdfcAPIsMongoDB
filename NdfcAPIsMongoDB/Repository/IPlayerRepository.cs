using NdfcAPIsMongoDB.Models;
namespace NdfcAPIsMongoDB.Repository
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllPlayers();
        Task<Player> GetPlayerById(string id);
        Task<Player> CreatePlayer(Player player, IFormFile image, string host);
        string SaveImage(IFormFile image, string host);
        Task<bool> UpdatePlayer(string id, Player player, IFormFile image, string host);
        Task<bool> DeletePlayer(string id);
    }
}
