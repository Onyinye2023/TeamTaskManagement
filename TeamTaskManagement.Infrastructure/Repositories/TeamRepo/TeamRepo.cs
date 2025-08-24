namespace TeamTaskManagement.Infrastructure.Repositories.TeamRepo
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;
	using TeamTaskManagement.Infrastructure.Data;
	using TeamTaskManagement.Application.DTOs;
	using TeamTaskManagement.Application.IRepositories.ITeamRepo;
	using TeamTaskManagement.Domain.Entities;
	using TeamTaskManagement.Domain.Enums;

	public class TeamRepo : ITeamRepo
	{
		private readonly AppDbContext _context;
		private readonly ILogger<TeamRepo> _logger;

		public TeamRepo(AppDbContext context, ILogger<TeamRepo> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<TeamResponseDTO> CreateTeamAsync(CreateTeamDTO createTeamDTO, Guid creatorId)
		{
			try
			{
				var creator = await _context.Users.FindAsync(creatorId);
				if (creator == null)
				{
					_logger.LogWarning("Creator not found for ID: {CreatorId}", creatorId);
					throw new ArgumentException("Creator user not found.", nameof(creatorId));
				}

				var team = new Team
				{
					Id = Guid.NewGuid(),
					Name = createTeamDTO.Name,
					Description = createTeamDTO.Description,
					CreatedAt = DateTime.UtcNow,
					CreatedByUserId = creatorId,
					CreatedBy = creator
				};

				_context.Teams.Add(team);

				var teamUser = new TeamUser
				{
					UserId = creatorId,
					TeamId = team.Id,
					Role = TeamRole.TeamAdmin
				};

				_context.TeamUsers.Add(teamUser);

				await _context.SaveChangesAsync();
				_logger.LogInformation("Team created: {TeamId} by user: {CreatorId}", team.Id, creatorId);
				
				return new TeamResponseDTO
				{
					Name = createTeamDTO.Name,
					Description = createTeamDTO.Description
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating team for creator ID: {CreatorId}", creatorId);
				throw new Exception("An error occurred while creating the team.", ex);
			}
		}

		public async Task<bool> AddUserToTeamAsync(Guid teamId, Guid userId, Guid inviterId)
		{
			try
			{
				var team = await _context.Teams.FindAsync(teamId);
				if (team == null)
				{
					_logger.LogWarning("Team not found for ID: {TeamId}", teamId);
					return false;
				}

				var user = await _context.Users.FindAsync(userId);
				if (user == null)
				{
					_logger.LogWarning("User not found for ID: {UserId}", userId);
					return false;
				}

				var inviter = await _context.TeamUsers
					.AnyAsync(ut => ut.UserId == inviterId && ut.TeamId == teamId && ut.Role == TeamRole.TeamAdmin);

				if (!inviter)
				{
					_logger.LogWarning("Inviter {InviterId} is not authorized to add users to team {TeamId}", inviterId, teamId);
					return false;
				}

				var existingUserTeam = await _context.TeamUsers
					.AnyAsync(ut => ut.UserId == userId && ut.TeamId == teamId);
				if (existingUserTeam)
				{
					_logger.LogWarning("User {UserId} is already a member of team {TeamId}", userId, teamId);
					return false;
				}

				var userTeam = new TeamUser
				{
					UserId = userId,
					TeamId = teamId,
					Role = TeamRole.Member 
				};

				_context.TeamUsers.Add(userTeam);
				await _context.SaveChangesAsync();
				_logger.LogInformation("User {UserId} added to team {TeamId} by inviter {InviterId}", userId, teamId, inviterId);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error adding user {UserId} to team {TeamId} by inviter {InviterId}", userId, teamId, inviterId);
				throw new Exception("An error occurred while adding the user to the team.", ex);
			}
		}
	}
}