namespace TeamTaskManagement.Api.Extensions
{
	using System.Reflection;
	using System.Text;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
	using TeamTaskManagement.Application.IRepositories.ITaskRepo;
	using TeamTaskManagement.Application.IRepositories.ITeamRepo;
    using TeamTaskManagement.Application.IRepositories.IUserRepo;
	using TeamTaskManagement.Application.IServices.ITaskService;
	using TeamTaskManagement.Application.IServices.ITeamService;
    using TeamTaskManagement.Application.IServices.IUserService;
    using TeamTaskManagement.Application.MappingProfiles;
    using TeamTaskManagement.Infrastructure.Data;
	using TeamTaskManagement.Infrastructure.Repositories.TaskRepo;
	using TeamTaskManagement.Infrastructure.Repositories.TeamRepo;
    using TeamTaskManagement.Infrastructure.Repositories.UserRepo;
	using TeamTaskManagement.Infrastructure.Services.TaskService;
	using TeamTaskManagement.Infrastructure.Services.TeamService;
    using TeamTaskManagement.Infrastructure.Services.UserService;

    public static class TaskManagementExtensionRegistrar
	{
		public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<AppDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("Default"))
			);

			AddAuthentication(services, configuration);

			AddSwagger(services);

			services.AddAutoMapper(configuration => configuration.AddProfile<MappingProfiles>());

			services.AddControllers();

			//Add Repositories and Services
			services.AddScoped<IUserRepo, UserRepo>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<ITeamRepo, TeamRepo>();
			services.AddScoped<ITeamService, TeamService>();
			services.AddScoped<ITaskRepo, TaskRepo>();
			services.AddScoped<ITaskService,TaskService>();
		}

		private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
		{
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = configuration["JWT:Issuer"],
						ValidAudience = configuration["JWT:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"])),
						ClockSkew = TimeSpan.FromMinutes(5) 
					};
					options.Events = new JwtBearerEvents
					{
						OnAuthenticationFailed = context =>
						{
							var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
							logger.LogError("JWT validation failed: {Error}", context.Exception.Message);
							return Task.CompletedTask;
						}
					};
				});
		}
		private static void AddSwagger(IServiceCollection services)
		{
			services.AddSwaggerGen(option =>
			{
				option.SwaggerDoc("v1", new OpenApiInfo { Title = "TeamTaskManagement", Version = "v1" });
				option.EnableAnnotations();
				option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Bearer"
				});
				option.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[] {}
					}
				});
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				option.IncludeXmlComments(xmlPath);
			});
		}
	}
}
