using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;
using System.Text.RegularExpressions;

namespace NdfcAPIsMongoDB.Repository.SliderService
{
    public class SliderRepository : ISliderRepository
    {
        private readonly IMongoCollection<Slider> _sliderCollection;

        public SliderRepository(IMongoDatabase database)
        {
            _sliderCollection = database.GetCollection<Slider>("Slider");
        }

        public async Task<bool> DeleteSlider(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Slider>.Filter.Eq("_id", objectId);
            var result = await _sliderCollection.DeleteOneAsync(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<Respaging<Slider>> GetAllSliders(int pageNumber = 1, int pageSize = 10, string? searchTitle = null)
        {
            var filter = Builders<Slider>.Filter.Empty;

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            // Tìm kiếm theo tên nếu có giá trị searchTitle được cung cấp
            if (!string.IsNullOrEmpty(searchTitle))
            {
                filter = Builders<Slider>.Filter.Regex(x => x.Title, new BsonRegularExpression(searchTitle, "i"));
            }

            // Thêm đoạn mã sau vào để bỏ qua điều kiện tìm kiếm khi searchTitle không được cung cấp
            if (string.IsNullOrEmpty(searchTitle))
            {
                filter = Builders<Slider>.Filter.Empty;
            }

            // Đếm tổng số bản ghi
            var totalRecords = await _sliderCollection.CountDocumentsAsync(filter);

            // Phân trang và lấy dữ liệu
            var Sliders = await _sliderCollection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Tính toán số trang
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Tạo đối tượng Respaging để trả về
            var respaging = new Respaging<Slider>
            {
                currentPage = pageNumber,
                totalPages = totalPages,
                pageSize = pageSize,
                totalRecords = (int)totalRecords,
                content = Sliders
            };

            return respaging;
        }

        public async Task<Slider> GetSliderById(string id)
        {
            // chuyển chuỗi string ID thành các ObjectId của mongoDB
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Slider>.Filter.Eq("_id", objectId);
            return await _sliderCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
