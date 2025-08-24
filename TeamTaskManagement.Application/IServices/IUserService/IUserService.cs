namespace TeamTaskManagement.Application.IServices.IUserService
{
	using TeamTaskManagement.Application.DTOs;

	public interface IUserService
	{
		Task<ResponseDTO> RegisterAdmin(RegisterDTO adminDTO, int roleId);
		Task<ResponseDTO> RegisterUser(RegisterDTO userRegisterDTO, int roleId);
		Task<ResponseDTO> UserLogin(LoginDTO loginDTO);
		Task<UserDTO?> GetCurrentUserAsync(string userId);

	}
}
