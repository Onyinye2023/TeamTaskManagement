namespace TeamTaskManagement.Application.IRepositories.IUserRepo
{
	using TeamTaskManagement.Application.DTOs;
	using TeamTaskManagement.Domain.Entities;

	public interface IUserRepo
	{
		Task<User> RegisterAdminAsync(User adminDTO, int roleId);
		Task<ResponseDTO> RegisterUserAsync(User userDTO, int roleId);
		Task<User?> LoginAsync(LoginDTO loginDTO);
		Task<User?> GetUserByIdAsync(Guid userId);
		Task<User?> GetUserByEmailAsync(string email);
	}
}
