using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository
{
    public interface IVideoRepository
    {
        Task<Respaging<Video>> GetAllVideos(int pageNumber = 1, int pageSize = 10, string? searchTitle = null);
    }
}
