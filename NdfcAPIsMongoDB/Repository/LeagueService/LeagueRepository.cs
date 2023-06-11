using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.LeagueService
{
    public class LeagueRepository : ILeagueRepository
    {
        private readonly IMongoCollection<League> _leagueCollection;

        public LeagueRepository(IMongoDatabase database)
        {
            _leagueCollection = database.GetCollection<League>("League");
        }
        //public Task<League> CreateLeague(League league)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> DeleteLeague(string id)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<Respaging<League>> GetAllLeague(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var filter = Builders<League>.Filter.Empty;

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            // Tìm kiếm theo tên nếu có giá trị searchName được cung cấp
            if (!string.IsNullOrEmpty(searchName))
            {
                filter = Builders<League>.Filter.Regex(x => x.Name, new BsonRegularExpression(searchName, "i"));
            }

            // Thêm đoạn mã sau vào để bỏ qua điều kiện tìm kiếm khi searchName không được cung cấp
            if (string.IsNullOrEmpty(searchName))
            {
                filter = Builders<League>.Filter.Empty;
            }

            // Đếm tổng số bản ghi
            var totalRecords = await _leagueCollection.CountDocumentsAsync(filter);

            // Phân trang và lấy dữ liệu
            var Leagues = await _leagueCollection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Tính toán số trang
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Tạo đối tượng Respaging để trả về
            var respaging = new Respaging<League>
            {
                currentPage = pageNumber,
                totalPages = totalPages,
                pageSize = pageSize,
                totalRecords = (int)totalRecords,
                content = Leagues
            };

            return respaging;
        }

        public async Task<League> GetLeagueById(string id)
        {
            // chuyển chuỗi string ID thành các ObjectId của mongoDB
            var objectId = ObjectId.Parse(id);
            var filter = Builders<League>.Filter.Eq("_id", objectId);
            return await _leagueCollection.Find(filter).FirstOrDefaultAsync();
        }

        //public Task<bool> UpdateLeague(string id, League league)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
