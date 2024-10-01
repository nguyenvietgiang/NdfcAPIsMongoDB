using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.NewsService
{
    public class NewsRepository : INewsRepository
    {
        private readonly IMongoCollection<News> _newsCollection;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NewsRepository(IMongoDatabase database, IHttpContextAccessor httpContextAccessor)
        {
            _newsCollection = database.GetCollection<News>("News");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<News> CreateNew(News news, IFormFile image, string host)
        {
            // Lưu tệp ảnh và lấy đường dẫn tới nó
            if (image != null)
            {
                var imagePath = SaveImage(image, host);
                news.Image = imagePath;
            }

            await _newsCollection.InsertOneAsync(news);
            return news;
        }


        public async Task<bool> DeleteNew(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<News>.Filter.Eq("_id", objectId);
            var result = await _newsCollection.DeleteOneAsync(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<bool> DeleteNews(List<string> ids)
        {
            foreach (var id in ids)
            {
                var objectId = ObjectId.Parse(id);
                var filter = Builders<News>.Filter.Eq("_id", objectId);
                var result = await _newsCollection.DeleteOneAsync(filter);
            }
            return true;
        }

        public async Task<Respaging<News>> GetAllNews(int pageNumber = 1, int pageSize = 10, string? searchTitle = null, string sortField = "CreateOn", int sortOrder = 1)
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

            // Tạo đối tượng SortDefinition để xác định sắp xếp
            var sortDefinition = Builders<News>.Sort
                .Ascending(sortField)
                .Descending(sortField);

            if (sortOrder == -1)
            {
                sortDefinition = sortDefinition.Descending(sortField);
            }
            else
            {
                sortDefinition = sortDefinition.Ascending(sortField);
            }

            // Phân trang và lấy dữ liệu
            var Newss = await _newsCollection.Find(filter)
                .Sort(sortDefinition)
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


        public async Task<News> GetNewById(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<News>.Filter.Eq("_id", objectId);
            return await _newsCollection.Find(filter).FirstOrDefaultAsync();
        }

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


    }
}
