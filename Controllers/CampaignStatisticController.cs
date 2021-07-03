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
    public class CampaignStatisticController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CampaignStatisticController> _logger;

        public CampaignStatisticController(ILogger<CampaignStatisticController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        // GET: api/CampaignStatistic
        [HttpGet]
        public IEnumerable<CampaignStatistic> GetCampaignStatistic()
        {
            return _context.CampaignStatistics.ToList();
        }

        // GET: api/CampaignStatisticByCompany
        [HttpGet("CampaignStatisticByCompany/{id}")]
        public IEnumerable<CampaignStatistic> CampaignStatisticByCompany([FromRoute] int id)
        {
            return (from stats in _context.CampaignStatistics
                    join store in _context.Stores on stats.StoreId equals store.Id
                    where store.CompanyId == id
                    select stats);
        }

        // GET: api/CampaignStatisticByStore
        [HttpGet("CampaignStatisticByStore/{id}")]
        public IEnumerable<CampaignStatistic> CampaignStatisticByStore([FromRoute] int id)
        {
            return _context.CampaignStatistics.Where(c=>c.StoreId == id).ToList();
        }

        // GET: api/CampaignStatisticByCampaign
        [HttpGet("CampaignStatisticByCampaign/{id}")]
        public IEnumerable<CampaignStatistic> CampaignStatisticByCampaign([FromRoute] int id)
        {
            return _context.CampaignStatistics.Where(c => c.CampaignId == id).ToList();
        }

        // GET: api/CampaignStatisticByModule
        [HttpGet("CampaignStatisticByModule/{id}")]
        public IEnumerable<CampaignStatistic> CampaignStatisticByModule([FromRoute] int id)
        {
            return _context.CampaignStatistics.Where(c => c.ModuleId == id).ToList();
        }

        // GET: api/CampaignStatisticByCampaignStore
        [HttpGet("CampaignStatisticByCampaignStore/{id}")]
        public IEnumerable<CampaignStatistic> CampaignStatisticByCampaignStore([FromRoute] int id)
        {
            return _context.CampaignStatistics.Where(c => c.CampaignStoreId == id).ToList();
        }

        // GET: api/CampaignStatistic/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampaignStatistic([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var CampaignStatistic = await _context.CampaignStatistics.FindAsync(id);

            if (CampaignStatistic == null)
            {
                return NotFound();
            }

            return Ok(CampaignStatistic);
        }

        // PUT: api/CampaignStatistic/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaignStatistic([FromRoute] int id, [FromBody] CampaignStatistic CampaignStatistic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != CampaignStatistic.Id)
            {
                return BadRequest();
            }

            CampaignStatistic CampaignStatisticObj = _context.CampaignStatistics.FirstOrDefault(e => e.Id == id);
            if (CampaignStatisticObj != null)
            {
                CampaignStatistic.CreatedDate = CampaignStatisticObj.CreatedDate;

                _context.Entry(CampaignStatistic).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CampaignStatisticExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return NoContent();
        }

        // POST: api/CampaignStatistic
        [HttpPost]
        public async Task<IActionResult> PostCampaignStatistic([FromBody] CampaignStatistic CampaignStatistic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CampaignStatistic.CreatedDate = DateTime.Now;
            _context.CampaignStatistics.Add(CampaignStatistic);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCampaignStatistic", new { id = CampaignStatistic.Id }, CampaignStatistic);
        }

        // DELETE: api/CampaignStatistic/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaignStatistic([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var CampaignStatistic = await _context.CampaignStatistics.FindAsync(id);
            if (CampaignStatistic == null)
            {
                return NotFound();
            }

            _context.Entry(CampaignStatistic).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampaignStatisticExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(CampaignStatistic);
        }

        private bool CampaignStatisticExists(long id)
        {
            return _context.CampaignStatistics.Any(e => e.Id == id);
        }
    }
}
