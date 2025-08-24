namespace TeamTaskManagement.Infrastructure.Services.UserService
{
	using System.ComponentModel.DataAnnotations;
	using System.IdentityModel.Tokens.Jwt;
	using System.Security.Claims;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using AutoMapper;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;
	using Microsoft.IdentityModel.Tokens;
	using TeamTaskManagement.Application.DTOs;
	using TeamTaskManagement.Application.IRepositories.IUserRepo;
	using TeamTaskManagement.Application.IServices.IUserService;
	using TeamTaskManagement.Domain.Entities;

	public class UserService : IUserService
	{
		private readonly IUserRepo _userRepo;
		private readonly ILogger<UserService> _logger;
		private readonly IConfiguration _configuration;
		private readonly IMapper _mapper;

		public UserService(IUserRepo userRepo, ILogger<UserService> logger,
			IConfiguration configuration, IMapper mapper)
		{
			_userRepo = userRepo;
			_logger = logger;
			_mapper = mapper;
			_configuration = configuration;

		}

		public async Task<ResponseDTO> RegisterAdmin(RegisterDTO adminDTO, int roleId)
		{
			if (adminDTO == null)
			{
				_logger.LogError("AdminDTO is null.");
				return new ResponseDTO { Success = false, Message = "Invalid Admin details." };
			}

			try
			{
				var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com|system\.com|hotmail\.com)$");
				if (!emailRegex.IsMatch(adminDTO.Email))
				{
					_logger.LogWarning("Invalid email format for registration: {Email}", adminDTO.Email);
					return new ResponseDTO { Success = false, Message = "Email must be from gmail.com, yahoo.com, outlook.com, system.com or hotmail.com." };
				}

				bool passwordCheck = Validator.TryValidateProperty(adminDTO.Password,
					new ValidationContext(adminDTO) { MemberName = nameof(adminDTO.Password) },
					null);

				if (!passwordCheck)
				{
					return new ResponseDTO { Success = false, Message = "Invalid Password details." };
				}

				var adminUser = _mapper.Map<User>(adminDTO);

				if (adminUser == null)
				{
					_logger.LogError("Mapping of RegisterDTO to User failed.");
					return new ResponseDTO { Success = false, Message = "Mapping error." };
				}

				var createdAdmin = await _userRepo.RegisterAdminAsync(adminUser, roleId);
				if (createdAdmin == null)
				{
					return new ResponseDTO { Success = false, Message = "Failed to create an Admin." };
				}
				return new ResponseDTO
				{
					Success = true,
					Email = createdAdmin.Email,
					Message = "Admin registered successfully."
				};
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Validation error during Admin registration.");
				return new ResponseDTO { Success = false, Message = ex.Message };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while registering an Admin.");
				return new ResponseDTO { Success = false, Message = "An error occurred while registering an Admin." };
			}
		}

		public async Task<ResponseDTO> RegisterUser(RegisterDTO userRegisterDTO, int roleId)
		{
			if (userRegisterDTO == null)
			{
				_logger.LogError("userRegisterDTO is null.");
				return new ResponseDTO { Success = false, Message = "Invalid User details." };
			}

			try
			{
				var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com|system\.com|hotmail\.com)$");
				if (!emailRegex.IsMatch(userRegisterDTO.Email))
				{
					_logger.LogWarning("Invalid email format for registration: {Email}", userRegisterDTO.Email);
					return new ResponseDTO { Success = false, Message = "Email must be from gmail.com, yahoo.com, outlook.com, or hotmail.com." };
				}

				bool passwordCheck = Validator.TryValidateProperty(userRegisterDTO.Password,
					new ValidationContext(userRegisterDTO)
					{
						MemberName = nameof(userRegisterDTO.Password)
					}, null);

				if (!passwordCheck)
				{
					return new ResponseDTO { Success = false, Message = "Invalid Password details." };
				}

				var user = _mapper.Map<User>(userRegisterDTO);
				if (user == null)
				{
					_logger.LogError("Mapping of UserDTO to User failed.");
					return new ResponseDTO { Success = false, Message = "Mapping error." };
				}

				user.RoleId = roleId;

				var result = await _userRepo.RegisterUserAsync(user,roleId);

				if (!result.Success)
				{
					return new ResponseDTO { Success = false, Message = result.Message };
				}

				return new ResponseDTO
				{
					Success = true,
					Email = result.Email,
					Password = result.Password,
					Message = "User registered successfully."
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while registering a User.");
				return new ResponseDTO { Success = false, Message = "An error occurred while registering a User." };
			}
		}

		public async Task<ResponseDTO> UserLogin(LoginDTO loginDTO)
		{
			if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
			{
				_logger.LogWarning("Invalid login attempt: Email or password is missing.");
				return new ResponseDTO { Success = false, Message = "Email and password are required." };
			}

			try
			{
				var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com|system\.com|hotmail\.com)$");
				if (!emailRegex.IsMatch(loginDTO.Email))
				{
					_logger.LogWarning("Invalid email format for login: {Email}", loginDTO.Email);
					return new ResponseDTO { Success = false, Message = "Email must be from gmail.com, yahoo.com, outlook.com, or hotmail.com." };
				}

				var user = await _userRepo.LoginAsync(loginDTO);
				if (user == null)
				{
					_logger.LogWarning("Login failed for email: {Email}. Invalid credentials.", loginDTO.Email);
					return new ResponseDTO { Success = false, Message = "Invalid email or password." };
				}

				var token = await GenerateJwtTokenAsync(loginDTO.Email);
				return new ResponseDTO
				{
					Success = true,
					Message = "Login successful.",
					Token = token
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login for email: {Email}", loginDTO.Email);
				return new ResponseDTO { Success = false, Message = "An error occurred during login. Please try again later." };
			}
		}

		public async Task<UserDTO?> GetCurrentUserAsync(string userId)
		{
			if (!Guid.TryParse(userId, out var parsedUserId))
			{
				_logger.LogWarning("Invalid UserId format: {UserId}", userId);
				return null;
			}

			try
			{
				var user = await _userRepo.GetUserByIdAsync(parsedUserId);
				if (user == null)
				{
					_logger.LogWarning("User not found for ID: {UserId}", userId);
					return null;
				}

				return _mapper.Map<UserDTO>(user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving user with ID: {UserId}", userId);
				return null;
			}
		}
		private async Task<string> GenerateJwtTokenAsync(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				throw new ArgumentException("Email cannot be null or empty.", nameof(email));
			}

			try
			{
				var user = await _userRepo.GetUserByEmailAsync(email);

				if (user == null)
				{
					throw new InvalidOperationException("User not found.");
				}

				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
					new Claim(ClaimTypes.Email, user.Email),
					new Claim(ClaimTypes.GivenName, user.FirstName ?? "Unknown"),
					new Claim(ClaimTypes.Surname, user.LastName ?? "Unknown")
				};

				var role = await GetUserRoleAsync(email);

				if (!string.IsNullOrEmpty(role))
				{
					claims.Add(new Claim(ClaimTypes.Role, role));
				}

				var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
				var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

				var jwtSecurityToken = new JwtSecurityToken(
					issuer: _configuration["JWT:Issuer"],
					audience: _configuration["JWT:Audience"],
					claims: claims,
					expires: DateTime.UtcNow.AddHours(1),
					signingCredentials: signingCredentials
				);

				return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while generating the JWT token for email: {Email}", email);
				return null;
			}
		}

		private async Task<string> GetUserRoleAsync(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				throw new ArgumentException("Email cannot be null or empty.", nameof(email));
			}

			var user = await _userRepo.GetUserByEmailAsync(email);

			if (user == null)
			{
				throw new ArgumentException($"User not found with email: {email}", nameof(email));
			}

			if (user.Role == null)
			{
				throw new InvalidOperationException($"No role found for user with email: {email}");
			}

			return user.Role.RoleName;
		}

	}
}


