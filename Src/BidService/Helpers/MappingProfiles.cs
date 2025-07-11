using AutoMapper;
using BidService.DTOs;
using BidService.Entities;
using Contracts;

namespace BidService.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Bid, BidDto>();
            CreateMap<Bid, BidPlaced>();
        }
    }
}
