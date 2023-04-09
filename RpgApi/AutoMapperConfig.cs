using AutoMapper;
using Rpg.Grpc;
using Rpg.Models;

namespace Rpg
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Player, PlayerResponse>().ReverseMap();
            CreateMap<Point, PointResponse>().ReverseMap();
        }
    }
}
