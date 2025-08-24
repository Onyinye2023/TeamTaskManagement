namespace TeamTaskManagement.Infrastructure.Repositories.UserRepo
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;
	using TeamTaskManagement.Application.DTOs;
	using TeamTaskManagement.Application.IRepositories.IUserRepo;
	using TeamTaskManagement.Domain.Entities;
	using TeamTaskManagement.Infrastructure.Data;

	public class UserRepo : IUserRepo
	{
		private readonly AppDbContext _context;
		private readonly ILogger<UserRepo> _logger;
		public UserRepo(ILogger<UserRepo> logger, AppDbContext context)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<User> RegisterAdminAsync(User adminDTO, int roleId)
		{
			if (adminDTO == null)
			{
				throw new ArgumentException("Admin details are missing.", nameof(adminDTO));
			}

			try
			{
				// Check if admin already exists
				var existingAdmin = await _context.Users
					.FirstOrDefaultAsync(u => u.Email == adminDTO.Email);

				if (existingAdmin != null)
				{
					throw new InvalidOperationException("Admin already exists.");
				}

				// Ensure role exists
				var role = await _context.Roles
					.FirstOrDefaultAsync(r => r.RoleId == roleId);

				if (role == null)
				{
					throw new InvalidOperationException("Invalid role ID.");
				}

				// Optional: enforce that the role must be "Admin"
				if (!string.Equals(role.RoleName, "SuperAdmin", StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidOperationException("The specified role is not an Admin role.");
				}

				// Create admin user
				var admin = new User
				{
					FirstName = adminDTO.FirstName,
					LastName = adminDTO.LastName,
					Email = adminDTO.Email,
					PasswordHash = HashPassword(adminDTO.PasswordHash),
					RoleId = roleId,
					Role = role,
					CreatedAt = DateTime.UtcNow
				};

				_context.Users.Add(admin);
				await _context.SaveChangesAsync();

				return admin;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred during Admin registration.");
				throw new Exception("An error occurred during Admin registration.", ex);
			}
		}
		public async Task<ResponseDTO> RegisterUserAsync(User userDTO, int roleId)
		{
			if (userDTO == null)
			{
				throw new ArgumentException("User details are missing.", nameof(userDTO));
			}

			try
			{
				var existingUser = await _context.Users
					.FirstOrDefaultAsync(p => p.Email == userDTO.Email);

				if (existingUser != null)
				{
					return new ResponseDTO
					{
						Success = false,
						Message = "User already exists."
					};
				}

				var role = await _context.Roles
					.FirstOrDefaultAsync(r => r.RoleId == roleId);

				if (role == null)
				{
					return new ResponseDTO
					{
						Success = false,
						Message = "Invalid role ID."
					};
				}

				if (roleId != 2)
				{

					return new ResponseDTO
					{
						Success = false,
						Message = "Invalid role ID for a user."
					};
				}

				var user = new User
				{
					FirstName = userDTO.FirstName,
					LastName = userDTO.LastName,
					Email = userDTO.Email,
					PasswordHash = HashPassword(userDTO.PasswordHash),
					RoleId = roleId,
					Role = role,
					CreatedAt = DateTime.UtcNow
				};

				_context.Users.Add(user);
				await _context.SaveChangesAsync();

				return new ResponseDTO
				{
					Success = true,
					Email = user.Email,
					Password = userDTO.PasswordHash,
					Message = $"User registered successfully with role: {role.RoleName}"
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred during User registration.");
				throw new Exception("An error occurred during User registration.", ex);
			}
		}

		public async Task<User?> LoginAsync(LoginDTO loginDTO)
		{
			if (loginDTO == null)
			{
				_logger.LogError("LoginDTO is null.");
				throw new ArgumentException("Login credentials are empty.", nameof(loginDTO));
			}

			try
			{
				var existingUser = await _context.Users
					.Include(r => r.Role)
					.FirstOrDefaultAsync(a => a.Email == loginDTO.Email);

				if (existingUser == null)
				{
					_logger.LogWarning("Login failed: Invalid email {Email}", loginDTO.Email);
					return null;
				}

				bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password, existingUser.PasswordHash);
				if (!isPasswordValid)
				{
					_logger.LogWarning("Login failed: Invalid password for email {Email}", loginDTO.Email);
					return null;
				}

				return existingUser;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login for email: {Email}", loginDTO.Email);
				throw new Exception("An error occurred during login.", ex);
			}
		}

		public async Task<User?> GetUserByIdAsync(Guid userId)
		{
			try
			{
				var user = await _context.Users
					.FirstOrDefaultAsync(u => u.UserId == userId);
				if (user == null)
				{
					_logger.LogWarning("User not found for ID: {UserId}", userId);
				}
				return user;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving user with ID: {UserId}", userId);
				throw new Exception("An error occurred while retrieving the user.", ex);
			}
		}

		public async Task<User?> GetUserByEmailAsync(string email)
		{
			try
			{
				var user = await _context.Users
					.FirstOrDefaultAsync(u => u.Email == email);
				if (user == null)
				{
					_logger.LogWarning("User not found for email: {Email}", email);
				}
				return user;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving user with email: {Email}", email);
				throw new Exception("An error occurred while retrieving the user.", ex);
			}
		}
		public string HashPassword(string password)
		{
			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 12);
			return hashedPassword;
		}
	}
}
