namespace TeamTaskManagement.Application.IRepositories.ITeamRepo
{
	using TeamTaskManagement.Application.DTOs;

	public interface ITeamRepo
	{
		Task<TeamResponseDTO> CreateTeamAsync(CreateTeamDTO createTeamDTO, Guid creatorId);
		Task<bool> AddUserToTeamAsync(Guid teamId, Guid userId, Guid inviterId);
	}
}
