using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository
{
    public interface INewsRepository
    {
        Task<Respaging<News>> GetAllNews(int pageNumber = 1, int pageSize = 10, string? searchTitle = null); 
    }
}
