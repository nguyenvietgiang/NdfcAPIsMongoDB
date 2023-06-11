using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.NewsService
{
    public interface INewsRepository
    {
        Task<Respaging<News>> GetAllNews(int pageNumber = 1, int pageSize = 10, string? searchTitle = null);

        Task<News> CreateNew(News news, IFormFile image, string host);
        string SaveImage(IFormFile image, string host);

        Task<News> GetNewById(string id);
        Task<bool> DeleteNew(string id);

        Task<bool> DeleteNews(List<string> ids);
    }
}
