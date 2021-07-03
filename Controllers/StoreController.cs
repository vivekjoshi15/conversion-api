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

            var store = await _context.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        // PUT: api/Store/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStore([FromRoute] int id, [FromBody] Store store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != store.Id)
            {
                return BadRequest();
            }

            Store storeObj = _context.Stores.FirstOrDefault(e => e.Id == id);
            if (storeObj != null)
            {
                if (string.IsNullOrWhiteSpace(store.UniqueUrl))
                {
                    store.UniqueUrl = "";
                }

                store.CreatedDate = storeObj.CreatedDate;
                store.ModifiedDate = DateTime.Now;
                _context.Entry(store).State = EntityState.Modified;

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
            }

            return NoContent();
        }

        // POST: api/Store
        [HttpPost]
        public async Task<IActionResult> PostStore([FromBody] Store store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            store.UniqueUrl = "";
            store.CreatedDate = DateTime.Now;
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStore", new { id = store.Id }, store);
        }

        // POST: api/Store/bulkStoreUpload
        [HttpPut("bulkStoreUpload/{id}")]
        public async Task<StoreResult> BulkStoreUpload([FromRoute] int id, [FromBody] List<Store> stores)
        {
            StoreResult result = new StoreResult();
            try
            {
                if (id > 0 && stores.Count > 0)
                {
                    List<Store> validStores = new();
                    List<Store> invalidStores = new();
                    List<string> storeIds = new();

                    List<Store> cStores = _context.Stores.Where(c => c.IsDelete != 1 && c.CompanyId == id).ToList();
                    foreach (var store in stores)
                    {
                        if (!storeIds.Contains(store.StoreId) && !StoreIdExists(store.StoreId, id, cStores))
                        {
                            store.CreatedDate = DateTime.Now;
                            store.UniqueUrl = "";

                            validStores.Add(store);
                            storeIds.Add(store.StoreId);
                        }
                        else if (!storeIds.Contains(store.StoreId) && StoreIdExists(store.StoreId, id, cStores))
                        {
                            store.ModifiedDate = DateTime.Now;

                            Store cStore = cStores.FirstOrDefault(c=>c.StoreId == store.StoreId);
                            if(cStore != null)
                            {
                                store.Id = cStore.Id;
                                store.CreatedDate = cStore.CreatedDate;
                                invalidStores.Add(store);
                            }
                            storeIds.Add(store.StoreId);
                        }
                        //else
                        //{
                        //    invalidStores.Add(store);
                        //}
                    }

                    if (validStores.Count > 0)
                    {
                        _context.Stores.AddRange(validStores);
                        await _context.SaveChangesAsync();
                    }

                    if (invalidStores.Count > 0)
                    {
                        _context.Stores.UpdateRange(validStores);
                        await _context.SaveChangesAsync();
                    }

                    result.message = "success";
                    result.validStores = validStores;
                    result.invalidStores = invalidStores;
                }
                else
                {
                    result.message = "no valid store";
                    result.validStores = new();
                    result.invalidStores = new();
                }
            }
            catch(Exception ex)
            {
                result.message = "error while uploading stores, "+ ex.Message;
                result.validStores = new();
                result.invalidStores = new();
            }

            return result;
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

        private bool StoreIdExists(string storeId, int companyId, List<Store> cStores)
        {
            return cStores.Any(e => e.CompanyId == companyId && e.StoreId == storeId);
        }

        private bool StoreExists(long id)
        {
            return _context.Stores.Any(e => e.Id == id);
        }
    }

    public class StoreResult
    {        
        public string message { get; set; }
        public List<Store> validStores { get; set; }
        public List<Store> invalidStores { get; set; }
    }
}
