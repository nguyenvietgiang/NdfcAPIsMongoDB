using Microsoft.AspNetCore.JsonPatch;
using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Common.PagingComon;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Repository.PlayerService;

public class PlayerRepository : IPlayerRepository
{
    private readonly IMongoCollection<Player> _playerCollection;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPagingComon _pagingComon;
    public PlayerRepository(IMongoDatabase database, IHttpContextAccessor httpContextAccessor, IPagingComon pagingComon)
    {
        _playerCollection = database.GetCollection<Player>("Player");
        _httpContextAccessor = httpContextAccessor;
        _pagingComon = pagingComon;
    }

    public async Task<Respaging<Player>> GetAllPlayers(int pageNumber = 1, int pageSize = 10, string? searchName = null)
    {
        return await _pagingComon.GetAllData<Player>(pageNumber, pageSize, searchName);
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
        var scheme = _httpContextAccessor.HttpContext.Request.Scheme;
        var imageUrl = $"{scheme}://{host}/uploads/{fileName}";
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

    public async Task<bool> PatchPlayer(string id, JsonPatchDocument<Player> playerPatch)
    {
        var objectId = ObjectId.Parse(id);
        var filter = Builders<Player>.Filter.Eq("_id", objectId);
        var player = await _playerCollection.Find(filter).FirstOrDefaultAsync();

        if (player == null)
        {
            return false;
        }

        playerPatch.ApplyTo(player);

        var result = await _playerCollection.ReplaceOneAsync(filter, player);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeletePlayers(List<string> ids)
    {
        foreach (var id in ids)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Player>.Filter.Eq("_id", objectId);
            var result = await _playerCollection.DeleteOneAsync(filter);
        }
        return true;
    }

    public async Task<Player> GetRandomPlayer()
    {
        // Lấy tổng số lượng cầu thủ
        var totalPlayers = await _playerCollection.CountDocumentsAsync(Builders<Player>.Filter.Empty);

        if (totalPlayers == 0)
        {
            return null; // Không có cầu thủ nào trong cơ sở dữ liệu
        }

        // Tạo một số nguyên ngẫu nhiên từ 0 đến (tổng số lượng cầu thủ - 1)
        var randomIndex = new Random().Next((int)totalPlayers);

        // Lấy một cầu thủ ngẫu nhiên
        var randomPlayer = await _playerCollection.Find(Builders<Player>.Filter.Empty)
            .Skip(randomIndex)
            .Limit(1)
            .FirstOrDefaultAsync();

        return randomPlayer;
    }
}


