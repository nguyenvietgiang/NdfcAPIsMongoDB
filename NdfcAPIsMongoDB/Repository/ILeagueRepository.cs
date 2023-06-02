using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository
{
    public interface ILeagueRepository
    {
        Task<Respaging<League>> GetAllLeague(int pageNumber = 1, int pageSize = 10, string? searchName = null);
        Task<League> GetLeagueById(string id);
        //Task<Player> CreateLeague(League league);
        //Task<bool> UpdateLeague(string id, League league); 
        //Task<bool> DeleteLeague(string id);
    }
}
