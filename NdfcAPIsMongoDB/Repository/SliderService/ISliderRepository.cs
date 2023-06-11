using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.SliderService
{
    public interface ISliderRepository
    {
        Task<Respaging<Slider>> GetAllSliders(int pageNumber = 1, int pageSize = 10, string? searchTitle = null);

        Task<Slider> GetSliderById(string id);

        Task<bool> DeleteSlider(string id);
    }
}
