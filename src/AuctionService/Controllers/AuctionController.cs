using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController(
        DataContext dataContext,
        IMapper mapper,
        IPublishEndpoint publishEndpoint
    ) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IMapper _mapper = mapper;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date)
        {
            var query = _dataContext.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {
                query = query.Where(x =>
                    x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0
                );
            }
            return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
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
            var auction = _mapper.Map<Auction>(createAuctionDto);

            // FIXME: This should be set to the authenticated user
            auction.Seller = "test";

            _dataContext.Auctions.Add(auction);
            var newAuction = _mapper.Map<AuctionDto>(auction);

            var auctionCreated = _mapper.Map<AuctionCreated>(newAuction);
            await _publishEndpoint.Publish(auctionCreated);

            if (0 < await _dataContext.SaveChangesAsync())
            {
                return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);
            }

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

            var auctionUpdated = _mapper.Map<AuctionUpdated>(auction);
            await _publishEndpoint.Publish(auctionUpdated);

            if (0 < await _dataContext.SaveChangesAsync())
            {
                return Ok();
            }

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

            var auctionDeleted = new AuctionDeleted { Id = auction.Id.ToString() };
            await _publishEndpoint.Publish(auctionDeleted);

            if (0 < await _dataContext.SaveChangesAsync())
            {
                return Ok();
            }

            return BadRequest("Problem saving changes");
        }
    }
}
