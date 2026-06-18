using Microsoft.AspNetCore.Mvc;
using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Interfaces;

namespace SikuloPizzeria.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
private readonly IDashboardService _dashboardService;


public DashboardController(IDashboardService dashboardService)
{
    _dashboardService = dashboardService;
}

[HttpGet("stats")]
public async Task<ActionResult<DashboardStatsDto>> GetStats()
{
    DashboardStatsDto stats =
        await _dashboardService.GetStatsAsync();

    return Ok(stats);
}


}
