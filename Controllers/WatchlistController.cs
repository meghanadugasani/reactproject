using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Services;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Admin")] 
    public class WatchlistController : ControllerBase
    {
        private readonly IWatchlistService _watchlistService;

        public WatchlistController(IWatchlistService watchlistService)
        {
            _watchlistService = watchlistService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWatchlist(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { message = "Valid user ID is required" });
                    
                var watchlist = await _watchlistService.GetUserWatchlistAsync(userId);
                return Ok(watchlist);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToWatchlist([FromQuery] int userId, CreateWatchlistDto createDto)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { message = "Valid user ID is required" });
                    
                if (createDto == null)
                    return BadRequest(new { message = "Watchlist data is required" });
                    
                if (createDto.StockId <= 0)
                    return BadRequest(new { message = "Valid stock ID is required" });
                    
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new { message = "Validation failed", errors });
                }
                    
                var watchlist = await _watchlistService.AddToWatchlistAsync(userId, createDto);
                return CreatedAtAction(nameof(GetWatchlist), new { userId }, watchlist);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "";
                if (innerMessage.Contains("UNIQUE") || innerMessage.Contains("duplicate"))
                    return BadRequest(new { message = "Stock is already in your watchlist" });
                if (innerMessage.Contains("FOREIGN KEY") || innerMessage.Contains("constraint"))
                    return BadRequest(new { message = "Invalid stock ID or user ID" });
                return BadRequest(new { message = "Database error: Unable to add to watchlist", details = innerMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWatchlist(int id, UpdateWatchlistDto updateDto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Valid watchlist ID is required" });
                    
                var watchlist = await _watchlistService.UpdateWatchlistAsync(id, updateDto);
                return Ok(watchlist);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                return BadRequest(new { message = "Database error: Unable to update watchlist", details = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromWatchlist(int id, [FromQuery] int userId)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "Valid watchlist ID is required" });
                    
                if (userId <= 0)
                    return BadRequest(new { message = "Valid user ID is required" });
                    
                var success = await _watchlistService.RemoveFromWatchlistAsync(id, userId);
                if (!success)
                    return NotFound(new { message = "Watchlist entry not found" });

                return Ok(new { message = "Removed from watchlist successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{userId}/alerts")]
        public async Task<IActionResult> GetTriggeredAlerts(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { message = "Valid user ID is required" });
                    
                var alerts = await _watchlistService.GetTriggeredAlertsAsync(userId);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}