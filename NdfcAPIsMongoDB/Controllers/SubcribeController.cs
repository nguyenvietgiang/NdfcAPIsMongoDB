using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Models.DTO;
using NdfcAPIsMongoDB.Repository.SubscribService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class SubcribeController : ControllerBase
    {
        private readonly ISubscriberRepository _SubscriberRepository;

        public SubcribeController(ISubscriberRepository SubscriberRepository)
        { 
            _SubscriberRepository = SubscriberRepository;
        }

        /// <summary>
        /// get all Subscriber 
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllSubscriber(int pageNumber = 1, int pageSize = 10, string? searchName = null)
        {
            var respaging = await _SubscriberRepository.GetAllSubcriber(pageNumber, pageSize, searchName);
            return Ok(respaging);
        }

        /// <summary>
        /// create new Subscriber - no auth
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateSubscriber([FromBody] SubcriberDTO SubscriberDto)
        {
            if (SubscriberDto == null)
            {
                return BadRequest();
            }
            // tạo một classDTO không bao gồm ID để mongoDB tự tạo
            var Subscriber = new Subscriber
            {
                Name = SubscriberDto.Name,
                Email = SubscriberDto.Email,
            };

            await _SubscriberRepository.Subcribe(Subscriber);

            return Ok(Subscriber);
        }
    }
}
