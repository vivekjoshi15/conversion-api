using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using conversion_api.Models;

namespace conversion_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CampaignController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CampaignController> _logger;

        public CampaignController(ILogger<CampaignController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Campaign
        [HttpGet]
        public IEnumerable<Campaign> GetCampaign()
        {
            return _context.Campaigns.Where(c=>c.IsDelete != 1).ToList();
        }

        // GET: api/Campaign/getCompanyCampaigns/{id}
        [HttpGet("getCompanyCampaigns/{id}")]
        public IEnumerable<Campaign> GetCompanyCampaigns([FromRoute] int id)
        {
            return _context.Campaigns.Where(c => c.IsDelete != 1 && c.CompanyId == id).ToList();
        }

        // GET: api/Campaign/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampaign([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Campaign = await _context.Campaigns.FindAsync(id);

            if (Campaign == null)
            {
                return NotFound();
            }

            return Ok(Campaign);
        }

        // PUT: api/Campaign/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaign([FromRoute] int id, [FromBody] Campaign Campaign)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Campaign.Id)
            {
                return BadRequest();
            }

            Campaign.ModifiedDate = DateTime.Now;
            _context.Entry(Campaign).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampaignExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Campaign
        [HttpPost]
        public async Task<IActionResult> PostCampaign([FromBody] Campaign Campaign)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Campaign.CreatedDate = DateTime.Now;
            _context.Campaigns.Add(Campaign);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCampaign", new { id = Campaign.Id }, Campaign);
        }

        // DELETE: api/Campaign/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Campaign = await _context.Campaigns.FindAsync(id);
            if (Campaign == null)
            {
                return NotFound();
            }

            Campaign.IsDelete = 1;
            _context.Entry(Campaign).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampaignExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(Campaign);
        }

        private bool CampaignExists(long id)
        {
            return _context.Campaigns.Any(e => e.Id == id);
        }
    }
}
