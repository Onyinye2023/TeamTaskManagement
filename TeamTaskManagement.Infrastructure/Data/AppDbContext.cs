namespace TeamTaskManagement.Infrastructure.Data
{
	using Microsoft.EntityFrameworkCore;
	using TeamTaskManagement.Domain.Entities;

	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<User> Users { get; set; } = default!;
		public DbSet<Team> Teams { get; set; } = default!;
		public DbSet<ProjectTask> Tasks { get; set; } = default!;
		public DbSet<Role> Roles { get; set; }
		public DbSet<TeamUser> TeamUsers { get; set; } = default!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// User -> CreatedTasks (One-to-Many, No Cascade)
			modelBuilder.Entity<ProjectTask>()
				.HasOne(t => t.CreatedBy)
				.WithMany(u => u.CreatedTasks)
				.HasForeignKey(t => t.CreatedByUserId)
				.IsRequired()
				.OnDelete(DeleteBehavior.NoAction); 

			// User -> AssignedTasks (One-to-Many, Optional)
			modelBuilder.Entity<ProjectTask>()
				.HasOne(t => t.AssignedTo)
				.WithMany(u => u.AssignedTasks)
				.HasForeignKey(t => t.AssignedToUserId)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.NoAction); 

			// Team -> Tasks (One-to-Many)
			modelBuilder.Entity<ProjectTask>()
				.HasOne(t => t.Team)
				.WithMany(t => t.Tasks)
				.HasForeignKey(t => t.TeamId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

			// User -> Teams (CreatedBy)
			modelBuilder.Entity<Team>()
				.HasOne(t => t.CreatedBy)
				.WithMany()
				.HasForeignKey(t => t.CreatedByUserId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade); 

			// TeamUser (Many-to-Many between Team and User)
			modelBuilder.Entity<TeamUser>()
				.HasKey(tu => new { tu.TeamId, tu.UserId });

			modelBuilder.Entity<TeamUser>()
				.HasOne(tu => tu.Team)
				.WithMany(t => t.Members)
				.HasForeignKey(tu => tu.TeamId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TeamUser>()
				.HasOne(tu => tu.User)
				.WithMany(u => u.Teams)
				.HasForeignKey(tu => tu.UserId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<User>()
				.HasOne(u => u.Role)
				.WithMany(r => r.Users)
				.HasForeignKey(u => u.RoleId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			// Configure TaskStatus enum to store as string
			modelBuilder.Entity<ProjectTask>()
				.Property(t => t.Status)
				.HasConversion<string>();
		}
		private void SeedData(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Role>().HasData(
				new Role { RoleId = 1, RoleName = "SuperAdmin" },
				new Role { RoleId = 2, RoleName = "User" }
			);
		}
	}
}
