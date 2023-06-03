using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Repository;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SliderController : ControllerBase
    {
        private readonly ISliderRepository _sliderRepository; 

        public SliderController(ISliderRepository sliderRepository)
        {
            _sliderRepository = sliderRepository; 
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSlider(int pageNumber = 1, int pageSize = 10, string? searchTitle = null)
        {
            var slider = await _sliderRepository.GetAllSliders(pageNumber, pageSize, searchTitle);
            return Ok(slider);
        }
    }
}
