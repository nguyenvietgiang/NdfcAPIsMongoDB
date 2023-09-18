using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.SubscribService
{
    public interface ISubscriberRepository
    {
        Task<Respaging<Subscriber>> GetAllSubcriber(int pageNumber = 1, int pageSize = 10, string? searchName = null);
        Task<Subscriber> Subcribe(Subscriber subscriber);  
    }
}
