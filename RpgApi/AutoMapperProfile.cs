using AutoMapper;
using Rpg.Models;
using Rpg.Protocols;

namespace Rpg
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Player, PlayerPacket>().ReverseMap();
            CreateMap<Point, PointPacket>().ReverseMap();
        }
    }
}
