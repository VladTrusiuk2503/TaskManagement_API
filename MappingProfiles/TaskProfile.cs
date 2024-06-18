using AutoMapper;
using TaskManagement_API.Models;

namespace TaskManagement_API.MappingProfiles
{
    public class TaskProfile : Profile
    {
        public void CreateMap()
        {
            CreateMap<TaskModel, Task>()
                .ReverseMap();
        }
    }
}