using SikuloPizzeria.Core.DTOs;

namespace SikuloPizzeria.Core.Interfaces;

public interface IDashboardRepository
{
Task<DashboardStatsDto> GetStatsAsync();
}
