using AutoMapper;
using MedvetitleBot.UpdateChat.Models;
using MedvetitleBot.UpdateChat.ViewModels;

namespace MedvetitleBot.UpdateChat.Mappers
{
	public class MapperProfile : Profile
    {
		public MapperProfile()
		{
			CreateMap<ChatEntity, Chat>().ReverseMap();
            CreateMap<TitleOptionEntity, TitleOption>().ReverseMap();
        }
	}
}

