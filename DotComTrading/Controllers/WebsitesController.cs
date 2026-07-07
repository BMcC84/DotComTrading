using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DotComTrading.Models;
using DotComTrading.Services;
using System.Runtime.CompilerServices;
using DotComTrading.Data;
using Microsoft.EntityFrameworkCore;

namespace DotComTrading.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebsitesController : ControllerBase
    {
        private readonly WebsiteRepository _websiteRepository;
        private readonly DotComTradingDBContext _context;
        private readonly CreateWebsiteService _createWebsiteService;

        public WebsitesController(WebsiteRepository websiteRepository, DotComTradingDBContext context, CreateWebsiteService createWebsiteService)
        {
            _websiteRepository = websiteRepository;
            _context = context;
            _createWebsiteService = createWebsiteService;
        }

        //Retrieves full list of websites
        [HttpGet]
        public async Task<ActionResult<List<Website>>> GetAll()
        {
            var websites = await _websiteRepository.GetAllAsync();

            //Recalculates price before returning it
            foreach (var website in websites) 
            {
                website.Price = ValuateWebsite.CalculatePrice(website);
            }

            return Ok(websites);
        }

        //Returns a website given its id
        [HttpGet("{id}")]
        public async Task<ActionResult<Website>> GetById(int id)
        {
            var website = await _websiteRepository.GetByIdAsync(id);

            if (website == null)
            {
                return NotFound();
            }

            return Ok(website);
        }

        //Creates a new website through external data services
        [HttpPost]
        public async Task<ActionResult<Website>> CreateWebsite([FromBody] CreateWebsiteRequest request)
        {
            try
            {
				if (request == null || string.IsNullOrWhiteSpace(request.Url))
				{
					return BadRequest("Please Enter URL For Website Being Added.");
				}

				Website? website = await _createWebsiteService.CreateWebsite(request.Url);

				if (website == null)
				{
					return BadRequest("Website Not Found");
				}

				_context.Websites.Add(website);
				await _context.SaveChangesAsync();
				return Ok(website);
			}
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Retrieves recent price changes of a website stock
        [HttpGet("price-changes")]
        public async Task<IActionResult> GetPriceChanges()
        {
            var result = await _context.WebsitePriceRecords.GroupBy(r => r.WebsiteId).Select(g => new
            {
                WebsiteId = g.Key,
                Records = g.OrderByDescending(r => r.TimeOfRecording).Take(8).ToList()
            }).ToListAsync();

            var changes = result.Select(r =>
            {
                var current = r.Records.FirstOrDefault();
                var past = r.Records.LastOrDefault();

                return new
                {
                    WebsiteId = r.WebsiteId,
                    currentPrice = current!.Price,
                    pastPrice = past!.Price
                };
            });
            return Ok(changes);
        }

        //Retrieves price history used in creating graphs
        [HttpGet("{id}/price-history")]
        public async Task<IActionResult> GetPriceHistory(int id, [FromQuery] int count = 40)
        {
            if(count <= 0)
            {
                count = 40;
            }

            var records = await _context.WebsitePriceRecords.Where(r => r.WebsiteId == id).OrderByDescending(r => r.TimeOfRecording).Take(count).OrderBy(r => r.TimeOfRecording).ToListAsync();
            return Ok(records);
        }
    }
}
