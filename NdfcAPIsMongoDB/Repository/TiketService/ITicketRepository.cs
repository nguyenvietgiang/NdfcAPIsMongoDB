using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.TiketService
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Tickets>> GetTickets();
        Task<Tickets> GetTicketById(string id);
        Task<Tickets> AddTicket(Tickets ticket);
        Task<bool> DeleteTicket(string id);
    }
}
