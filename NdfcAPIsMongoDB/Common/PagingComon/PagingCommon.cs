using MongoDB.Bson;
using MongoDB.Driver;

namespace NdfcAPIsMongoDB.Common.PagingComon
{
    public class PagingCommon : IPagingComon
    {
        private readonly IMongoDatabase _database;

        public PagingCommon(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<Respaging<T>> GetAllData<T>(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var filter = Builders<T>.Filter.Empty;

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            // Tìm kiếm theo tên nếu có giá trị searchName được cung cấp
            if (!string.IsNullOrEmpty(searchName))
            {
                filter = Builders<T>.Filter.Regex("Name", new BsonRegularExpression(searchName, "i"));
            }

            // Thêm đoạn mã sau vào để bỏ qua điều kiện tìm kiếm khi searchName không được cung cấp
            if (string.IsNullOrEmpty(searchName))
            {
                filter = Builders<T>.Filter.Empty;
            }

            // Đếm tổng số bản ghi
            var totalRecords = await GetCollection<T>().CountDocumentsAsync(filter);

            // Phân trang và lấy dữ liệu
            var data = await GetCollection<T>().Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Tính toán số trang
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Tạo đối tượng Respaging để trả về
            var respaging = new Respaging<T>
            {
                currentPage = pageNumber,
                totalPages = totalPages,
                pageSize = pageSize,
                totalRecords = (int)totalRecords,
                content = data
            };

            // Thêm thông tin về next và prev
            if (pageNumber < totalPages)
            {
                respaging.next = $"http://localhost:5107/v1/api/?pageNumber={pageNumber + 1}";
            }

            if (pageNumber > 1)
            {
                respaging.prev = $"http://localhost:5107/v1/api/?pageNumber={pageNumber - 1}";
            }

            return respaging;
        }

        private IMongoCollection<T> GetCollection<T>()
        {
            // Lấy collection tương ứng với kiểu dữ liệu T
            return _database.GetCollection<T>(typeof(T).Name);
        }
    }
}
