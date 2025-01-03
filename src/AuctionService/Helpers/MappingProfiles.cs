using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
            CreateMap<Item, AuctionDto>();
        }
    }
}
