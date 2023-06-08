using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository
{
    public interface IContact
    {
        Task<Respaging<Contact>> GetAllContact(int pageNumber = 1, int pageSize = 10, string? searchName = null);
        Task<Contact> CreateContact(Contact contact);

        Task<Contact> GetContactById(string id);

        Task<bool> DeleteContact(string id);
    }
}
