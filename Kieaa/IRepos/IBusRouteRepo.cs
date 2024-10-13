using Kieaa.Dtos.BusRouteDto;
using Kieaa.Models;

namespace Kieaa.IRepos
{
    public interface IBusRouteRepo
    {
        Task <List<BusRoute>> GetAllBusRotuesAsync();
        Task <BusRoute> GetBusRotueByIdAsync(int id);
        Task <List<BusRoute>> GetBusRotueByStartNameAsync(string name);
        Task <List<BusRoute>> GetBusRotueByEndNameAsync(string name);
        Task <List<BusRoute>> GetBusRotueByNameAsync(string name);//search in both start and end
        Task <CheckFunc> CreateBusRouteAsync(string userId,CreateBusRouteDto routeDto);
        Task <CheckFunc> UpdateBusRouteAsync(int id,UpdateBusRouteDto routeDto);
        Task <CheckFunc> DeleteBusRouteAsync(int id);
        
    }
}
