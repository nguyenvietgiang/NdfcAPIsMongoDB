using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.SubscribService
{
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly IMongoCollection<Subscriber> _subCollection;

        public SubscriberRepository(IMongoDatabase database)
        {
            _subCollection = database.GetCollection<Subscriber>("Subscriber");
        }
        public async Task<Respaging<Subscriber>> GetAllSubcriber(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var filter = Builders<Subscriber>.Filter.Empty;

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            // Tìm kiếm theo tên nếu có giá trị searchName được cung cấp
            if (!string.IsNullOrEmpty(searchName))
            {
                filter = Builders<Subscriber>.Filter.Regex(x => x.Name, new BsonRegularExpression(searchName, "i"));
            }

            // Thêm đoạn mã sau vào để bỏ qua điều kiện tìm kiếm khi searchName không được cung cấp
            if (string.IsNullOrEmpty(searchName))
            {
                filter = Builders<Subscriber>.Filter.Empty;
            }

            // Đếm tổng số bản ghi
            var totalRecords = await _subCollection.CountDocumentsAsync(filter);

            // Phân trang và lấy dữ liệu
            var Subscribers = await _subCollection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Tính toán số trang
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Tạo đối tượng Respaging để trả về
            var respaging = new Respaging<Subscriber>
            {
                currentPage = pageNumber,
                totalPages = totalPages,
                pageSize = pageSize,
                totalRecords = (int)totalRecords,
                content = Subscribers
            };

            return respaging;
        }

        public async Task<Subscriber> Subcribe(Subscriber subscriber)
        {
            await _subCollection.InsertOneAsync(subscriber);
            return subscriber;
        }
    }
}
