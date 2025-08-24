namespace TeamTaskManagement.Infrastructure.Utilities
{
	using TeamTaskManagement.Domain.Entities;
	using TeamTaskManagement.Infrastructure.Data;

	public class Seeder
	{
		public static void SeedRoles(AppDbContext context)
		{
			if (!context.Roles.Any())
			{
				var roles = new List<Role>
				{
					new Role
					{
						RoleName = "SuperAdmin"
					},

					 new Role
					{
						RoleName = "User"
					}
				};

				context.Roles.AddRange(roles);
				context.SaveChanges();
			}
		}
	}
}
