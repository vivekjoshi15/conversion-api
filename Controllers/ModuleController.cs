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
    public class ModuleController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ModuleController> _logger;

        public ModuleController(ILogger<ModuleController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Module
        [HttpGet]
        public IEnumerable<Module> GetModule()
        {
            return _context.Modules.Where(c => c.IsDelete != 1).ToList();
        }

        // GET: api/Module/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Module = await _context.Modules.FindAsync(id);

            if (Module == null)
            {
                return NotFound();
            }

            return Ok(Module);
        }

        // PUT: api/Module/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModule([FromRoute] int id, [FromBody] Module Module)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Module.Id)
            {
                return BadRequest();
            }

            _context.Entry(Module).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleExists(id))
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

        // POST: api/Module
        [HttpPost]
        public async Task<IActionResult> PostModule([FromBody] Module Module)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Modules.Add(Module);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModule", new { id = Module.Id }, Module);
        }

        // DELETE: api/Module/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Module = await _context.Modules.FindAsync(id);
            if (Module == null)
            {
                return NotFound();
            }

            Module.IsDelete = 1;
            _context.Entry(Module).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(Module);
        }

        private bool ModuleExists(long id)
        {
            return _context.Modules.Any(e => e.Id == id);
        }
    }
}
