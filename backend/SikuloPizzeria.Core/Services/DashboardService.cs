using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.Core.Services;

public sealed class DashboardService : IDashboardService
{
private readonly IDashboardRepository _dashboardRepository;


public DashboardService(IDashboardRepository dashboardRepository)
{
    _dashboardRepository = dashboardRepository;
}

public Task<DashboardStatsDto> GetStatsAsync()
{
    return _dashboardRepository.GetStatsAsync();
}


}
