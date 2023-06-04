using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository
{
    public class NewsRepository : INewsRepository
    {
        private readonly IMongoCollection<News> _newsCollection;

        public NewsRepository(IMongoDatabase database)
        {
            _newsCollection = database.GetCollection<News>("News");
        }
        public async Task<Respaging<News>> GetAllNews(int pageNumber = 1, int pageSize = 10, string? searchTitle = null)
        {
            var filter = Builders<News>.Filter.Empty;

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            // Tìm kiếm theo tên nếu có giá trị searchTitle được cung cấp
            if (!string.IsNullOrEmpty(searchTitle))
            {
                filter = Builders<News>.Filter.Regex(x => x.Title, new BsonRegularExpression(searchTitle, "i"));
            }

            // Thêm đoạn mã sau vào để bỏ qua điều kiện tìm kiếm khi searchTitle không được cung cấp
            if (string.IsNullOrEmpty(searchTitle))
            {
                filter = Builders<News>.Filter.Empty;
            }

            // Đếm tổng số bản ghi
            var totalRecords = await _newsCollection.CountDocumentsAsync(filter);

            // Phân trang và lấy dữ liệu
            var Newss = await _newsCollection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Tính toán số trang
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Tạo đối tượng Respaging để trả về
            var respaging = new Respaging<News>
            {
                currentPage = pageNumber,
                totalPages = totalPages,
                pageSize = pageSize,
                totalRecords = (int)totalRecords,
                content = Newss
            };

            return respaging;
        }
    }
}
