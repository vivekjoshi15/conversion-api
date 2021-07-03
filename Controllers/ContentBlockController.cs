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
    public class ContentBlockController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContentBlockController> _logger;

        public ContentBlockController(ILogger<ContentBlockController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        // GET: api/ContentBlock
        [HttpGet]
        public IEnumerable<ContentBlock> GetContentBlock()
        {
            return _context.ContentBlocks.Where(c => c.IsDelete != 1).ToList();
        }

        // GET: api/ContentBlock/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContentBlock([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ContentBlock = await _context.ContentBlocks.FindAsync(id);

            if (ContentBlock == null)
            {
                return NotFound();
            }

            return Ok(ContentBlock);
        }

        // PUT: api/ContentBlock/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContentBlock([FromRoute] int id, [FromBody] ContentBlock contentBlock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contentBlock.Id)
            {
                return BadRequest();
            }

            ContentBlock contentBlockObj = _context.ContentBlocks.FirstOrDefault(e => e.Id == id);
            if (contentBlockObj != null)
            {
                contentBlock.ModifiedDate = DateTime.Now;
                contentBlock.CreatedDate = contentBlockObj.CreatedDate;

                _context.Entry(contentBlock).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentBlockExists(id))
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

        // POST: api/ContentBlock
        [HttpPost]
        public async Task<IActionResult> PostContentBlock([FromBody] ContentBlock contentBlock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            contentBlock.CreatedDate = DateTime.Now;
            _context.ContentBlocks.Add(contentBlock);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContentBlock", new { id = contentBlock.Id }, contentBlock);
        }

        // DELETE: api/ContentBlock/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContentBlock([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ContentBlock = await _context.ContentBlocks.FindAsync(id);
            if (ContentBlock == null)
            {
                return NotFound();
            }

            ContentBlock.IsDelete = 1;
            _context.Entry(ContentBlock).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContentBlockExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(ContentBlock);
        }

        private bool ContentBlockExists(long id)
        {
            return _context.ContentBlocks.Any(e => e.Id == id);
        }
    }
}
