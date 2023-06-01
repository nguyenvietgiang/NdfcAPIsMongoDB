using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository
{
    public interface ILeagueRepository
    {
        Task<List<League>> GetAllLeague();
        Task<League> GetLeagueById(string id);
        //Task<Player> CreateLeague(League league);
        //Task<bool> UpdateLeague(string id, League league); 
        //Task<bool> DeleteLeague(string id);
    }
}
