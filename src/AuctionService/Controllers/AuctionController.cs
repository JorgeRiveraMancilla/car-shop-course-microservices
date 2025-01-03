using AuctionService.Data;
using AuctionService.DTOs;
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
    }
}
