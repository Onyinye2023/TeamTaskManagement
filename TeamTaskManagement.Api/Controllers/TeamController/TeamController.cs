using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagement.Application.DTOs;
using TeamTaskManagement.Application.IServices.ITeamService;

namespace TeamTaskManagement.Api.Controllers
{
	/// <summary>
	/// Manages team-related operations such as creating teams and adding users.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class TeamController : ControllerBase
	{
		private readonly ITeamService _teamService;
		private readonly ILogger<TeamController> _logger;

		public TeamController(ITeamService teamService, ILogger<TeamController> logger)
		{
			_teamService = teamService;
			_logger = logger;
		}

		/// <summary>
		/// Creates a new team for the logged-in user.
		/// </summary>
		/// <param name="createTeamDTO">The details of the team to create.</param>
		/// <returns>A confirmation message and the created team object.</returns>
		/// <response code="200">Team created successfully.</response>
		/// <response code="400">Invalid request data.</response>
		/// <response code="401">User is not authorized.</response>
		[HttpPost("team")]
		[Authorize]
		public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDTO createTeamDTO)
		{
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid model state for CreateTeamDTO.");
				return BadRequest(ModelState);
			}

			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))
			{
				_logger.LogWarning("No UserId found in JWT token.");
				return Unauthorized("User ID not found in token.");
			}

			try
			{
				var team = await _teamService.CreateTeamAsync(createTeamDTO, userId);
				return Ok(new { Message = "You have successfully created a team:", team });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating team for user ID: {UserId}", userId);
				return StatusCode(500, "An error occurred while creating the team.");
			}
		}


		/// <summary>
		/// Adds a user to an existing team.
		/// </summary>
		/// <param name="teamId">The ID of the team.</param>
		/// <param name="inviteDTO">The details of the user being invited.</param>
		/// <returns>A success or failure message.</returns>
		/// <response code="200">User added successfully.</response>
		/// <response code="400">Invalid request data or user already in team.</response>
		/// <response code="401">User is not authorized.</response>
		[HttpPost("{teamId}/users")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> AddUserToTeam(Guid teamId, [FromBody] InviteUserToTeamDTO inviteDTO)
		{
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid model state for InviteUserToTeamDTO.");
				return BadRequest(ModelState);
			}

			var inviterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(inviterId))
			{
				_logger.LogWarning("No UserId found in JWT token.");
				return Unauthorized("User ID not found in token.");
			}

			try
			{
				var result = await _teamService.AddUserToTeamAsync(teamId, inviteDTO, inviterId);
				if (!result)
				{
					return BadRequest("Failed to add user to team. User or team not found, or user is already a member.");
				}

				return Ok("User added to team successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error adding user to team {TeamId} by inviter {InviterId}", teamId, inviterId);
				return StatusCode(500, "An error occurred while adding the user to the team.");
			}
		}

	}
}