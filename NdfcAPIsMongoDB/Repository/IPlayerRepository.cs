using NdfcAPIsMongoDB.Models;
namespace NdfcAPIsMongoDB.Repository
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllPlayers();
        Task<Player> GetPlayerById(string id);
        Task<Player> CreatePlayer(Player player);
        Task<bool> UpdatePlayer(string id, Player player);
        Task<bool> DeletePlayer(string id);
    }
}
