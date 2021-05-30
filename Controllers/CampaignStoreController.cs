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
    public class CampaignStoreController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CampaignStoreController> _logger;

        public CampaignStoreController(ILogger<CampaignStoreController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        // GET: api/CampaignStore
        [HttpGet]
        public IEnumerable<CampaignStore> GetCampaignStore()
        {
            return _context.CampaignStores.Where(c => c.IsDelete != 1).ToList();
        }

        // GET: api/Store
        [HttpGet("getStoreCampaigns/{id}/{campaignId}")]
        public IEnumerable<CampaignStore> GetStoreCampaigns([FromRoute] int id, [FromRoute] int campaignId)
        {
            return _context.CampaignStores.Where(c => c.IsDelete != 1 && c.StoreId == id && c.CampaignId == campaignId).ToList();
        }

        // GET: api/CampaignStore/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampaignStore([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var CampaignStore = await _context.CampaignStores.FindAsync(id);

            if (CampaignStore == null)
            {
                return NotFound();
            }

            return Ok(CampaignStore);
        }

        // PUT: api/CampaignStore/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaignStore([FromRoute] int id, [FromBody] CampaignStore CampaignStore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != CampaignStore.Id)
            {
                return BadRequest();
            }

            _context.Entry(CampaignStore).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampaignStoreExists(id))
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

        // POST: api/CampaignStore
        [HttpPost]
        public async Task<IActionResult> PostCampaignStore([FromBody] CampaignStore CampaignStore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CampaignStores.Add(CampaignStore);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCampaignStore", new { id = CampaignStore.Id }, CampaignStore);
        }

        // DELETE: api/CampaignStore/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaignStore([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var CampaignStore = await _context.CampaignStores.FindAsync(id);
            if (CampaignStore == null)
            {
                return NotFound();
            }

            CampaignStore.IsDelete = 1;
            _context.Entry(CampaignStore).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampaignStoreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(CampaignStore);
        }

        private bool CampaignStoreExists(long id)
        {
            return _context.CampaignStores.Any(e => e.Id == id);
        }
    }
}
