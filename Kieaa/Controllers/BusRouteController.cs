using Kieaa.Dtos.BusRouteDto;
using Kieaa.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kieaa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusRouteController : ControllerBase
    {
        private readonly IBusRouteRepo _busRouteRepo;

        public BusRouteController(IBusRouteRepo busRouteRepo)
        {
            _busRouteRepo = busRouteRepo;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllBusRoutesAsync()
        {
            try
            {
                var busRoutes = await _busRouteRepo.GetAllBusRotuesAsync();
                if(busRoutes.Count() == 0)
                {
                    return NotFound("There is no Bus Routes!");
                }
                return Ok(busRoutes);
            }
            catch (Exception ex)
            {
                return BadRequest($"There is an error while trying to get all Bus Routes!: {ex.Message}");
            }
        }


        [HttpGet("{busId}")]
        public async Task<IActionResult> GetBusRouteByIdAsync(int busId)
        {
            try
            {
                var busRoute = await _busRouteRepo.GetBusRotueByIdAsync(busId);
                if(busRoute == null)
                {
                    return NotFound("There is no Bus Route with this id!");
                }
                return Ok(busRoute);
            }
            catch (Exception ex)
            {
                return BadRequest($"There is an error while trying to get Bus Route!: {ex.Message}");
            }
        }


        [HttpGet("search/startname/{startname}")]
        public async Task<IActionResult> GetBusRoutesByStartName(string startname)
        {
            try
            {
                var busRoutes = await _busRouteRepo.GetBusRotueByStartNameAsync(startname);
                if(busRoutes.Count()==0)
                {
                    return NotFound("There is no bus routes with this name");
                }
                return Ok(busRoutes);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurd while trying to get some bus routes by start name : {ex.Message}");
            }
        }


        [HttpGet("search/endname/{endname}")]
        public async Task<IActionResult> GetBusRoutesByEndName(string endname)
        {
            try
            {
                var busRoutes = await _busRouteRepo.GetBusRotueByEndNameAsync(endname);
                if (busRoutes.Count() == 0)
                {
                    return NotFound("There is no bus routes with this name");
                }
                return Ok(busRoutes);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurd while trying to get some bus routes by end name : {ex.Message}");
            }
        }


        [HttpGet("search/startname-endname/{startname}/{endname}")]
        public async Task<IActionResult> GetBusRoutesByName(string startname, string endname)
        {
            try
            {
                var busRoutes = await _busRouteRepo.GetBusRotueByNameAsync(startname, endname);
                if (busRoutes.Count() == 0)
                {
                    return NotFound("There is no bus routes with this name");
                }
                return Ok(busRoutes);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurd while trying to get some bus routes by name : {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusRouteAsync([FromBody] CreateBusRouteDto busRouteDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                /*
                string accessToken = Request.Headers[HeaderNames.Authorization];
                var token = accessToken.Substring(7);
                var userId = _jWTMangerRepo.GetUserId(token);
                */
                var userId = "0842a1a0-44d2-4882-8266-12e5a939d452";
                var process = await _busRouteRepo.CreateBusRouteAsync(userId,busRouteDto);
                if (!process.IsSucceeded)
                {
                    return BadRequest(process.Message);
                }
                return Ok(process.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"There is an error while trying to create a busRoute : {ex.Message}");
            }
        }


        [HttpPut("{busId}")]
        public async Task<IActionResult> UpdateBusRouteAsync([FromBody] UpdateBusRouteDto busRouteDto,int busId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var process = await _busRouteRepo.UpdateBusRouteAsync(busId, busRouteDto);
                if (!process.IsSucceeded)
                {
                    return BadRequest(process.Message);
                }
                return Ok(process.Message);
            }
            catch(Exception ex)
            {
                return BadRequest($"There is an error while trying to update a busRoute : {ex.Message}");
            }
        }


        [HttpDelete("{busId}")]
        public async Task<IActionResult> DeleteBusRouteAsync(int busId)
        {
            try
            {
                var process = await _busRouteRepo.DeleteBusRouteAsync(busId);
                if (!process.IsSucceeded)
                {
                    return BadRequest(process.Message);
                }
                return Ok(process.Message);
            }
            catch(Exception ex)
            {
                return BadRequest($"There is an error while trying to delete a busRoute : {ex.Message}");
            }
        }
        

    }
}
