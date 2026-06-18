using SikuloPizzeria.Core.DTOs;

namespace SikuloPizzeria.Core.Interfaces;

public interface IDashboardService
{
Task<DashboardStatsDto> GetStatsAsync();
}
