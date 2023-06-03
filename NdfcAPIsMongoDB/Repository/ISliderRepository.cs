using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository
{
    public interface ISliderRepository
    {
        Task<Respaging<Slider>> GetAllSliders(int pageNumber = 1, int pageSize = 10, string? searchTitle = null);
    }
}
