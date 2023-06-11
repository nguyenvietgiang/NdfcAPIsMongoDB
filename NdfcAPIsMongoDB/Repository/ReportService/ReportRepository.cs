using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.ReportService
{
    public class ReportRepository : IReportRepository
    {
        private readonly IMongoCollection<Report> _reportCollection;

        public ReportRepository(IMongoDatabase database)
        {
            _reportCollection = database.GetCollection<Report>("Report");
        }
        public async Task<Respaging<Report>> GetAllReports(int pageNumber = 1, int pageSize = 10, string? searchSeason = null)
        {
            var filter = Builders<Report>.Filter.Empty;

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            // Tìm kiếm theo tên nếu có giá trị searchSeason được cung cấp
            if (!string.IsNullOrEmpty(searchSeason))
            {
                filter = Builders<Report>.Filter.Regex(x => x.Season, new BsonRegularExpression(searchSeason, "i"));
            }

            // Thêm đoạn mã sau vào để bỏ qua điều kiện tìm kiếm khi searchSeason không được cung cấp
            if (string.IsNullOrEmpty(searchSeason))
            {
                filter = Builders<Report>.Filter.Empty;
            }

            // Đếm tổng số bản ghi
            var totalRecords = await _reportCollection.CountDocumentsAsync(filter);

            // Phân trang và lấy dữ liệu
            var Reports = await _reportCollection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Tính toán số trang
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Tạo đối tượng Respaging để trả về
            var respaging = new Respaging<Report>
            {
                currentPage = pageNumber,
                totalPages = totalPages,
                pageSize = pageSize,
                totalRecords = (int)totalRecords,
                content = Reports
            };

            return respaging;
        }
    }
}
