using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
            CreateMap<Item, AuctionDto>();
            CreateMap<CreateAuctionDto, Auction>()
                .ForMember(dest => dest.Item, opt => opt.MapFrom(src => src));
            CreateMap<CreateAuctionDto, Item>();
            CreateMap<AuctionDto, AuctionCreated>();
            CreateMap<Auction, AuctionUpdated>().IncludeMembers(x => x.Item);
            CreateMap<Item, AuctionUpdated>();
            CreateMap<UpdateAuctionDto, Item>()
                .ForMember(
                    dest => dest.Make,
                    opt => opt.Condition(src => !string.IsNullOrEmpty(src.Make))
                )
                .ForMember(
                    dest => dest.Model,
                    opt => opt.Condition(src => !string.IsNullOrEmpty(src.Model))
                )
                .ForMember(
                    dest => dest.Color,
                    opt => opt.Condition(src => !string.IsNullOrEmpty(src.Color))
                )
                .ForMember(dest => dest.Year, opt => opt.Condition(src => 0 < src.Year))
                .ForMember(dest => dest.Mileage, opt => opt.Condition(src => 0 <= src.Mileage))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Auction, opt => opt.Ignore())
                .ForMember(dest => dest.AuctionId, opt => opt.Ignore());
        }
    }
}
