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

    public async Task<Player> CreatePlayer(Player player, IFormFile image, string host)
    {
        // Lưu tệp ảnh và lấy đường dẫn tới nó
        if (image != null)
        {
            var imagePath = SaveImage(image, host);
            player.sImg = imagePath;
        }

        await _playerCollection.InsertOneAsync(player);
        return player;
    }

    // lưu avatar của cầu thủ
    public string SaveImage(IFormFile image, string host)
    {
        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileExtension = Path.GetExtension(image.FileName);
        if (!validExtensions.Contains(fileExtension.ToLower()))
        {
            throw new ArgumentException("File truyền vào phải là ảnh.");
        }

        var fileName = Guid.NewGuid().ToString() + fileExtension;
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            image.CopyTo(stream);
        }

        var imageUrl = $"{host}/{Path.Combine("uploads", fileName)}";
        return imageUrl;
    }

    public async Task<bool> UpdatePlayer(string id, Player player, IFormFile image, string host)
    {
        var objectId = ObjectId.Parse(id);
        var filter = Builders<Player>.Filter.Eq("_id", objectId);

        if (image != null)
        {
            var imagePath = SaveImage(image, host);
            player.sImg = imagePath;
        }

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

    public Task<Player> CreatePlayer(Player player)
    {
        throw new NotImplementedException();
    }
}

