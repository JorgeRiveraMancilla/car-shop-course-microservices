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
        private readonly DataContext _dataContext =
            dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        private readonly IMapper _mapper =
            mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IPublishEndpoint _publishEndpoint =
            publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAuctions(string? date)
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
        public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
        {
            var auction = await _dataContext
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
            var auction = _mapper.Map<Auction>(auctionDto);

            auction.Seller =
                User.Identity?.Name ?? throw new InvalidOperationException("User not found");

            _dataContext.Auctions.Add(auction);

            var newAuction = _mapper.Map<AuctionDto>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            var result = await _dataContext.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Failed to create auction");

            return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, newAuction);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<AuctionDto>> UpdateAuction(
            Guid id,
            [FromBody] UpdateAuctionDto auctionDto
        )
        {
            var auction = await _dataContext
                .Auctions.Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            if (auction.Seller != User.Identity?.Name)
                return Forbid();

            auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
            auction.Item.Year = auctionDto.Year;
            auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = auctionDto.Mileage;

            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

            var result = await _dataContext.SaveChangesAsync() > 0;

            if (!result)
            {
                return BadRequest("Failed to update auction");
            }

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _dataContext.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }

            if (auction.Seller != User.Identity?.Name)
                return Forbid();
            _dataContext.Auctions.Remove(auction);

            await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

            var result = await _dataContext.SaveChangesAsync() > 0;

            if (!result)
            {
                return BadRequest("Failed to delete auction");
            }

            return Ok();
        }
    }
}
