using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GymAngel.Business.DTOs.MembershipDTOs;
using GymAngel.Business.Services.Membership;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymAngel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipsController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipsController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        /// <summary>
        /// Get all active membership plans
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetActivePlans()
        {
            try
            {
                var plans = await _membershipService.GetActivePlansAsync();
                return Ok(plans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve plans", error = ex.Message });
            }
        }

        /// <summary>
        /// Get membership plan by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById(int id)
        {
            try
            {
                var plan = await _membershipService.GetPlanByIdAsync(id);
                if (plan == null)
                    return NotFound(new { message = "Plan not found" });

                return Ok(plan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve plan", error = ex.Message });
            }
        }

        /// <summary>
        /// Subscribe to a membership plan (requires authentication)
        /// </summary>
        [HttpPost("subscribe")]
        [Authorize]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeMembershipDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var subscription = await _membershipService.SubscribeToPlanAsync(dto, userId);
                return Ok(subscription);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to subscribe", error = ex.Message });
            }
        }

        /// <summary>
        /// Get current user's membership status
        /// </summary>
        [HttpGet("my-status")]
        [Authorize]
        public async Task<IActionResult> GetMyStatus()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var status = await _membershipService.GetUserMembershipStatusAsync(userId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve status", error = ex.Message });
            }
        }

        /// <summary>
        /// Renew membership with a new plan
        /// </summary>
        [HttpPost("renew")]
        [Authorize]
        public async Task<IActionResult> Renew([FromBody] SubscribeMembershipDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var subscription = await _membershipService.RenewMembershipAsync(userId, dto.PlanId);
                return Ok(subscription);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to renew membership", error = ex.Message });
            }
        }

        /// <summary>
        /// Check and update expired memberships (admin/background job endpoint)
        /// </summary>
        [HttpPost("check-expiry")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckExpiry()
        {
            try
            {
                await _membershipService.CheckAndUpdateExpiredMembershipsAsync();
                return Ok(new { message = "Expired memberships checked and updated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to check expiry", error = ex.Message });
            }
        }

        /// <summary>
        /// Enable auto-renewal for current user's membership
        /// </summary>
        [HttpPost("auto-renewal/enable")]
        [Authorize]
        public async Task<IActionResult> EnableAutoRenewal()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var result = await _membershipService.EnableAutoRenewalAsync(userId);
                if (!result)
                    return NotFound(new { message = "No active membership found" });

                return Ok(new { message = "Auto-renewal enabled", autoRenewal = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to enable auto-renewal", error = ex.Message });
            }
        }

        /// <summary>
        /// Disable auto-renewal for current user's membership
        /// </summary>
        [HttpPost("auto-renewal/disable")]
        [Authorize]
        public async Task<IActionResult> DisableAutoRenewal()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var result = await _membershipService.DisableAutoRenewalAsync(userId);
                if (!result)
                    return NotFound(new { message = "No active membership found" });

                return Ok(new { message = "Auto-renewal disabled", autoRenewal = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to disable auto-renewal", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancel membership (remains active until expiry)
        /// </summary>
        [HttpPost("cancel")]
        [Authorize]
        public async Task<IActionResult> CancelMembership([FromBody] CancelMembershipRequest? request)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var reason = request?.Reason;
                var result = await _membershipService.CancelMembershipAsync(userId, reason);
                
                if (!result)
                    return NotFound(new { message = "No active membership found" });

                return Ok(new { message = "Membership cancelled. Access remains until expiry date.", cancelled = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to cancel membership", error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CancelMembershipRequest
    {
        public string? Reason { get; set; }
    }
}
