using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository
{
    public class LeagueRepository : ILeagueRepository
    {
        private readonly IMongoCollection<League> _leagueCollection; 

        public LeagueRepository(IMongoDatabase database) 
        {
            _leagueCollection = database.GetCollection<League>("League");
        }
        //public Task<Player> CreateLeague(League league)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> DeleteLeague(string id)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<List<League>> GetAllLeague() 
        {
            return await _leagueCollection.Find(_ => true).ToListAsync();
        }

        public async Task<League> GetLeagueById(string id)
        {
            // chuyển chuỗi string ID thành các ObjectId của mongoDB
            var objectId = ObjectId.Parse(id);
            var filter = Builders<League>.Filter.Eq("_id", objectId);
            return await _leagueCollection.Find(filter).FirstOrDefaultAsync();
        }

        //public Task<bool> UpdateLeague(string id, League league)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
