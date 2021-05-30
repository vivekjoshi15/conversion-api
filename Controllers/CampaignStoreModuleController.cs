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
    public class CampaignStoreModuleController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CampaignStoreModuleController> _logger;

        public CampaignStoreModuleController(ILogger<CampaignStoreModuleController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        // GET: api/CampaignStoreModule
        [HttpGet]
        public IEnumerable<CampaignStoreModule> GetCampaignStoreModule()
        {
            return _context.CampaignStoreModules.Where(c => c.IsDelete != 1).ToList();
        }

        // GET: api/CampaignStoreModule/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampaignStoreModule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var CampaignStoreModule = await _context.CampaignStoreModules.FindAsync(id);

            if (CampaignStoreModule == null)
            {
                return NotFound();
            }

            return Ok(CampaignStoreModule);
        }

        // PUT: api/CampaignStoreModule/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaignStoreModule([FromRoute] int id, [FromBody] CampaignStoreModule CampaignStoreModule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != CampaignStoreModule.Id)
            {
                return BadRequest();
            }

            _context.Entry(CampaignStoreModule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampaignStoreModuleExists(id))
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

        // POST: api/CampaignStoreModule
        [HttpPost]
        public async Task<IActionResult> PostCampaignStoreModule([FromBody] CampaignStoreModule CampaignStoreModule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CampaignStoreModules.Add(CampaignStoreModule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCampaignStoreModule", new { id = CampaignStoreModule.Id }, CampaignStoreModule);
        }

        // DELETE: api/CampaignStoreModule/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaignStoreModule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var CampaignStoreModule = await _context.CampaignStoreModules.FindAsync(id);
            if (CampaignStoreModule == null)
            {
                return NotFound();
            }

            CampaignStoreModule.IsDelete = 1;
            _context.Entry(CampaignStoreModule).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampaignStoreModuleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(CampaignStoreModule);
        }

        private bool CampaignStoreModuleExists(long id)
        {
            return _context.CampaignStoreModules.Any(e => e.Id == id);
        }
    }
}
