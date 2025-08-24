namespace TeamTaskManagement.Application.IServices.ITeamService
{
    using TeamTaskManagement.Application.DTOs;

    public interface ITeamService
    {
        Task<TeamResponseDTO> CreateTeamAsync(CreateTeamDTO createTeamDTO, string userId);
		Task<bool> AddUserToTeamAsync(Guid teamId, InviteUserToTeamDTO inviteDTO, string inviterId);
	}
}
