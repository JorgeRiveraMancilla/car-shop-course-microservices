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

            if (0 < await _dataContext.SaveChangesAsync())
                return CreatedAtAction(
                    nameof(GetAuctionById),
                    new { auction.Id },
                    mapper.Map<AuctionDto>(auction)
                );

            return BadRequest("Problem saving changes");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await _dataContext
                .Auctions.Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
                return NotFound();

            // TODO: Check if the authenticated user is the seller

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            if (0 < await _dataContext.SaveChangesAsync())
                return Ok();

            return BadRequest("Problem saving changes");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _dataContext.Auctions.FindAsync(id);

            if (auction == null)
                return NotFound();

            // TODO: Check if the authenticated user is the seller

            _dataContext.Auctions.Remove(auction);

            if (0 < await _dataContext.SaveChangesAsync())
                return Ok();

            return BadRequest("Problem saving changes");
        }
    }
}
