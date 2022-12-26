using AutoMapper;
using Flitter.Api.Data.Caching;
using Flitter.Api.Dtos;
using Flitter.Api.Models;
using Flitter.Api.Models.AuthDb;

namespace Flitter.Api.Data
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<PostCreate, Post>();
            CreateMap<Post, PostView>();
            CreateMap<Post, PostDocument>().ReverseMap();
            CreateMap<PostDocument, PostView>();
            CreateMap<Poll, PollView>();
            CreateMap<Option, OptionView>();
            
            CreateMap<User, UserView>();
        }
    }
}
