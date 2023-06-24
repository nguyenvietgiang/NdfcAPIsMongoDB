using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.MatchService
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

        public async Task<Respaging<Match>> GetAllMatch(int pageNumber = 1, int pageSize = 10, string? searchName = null, DateTime? searchDate = null)
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

            // Tìm kiếm theo ngày nếu có giá trị searchDate được cung cấp
            if (searchDate.HasValue)
            {
                // Tạo một khoảng thời gian từ ngày bắt đầu đến ngày kết thúc của searchDate
                var startDate = searchDate.Value.Date;
                var endDate = searchDate.Value.Date.AddDays(1).AddTicks(-1);

                // Áp dụng điều kiện tìm kiếm theo khoảng thời gian
                var dateFilter = Builders<Match>.Filter.Gte(x => x.Time, startDate) & Builders<Match>.Filter.Lte(x => x.Time, endDate);
                filter &= dateFilter;
            }

            // Thêm đoạn mã sau vào để bỏ qua điều kiện tìm kiếm khi không có trường sắp xếp được chỉ định
            if (string.IsNullOrEmpty(searchName) && !searchDate.HasValue)
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
