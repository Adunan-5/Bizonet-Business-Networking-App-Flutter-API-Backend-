using Bizonet.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bizonet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LookupsController : ControllerBase
    {
        private readonly ILookupsService _lookupsService;

        public LookupsController(ILookupsService lookupsService)
        {
            _lookupsService = lookupsService;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> Countries()
        {
            var data = await _lookupsService.GetCountriesAsync();
            return Ok(new { success = true, data });
        }

        [HttpGet("states")]
        public async Task<IActionResult> States([FromQuery] int countryId)
        {
            if (countryId <= 0)
                return BadRequest(new { error = "countryId is required" });

            var data = await _lookupsService.GetStatesAsync(countryId);
            return Ok(new { success = true, data });
        }

        [HttpGet("cities")]
        public async Task<IActionResult> Cities([FromQuery] int stateId)
        {
            if (stateId <= 0)
                return BadRequest(new { error = "stateId is required" });

            var data = await _lookupsService.GetCitiesAsync(stateId);
            return Ok(new { success = true, data });
        }

        [HttpGet("business-categories")]
        public async Task<IActionResult> BusinessCategories()
        {
            var data = await _lookupsService.GetBusinessCategoriesAsync();
            return Ok(new { success = true, data });
        }
    }
}
