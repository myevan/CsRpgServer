using AutoMapper;
using Rpg.Grpc;
using Rpg.Models;

namespace Rpg
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Player, PlayerResponse>().ReverseMap();
            CreateMap<Point, PointResponse>().ReverseMap();
        }
    }
}
