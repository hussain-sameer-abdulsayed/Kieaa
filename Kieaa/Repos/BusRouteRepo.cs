using Kieaa.Data;
using Kieaa.Dtos.BusRouteDto;
using Kieaa.IRepos;
using Kieaa.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Kieaa.Repos
{
    public class BusRouteRepo : IBusRouteRepo
    {
        private readonly Context _context;

        public BusRouteRepo(Context context)
        {
            _context = context;
        }

        public async Task<List<BusRoute>> GetAllBusRotuesAsync()
        {
            var busRoutes = await _context.BusRoutes.ToListAsync();
            if (busRoutes == null)
            {
                return new List<BusRoute>();
            }
            return busRoutes;
        }


        public async Task<BusRoute> GetBusRotueByIdAsync(int id)
        {
            var busRoute = await _context.BusRoutes
                            .Where(r=>r.Id == id)
                            .Include(r=>r.RouteCoordinates)
                            .FirstOrDefaultAsync();

            // Ensure the coordinates are sorted by their order
            busRoute.RouteCoordinates = busRoute.RouteCoordinates
                .OrderBy(rc => rc.Order)
                .ToList();


            return busRoute;
        }


        public async Task<CheckFunc> CreateBusRouteAsync(string userId, CreateBusRouteDto routeDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new CheckFunc { Message = $"BusRoute creation was unsuccessful: user not found" };
                }

                // Create the BusRoute entity
                var busRoute = new BusRoute
                {
                    UserId = userId,
                    CreatedBy = user,
                    Title = routeDto.Title,
                    StartName = routeDto.StartName,
                    EndName = routeDto.EndName,
                    CreatedAt = DateTime.UtcNow.ToShortTimeString(),
                    UpdatedAt = DateTime.UtcNow.ToShortTimeString(),
                    Cites = routeDto.Cites,
                    RouteCoordinates = new List<RouteCoordinate>()  // Initialize an empty list for now
                };

                // Add the BusRoute to the database
                await _context.BusRoutes.AddAsync(busRoute);
                await _context.SaveChangesAsync();  // Save to generate BusRouteId

                // Now, set BusRouteId for each RouteCoordinate and add them to the context
                busRoute.RouteCoordinates = routeDto.Coordinates
                    .Select((c, index) => new RouteCoordinate
                    {
                        Latitude = c.Lat,
                        Longitude = c.Lng,
                        Order = index + 1, // Set the order based on index
                        BusRouteId = busRoute.Id,  // Set the BusRouteId here
                        BusRoute = busRoute
                    }).ToList();

                // Add the route coordinates to the context and save again
                await _context.RouteCoordinates.AddRangeAsync(busRoute.RouteCoordinates);
                await _context.SaveChangesAsync();

                return new CheckFunc { IsSucceeded = true, Message = "BusRoute created successfully" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"BusRoute creation was unsuccessful: {ex.Message}" };
            }
        }



        public async Task<CheckFunc> UpdateBusRouteAsync(int id, UpdateBusRouteDto routeDto)
        {
            try
            {
                var busRoute = await _context.BusRoutes.FirstOrDefaultAsync(r => r.Id == id);
                if (busRoute == null)
                {
                    return new CheckFunc() { Message = "Bus route not found" };
                }

                // Attach the bus route if it's being tracked
                _context.BusRoutes.Attach(busRoute);


                // Update fields
                if (!string.IsNullOrEmpty(routeDto.Title))
                {
                    busRoute.Title = routeDto.Title;
                }
                if (!string.IsNullOrEmpty(routeDto.StartName))
                {
                    busRoute.StartName = routeDto.StartName;
                }
                if (!string.IsNullOrEmpty(routeDto.EndName))
                {
                    busRoute.EndName = routeDto.EndName;
                }
                if (routeDto.Cites.Any())
                {
                    busRoute.Cites = routeDto.Cites;
                }

                // Update route coordinates
                if (routeDto.RouteCoordinates.Any())
                {
                    // Get existing coordinates for the bus route
                    var oldCoordinates = await _context.RouteCoordinates.Where(c => c.BusRouteId == busRoute.Id).ToListAsync();

                    // Remove old coordinates
                    _context.RouteCoordinates.RemoveRange(oldCoordinates);

                    // Add new coordinates from DTO
                    busRoute.RouteCoordinates = routeDto.RouteCoordinates
                        .Select((c,index) => new RouteCoordinate
                        {
                            Latitude = c.Lat,
                            Longitude = c.Lng,
                            Order = index + 1, // Set the order based on index
                            BusRouteId = busRoute.Id
                        }).ToList();
                }

                await _context.SaveChangesAsync();
                return new CheckFunc { IsSucceeded = true, Message = "Bus route updated successfully" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"Bus route update failed: {ex.Message}" };
            }
        }


        public async Task<CheckFunc> DeleteBusRouteAsync(int id)
        {
            try
            {
                var busRoute = await _context.BusRoutes.FirstOrDefaultAsync(r => r.Id == id);
                if (busRoute == null)
                {
                    return new CheckFunc() { Message = "busRoute not found" };
                }
                _context.BusRoutes.Remove(busRoute);
                await _context.SaveChangesAsync();
                return new CheckFunc { IsSucceeded = true, Message = "busRoute Deleted was Successfully" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"BusRoute Deleted was Unseccessfully : {ex.Message}" };
            }
        }


        public async Task<List<BusRoute>> GetBusRotueByStartNameAsync(string name)
        {
            var busRoutes = await _context.BusRoutes.Where(r=>r.StartName == name)
                                                    .Include(r => r.RouteCoordinates).ToListAsync();
            
            
            if (!busRoutes.Any())
            {
                return new List<BusRoute>();
            }

            return busRoutes;
        }


        public async Task<List<BusRoute>> GetBusRotueByEndNameAsync(string name)
        {
            var busRoutes = await _context.BusRoutes.Where(r => r.EndName == name)
                                                    .Include(r => r.RouteCoordinates).ToListAsync();
            
            
            if (!busRoutes.Any())
            {
                return new List<BusRoute>();
            }

            return busRoutes;
        }


        public async Task<List<BusRoute>> GetBusRotueByNameAsync(string startname, string endname)
        {
            var busRoutes = await _context.BusRoutes.Where(r => r.StartName == startname && r.EndName == endname)
                                                    .Include(r => r.RouteCoordinates).ToListAsync();
            
            
            if (!busRoutes.Any())
            {
                return new List<BusRoute>();
            }
            
            
            return busRoutes;
        }
    }
}
