using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController(DataContext dataContext, IMapper mapper) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            var auctions = await _dataContext
                .Auctions.Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .ProjectTo<AuctionDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _dataContext
                .Auctions.Include(x => x.Item)
                .ProjectTo<AuctionDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
                return NotFound();

            return Ok(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = mapper.Map<Auction>(createAuctionDto);

            // FIXME: This should be set to the authenticated user
            auction.Seller = "test";

            _dataContext.Auctions.Add(auction);

            var result = await _dataContext.SaveChangesAsync();

            if (result == 0)
                return BadRequest("Failed to create auction");

            return CreatedAtAction(
                nameof(GetAuctionById),
                new { auction.Id },
                mapper.Map<AuctionDto>(auction)
            );
        }
    }
}
