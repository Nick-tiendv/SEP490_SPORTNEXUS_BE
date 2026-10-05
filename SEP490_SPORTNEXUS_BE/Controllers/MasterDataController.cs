using Microsoft.AspNetCore.Mvc;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/v1/master-data")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly IMasterDataService _service;
        public MasterDataController(IMasterDataService service)
        {
            _service = service;
        }

        [HttpGet("sports")]
        public async Task<IActionResult> GetSports()
        {
            var res = await _service.GetAllSportCategoriesAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("amenities")]
        public async Task<IActionResult> GetAmenities()
        {
            var res = await _service.GetAllAmenitiesAsync();
            return StatusCode(res.StatusCode, res);
        }
    }
}
