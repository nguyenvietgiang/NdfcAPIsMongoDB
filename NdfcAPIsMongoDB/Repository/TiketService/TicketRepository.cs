using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.TiketService
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Tickets> _ticketsCollection;
        private readonly IMongoCollection<Seat> _seatCollection;

        public TicketRepository(IMongoDatabase database)
        {
            _ticketsCollection = database.GetCollection<Tickets>("Tickets");
            _seatCollection = database.GetCollection<Seat>("Seat");
        }

        public async Task<IEnumerable<Tickets>> GetTickets()
        {
            return await _ticketsCollection.Find(ticket => true).ToListAsync();
        }

        public async Task<Tickets> GetTicketById(string id)
        {
            return await _ticketsCollection.Find(ticket => ticket.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Tickets> AddTicket(Tickets ticket)
        {
            // Mặc định giá vé là 50000 VND
            if (ticket.price == 0)
            {
                ticket.price = 50000;
            }

            // Lấy ra ID ghế của vé vừa thêm vào
            var seatObjectId = ObjectId.Parse(ticket.SeatId);

            // Cập nhật trạng thái của ghế từ false sang true
            var filter = Builders<Seat>.Filter.Eq(s => s.Id, seatObjectId.ToString());
            var update = Builders<Seat>.Update.Set(s => s.Status, true);

            await _seatCollection.UpdateOneAsync(filter, update);

            // Thêm vé vào collection
            await _ticketsCollection.InsertOneAsync(ticket);

            return ticket;
        }

        public async Task<bool> DeleteTicket(string id)
        {
            var result = await _ticketsCollection.DeleteOneAsync(ticket => ticket.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
