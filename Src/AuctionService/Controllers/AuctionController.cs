using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<List<AuctionDto>>> GetAuctions([FromQuery] string? date)
        {
            IQueryable<Auction> query = _dataContext
                .Auctions.OrderBy(x => x.Item!.Make)
                .AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {
                query = query.Where(x =>
                    0 < x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime())
                );
            }

            return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuction([FromRoute] Guid id)
        {
            Auction? auction = await _dataContext
                .Auctions.Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            return _mapper.Map<AuctionDto>(auction);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(
            [FromBody] CreateAuctionDto auctionDto
        )
        {
            Auction auction = _mapper.Map<Auction>(auctionDto);

            auction.Seller =
                User.Identity?.Name ?? throw new InvalidOperationException("User not found");

            _dataContext.Auctions.Add(auction);

            AuctionDto newAuction = _mapper.Map<AuctionDto>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            bool result = 0 < await _dataContext.SaveChangesAsync();

            if (!result)
            {
                return BadRequest("Failed to create auction");
            }

            return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, newAuction);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<AuctionDto>> UpdateAuction(
            [FromRoute] Guid id,
            [FromBody] UpdateAuctionDto auctionDto
        )
        {
            Auction? auction = await _dataContext
                .Auctions.Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            if (auction.Seller != User.Identity?.Name)
            {
                return Forbid();
            }

            _mapper.Map(auctionDto, auction.Item);
            auction.UpdatedAt = DateTime.UtcNow;

            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

            bool result = 0 < await _dataContext.SaveChangesAsync();

            if (!result)
            {
                return BadRequest("Failed to update auction");
            }

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction([FromRoute] Guid id)
        {
            Auction? auction = await _dataContext.Auctions.FindAsync(id);

            if (auction == null)
            {
                return NotFound();
            }

            if (auction.Seller != User.Identity?.Name)
            {
                return Forbid();
            }

            _dataContext.Auctions.Remove(auction);

            await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

            bool result = 0 < await _dataContext.SaveChangesAsync();

            if (!result)
            {
                return BadRequest("Failed to delete auction");
            }

            return Ok();
        }
    }
}
