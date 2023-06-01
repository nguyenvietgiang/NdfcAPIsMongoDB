using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerRepository : IPlayerRepository
{
    private readonly IMongoCollection<Player> _playerCollection;

    public PlayerRepository(IMongoDatabase database)
    {
        _playerCollection = database.GetCollection<Player>("Player");
    }

    public async Task<List<Player>> GetAllPlayers()
    {
        return await _playerCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Player> GetPlayerById(string id)
    {
        // chuyển chuỗi string ID thành các ObjectId của mongoDB
        var objectId = ObjectId.Parse(id);
        var filter = Builders<Player>.Filter.Eq("_id", objectId);
        return await _playerCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Player> CreatePlayer(Player player)
    {
        await _playerCollection.InsertOneAsync(player);
        return player;
    }

    public async Task<bool> UpdatePlayer(string id, Player player)
    {
        var objectId = ObjectId.Parse(id);
        var filter = Builders<Player>.Filter.Eq("_id", objectId);
        var result = await _playerCollection.ReplaceOneAsync(filter, player);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
    public async Task<bool> DeletePlayer(string id)
    {
        var objectId = ObjectId.Parse(id);
        var filter = Builders<Player>.Filter.Eq("_id", objectId);
        var result = await _playerCollection.DeleteOneAsync(filter);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

}

