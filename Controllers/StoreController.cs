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
    public class StoreController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StoreController> _logger;

        public StoreController(ILogger<StoreController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Store
        [HttpGet]
        public IEnumerable<Store> GetStore()
        {
            return _context.Stores.Where(c => c.IsDelete != 1).ToList();
        }

        // GET: api/Store/getCompanyStores/{id}
        [HttpGet("getCompanyStores/{id}")]
        public IEnumerable<Store> GetCompanyStores([FromRoute] int id)
        {
            return _context.Stores.Where(c => c.IsDelete != 1 && c.CompanyId == id).ToList();
        }

        // GET: api/Store/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStore([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Store = await _context.Stores.FindAsync(id);

            if (Store == null)
            {
                return NotFound();
            }

            return Ok(Store);
        }

        // PUT: api/Store/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStore([FromRoute] int id, [FromBody] Store Store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Store.Id)
            {
                return BadRequest();
            }

            Store.ModifiedDate = DateTime.Now;
            _context.Entry(Store).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(id))
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

        // POST: api/Store
        [HttpPost]
        public async Task<IActionResult> PostStore([FromBody] Store Store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Store.CreatedDate = DateTime.Now;
            _context.Stores.Add(Store);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStore", new { id = Store.Id }, Store);
        }

        // DELETE: api/Store/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Store = await _context.Stores.FindAsync(id);
            if (Store == null)
            {
                return NotFound();
            }

            Store.IsDelete = 1;
            _context.Entry(Store).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(Store);
        }

        private bool StoreExists(long id)
        {
            return _context.Stores.Any(e => e.Id == id);
        }
    }
}
