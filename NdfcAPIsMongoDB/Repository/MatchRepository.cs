using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository
{
    public class MatchRepository : IMatchRepository
    {
        private readonly IMongoCollection<Match> _matchCollection;

        public MatchRepository(IMongoDatabase database)
        {
            _matchCollection = database.GetCollection<Match>("Match");
        }

        public async Task<bool> DeleteMatch(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Match>.Filter.Eq("_id", objectId);
            var result = await _matchCollection.DeleteOneAsync(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<Respaging<Match>> GetAllMatch(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var filter = Builders<Match>.Filter.Empty;

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            // Tìm kiếm theo tên nếu có giá trị searchName được cung cấp
            if (!string.IsNullOrEmpty(searchName))
            {
                filter = Builders<Match>.Filter.Regex(x => x.Enemy, new BsonRegularExpression(searchName, "i"));
            }

            // Thêm đoạn mã sau vào để bỏ qua điều kiện tìm kiếm khi searchName không được cung cấp
            if (string.IsNullOrEmpty(searchName))
            {
                filter = Builders<Match>.Filter.Empty;
            }

            // Đếm tổng số bản ghi
            var totalRecords = await _matchCollection.CountDocumentsAsync(filter);

            // Phân trang và lấy dữ liệu
            var Matchs = await _matchCollection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Tính toán số trang
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Tạo đối tượng Respaging để trả về
            var respaging = new Respaging<Match>
            {
                currentPage = pageNumber,
                totalPages = totalPages,
                pageSize = pageSize,
                totalRecords = (int)totalRecords,
                content = Matchs
            };

            return respaging;
        }

        public async Task<Match> GetMatchById(string id)
        {
            // chuyển chuỗi string ID thành các ObjectId của mongoDB
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Match>.Filter.Eq("_id", objectId);
            return await _matchCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
