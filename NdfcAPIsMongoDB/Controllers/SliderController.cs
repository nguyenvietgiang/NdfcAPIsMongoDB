﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NdfcAPIsMongoDB.Repository;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSliderById(string id)
        {
            var slider = await _sliderRepository.GetSliderById(id);

            if (slider == null)
            {
                return NotFound();
            }

            // Trả về kết quả thành công
            return Ok(slider);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePlayer(string id)
        {
            var existingSlider = await _sliderRepository.GetSliderById(id);
            if (existingSlider == null)
            {
                return NotFound();
            }

            var deletedSlider = await _sliderRepository.DeleteSlider(id);
            if (!deletedSlider)
            {
                return StatusCode(500, "An error occurred while deleting the Slider.");
            }

            return NoContent();
        }
    }
}
