namespace TeamTaskManagement.Infrastructure.Services.TeamService
{
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using TeamTaskManagement.Application.DTOs;
    using TeamTaskManagement.Application.IRepositories.ITeamRepo;
    using TeamTaskManagement.Application.IServices.ITeamService;
	using TeamTaskManagement.Application.IRepositories.IUserRepo;

	public class TeamService : ITeamService
	{
		private readonly ITeamRepo _teamRepo;
		private readonly IUserRepo _userRepo;
		private readonly IMapper _mapper;
		private readonly ILogger<TeamService> _logger;

		public TeamService(ITeamRepo teamRepo, IUserRepo userRepo, IMapper mapper, ILogger<TeamService> logger)
		{
			_teamRepo = teamRepo;
			_userRepo = userRepo;
			_mapper = mapper;
			_logger = logger;
		}
		public async Task<TeamResponseDTO> CreateTeamAsync(CreateTeamDTO createTeamDTO, string userId)
		{
			if (!Guid.TryParse(userId, out var parsedUserId))
			{
				_logger.LogWarning("Invalid UserId format: {UserId}", userId);
				throw new ArgumentException("Invalid UserId format.", nameof(userId));
			}

			try
			{
				var team = await _teamRepo.CreateTeamAsync(createTeamDTO, parsedUserId);

				return _mapper.Map<TeamResponseDTO>(team);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating team for user ID: {UserId}", userId);
				throw;
			}
		}

		public async Task<bool> AddUserToTeamAsync(Guid teamId, InviteUserToTeamDTO inviteDTO, string inviterId)
		{
			if (!Guid.TryParse(inviterId, out var parsedInviterId))
			{
				_logger.LogWarning("Invalid InviterId format: {InviterId}", inviterId);
				throw new ArgumentException("Invalid InviterId format.", nameof(inviterId));
			}

			try
			{
				var user = await _userRepo.GetUserByEmailAsync(inviteDTO.Email);
				if (user == null)
				{
					_logger.LogWarning("User not found for email: {Email}", inviteDTO.Email);
					return false;
				}

				return await _teamRepo.AddUserToTeamAsync(teamId, user.UserId, parsedInviterId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error adding user with email {Email} to team {TeamId} by inviter {InviterId}",
					inviteDTO.Email, teamId, inviterId);
				throw;
			}
		}
	}
}