using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Common.PagingComon;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.LeagueService
{
    public class LeagueRepository : ILeagueRepository
    {
        private readonly IMongoCollection<League> _leagueCollection;
        private readonly IPagingComon _pagingComon;

        public LeagueRepository(IMongoDatabase database, IPagingComon pagingCommon)
        {
            _leagueCollection = database.GetCollection<League>("League");
            _pagingComon = pagingCommon;
        }
        public async Task<League> CreateLeague(League league)
        {
            await _leagueCollection.InsertOneAsync(league);
            return league;
        }

        //public Task<bool> DeleteLeague(string id)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<Respaging<League>> GetAllLeague(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            return await _pagingComon.GetAllData<League>(pageNumber, pageSize, searchName);
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
