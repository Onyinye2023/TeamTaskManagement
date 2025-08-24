namespace TeamTaskManagement.Application.MappingProfiles
{
    using AutoMapper;
    using TeamTaskManagement.Application.DTOs;
    using TeamTaskManagement.Domain.Entities;

    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
			CreateMap<RegisterDTO, User>()
						   .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
						   .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

			CreateMap<User, UserDTO>().ReverseMap();

			CreateMap<CreateTeamDTO, Team>();

			CreateMap<Team, TeamResponseDTO>();

			CreateMap<Team, TeamDTO>()
				.ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedByUserId))
				.ForMember(dest => dest.CreatedByEmail, opt => opt.MapFrom(src => src.CreatedBy.Email));

			CreateMap<Team, TeamResponseDTO>();

			CreateMap<ProjectTask, TaskDTO>()
                .ForMember(dest => dest.CreatedByEmail, opt => opt.MapFrom(src => src.CreatedBy.Email));
           
            CreateMap<CreateTaskDTO, ProjectTask>();

			CreateMap<UpdateTaskDTO, ProjectTask>();
		}
	}
}
