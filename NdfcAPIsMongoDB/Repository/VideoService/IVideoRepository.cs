using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.VideoService
{
    public interface IVideoRepository
    {
        Task<Respaging<Video>> GetAllVideos(int pageNumber = 1, int pageSize = 10, string? searchTitle = null);

        Task<Video> GetVideoById(string id);

        Task<bool> DeleteVideo(string id);
    }
}
