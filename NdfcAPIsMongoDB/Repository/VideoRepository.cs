using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository
{
    public class VideoRepository : IVideoRepository
    {
        private readonly IMongoCollection<Video> _videoCollection;

        public VideoRepository(IMongoDatabase database)
        {
            _videoCollection = database.GetCollection<Video>("Video");
        }
        public async Task<Respaging<Video>> GetAllVideos(int pageNumber = 1, int pageSize = 10, string? searchTitle = null)
        {
            var filter = Builders<Video>.Filter.Empty;

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            // Tìm kiếm theo tên nếu có giá trị searchTitle được cung cấp
            if (!string.IsNullOrEmpty(searchTitle))
            {
                filter = Builders<Video>.Filter.Regex(x => x.Title, new BsonRegularExpression(searchTitle, "i"));
            }

            // Thêm đoạn mã sau vào để bỏ qua điều kiện tìm kiếm khi searchTitle không được cung cấp
            if (string.IsNullOrEmpty(searchTitle))
            {
                filter = Builders<Video>.Filter.Empty;
            }

            // Đếm tổng số bản ghi
            var totalRecords = await _videoCollection.CountDocumentsAsync(filter);

            // Phân trang và lấy dữ liệu
            var Videos = await _videoCollection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Tính toán số trang
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Tạo đối tượng Respaging để trả về
            var respaging = new Respaging<Video>
            {
                currentPage = pageNumber,
                totalPages = totalPages,
                pageSize = pageSize,
                totalRecords = (int)totalRecords,
                content = Videos
            };

            return respaging;
        }
    }
}
