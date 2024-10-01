using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.ContactService
{
    public class ContactRepository : IContact
    {
        private readonly IMongoCollection<Contact> _contactCollection;

        public ContactRepository(IMongoDatabase database)
        {
            _contactCollection = database.GetCollection<Contact>("Contact");
        }
        public async Task<Contact> CreateContact(Contact contact)
        {
            await _contactCollection.InsertOneAsync(contact);
            return contact;
        }

        public async Task<bool> DeleteContact(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Contact>.Filter.Eq("_id", objectId);
            var result = await _contactCollection.DeleteOneAsync(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<bool> DeleteContacts(List<string> ids)
        {
            foreach (var id in ids)
            {
                var objectId = ObjectId.Parse(id);
                var filter = Builders<Contact>.Filter.Eq("_id", objectId);
                var result = await _contactCollection.DeleteOneAsync(filter);
            }
            return true;
        }

        public async Task<Respaging<Contact>> GetAllContact(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var filter = Builders<Contact>.Filter.Empty;

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            // Tìm kiếm theo tên nếu có giá trị searchName được cung cấp
            if (!string.IsNullOrEmpty(searchName))
            {
                filter = Builders<Contact>.Filter.Regex(x => x.Name, new BsonRegularExpression(searchName, "i"));
            }

            // Thêm đoạn mã sau vào để bỏ qua điều kiện tìm kiếm khi searchName không được cung cấp
            if (string.IsNullOrEmpty(searchName))
            {
                filter = Builders<Contact>.Filter.Empty;
            }

            // Đếm tổng số bản ghi
            var totalRecords = await _contactCollection.CountDocumentsAsync(filter);

            // Phân trang và lấy dữ liệu
            var Contacts = await _contactCollection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Tính toán số trang
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Tạo đối tượng Respaging để trả về
            var respaging = new Respaging<Contact>
            {
                currentPage = pageNumber,
                totalPages = totalPages,
                pageSize = pageSize,
                totalRecords = (int)totalRecords,
                content = Contacts
            };

            return respaging;
        }

        public async Task<Contact> GetContactById(string id)
        {
            // chuyển chuỗi string ID thành các ObjectId của mongoDB
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Contact>.Filter.Eq("_id", objectId);
            return await _contactCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
