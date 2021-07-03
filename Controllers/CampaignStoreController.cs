using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using conversion_api.Models;
using conversion_api.Utilities;

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

        // GET: api/CampaignStore
        [HttpGet("getStoreCampaigns/{id}/{campaignId}")]
        public IEnumerable<CampaignStore> GetStoreCampaigns([FromRoute] int id, [FromRoute] int campaignId)
        {
            return _context.CampaignStores.Where(c => c.IsDelete != 1 && c.StoreId == id && c.CampaignId == campaignId).ToList();
        }

        // GET: api/CampaignStore
        [HttpGet("getCampaignStoreByShortCode/{shortCode}")]
        public CampaignStore GetCampaignStore([FromRoute] string shortCode)
        {
            var CampaignStores = _context.CampaignStores
                    .Include(c => c.Campaign)
                    .Include(c => c.Store)
                    .Include(c => c.CampaignStoreModules)
                    .FirstOrDefault(c => c.IsDelete != 1 && c.ShortCode == shortCode);

            if(CampaignStores != null && CampaignStores.CampaignStoreModules.Count() > 0)
            {
                foreach (var item in CampaignStores.CampaignStoreModules)
                {
                    if (!string.IsNullOrWhiteSpace(item.Content) && item.Content.StartsWith("id="))
                    {
                        item.Content = getContent(item.Content);
                    }
                }
            }
            return CampaignStores;
        }

        string getContent(string content)
        {
            string contents = content;
            string[] contentBlockIdStr = contents.Split('=');
            if (contentBlockIdStr.Length > 1 && !string.IsNullOrWhiteSpace(contentBlockIdStr[1]))
            {
                int contentBlockId = Convert.ToInt32(contentBlockIdStr[1]);
                try
                {
                    var ContentBlock = _context.ContentBlocks.Find(contentBlockId);
                    if (ContentBlock != null)
                    {
                        contents = ContentBlock.Content;
                    }
                }
                catch { }
            }
            return contents;
        }

        // GET: api/CampaignStore
        [HttpGet("getCampaignStores/{companyId}/{id}")]
        public IEnumerable<CampaignStoreData> GetCampaignStores([FromRoute] int companyId, [FromRoute] int id)
        {
            List<CampaignStoreData> campaignStoresData = new();

            try
            {
                List<CampaignStore> campStores = _context.CampaignStores.Where(c => c.IsDelete != 1 && c.CampaignId == id).ToList();
                List<Store> cStores = _context.Stores.Where(c => c.IsDelete != 1 && c.CompanyId == companyId).ToList();

                foreach (var store in campStores)
                {
                    Store tstore = cStores.FirstOrDefault(s => s.Id == store.StoreId);
                    if (tstore != null)
                    {
                        string storeId = tstore.StoreId;

                        CampaignStoreData campaignStoreData = new();
                        campaignStoreData.Id = store.Id;
                        campaignStoreData.CampaignId = store.CampaignId;
                        campaignStoreData.UniqueUrl = store.UniqueUrl;
                        campaignStoreData.IsActive = store.IsActive;
                        campaignStoreData.StoreId = storeId;

                        List<CampaignStoreModule> campaignStoreModules = _context.CampaignStoreModules.Where(c => c.IsDelete != 1 && c.CampaignStoreId == store.Id && c.CampaignId == id).ToList();

                        foreach (var campaignStoreModule in campaignStoreModules)
                        {
                            if (campaignStoreModule.ModuleId == 1)
                            {
                                campaignStoreData.Module1 = campaignStoreModule.Content;
                            }
                            else if (campaignStoreModule.ModuleId == 3)
                            {
                                campaignStoreData.Module2 = campaignStoreModule.Content;
                            }
                            else if (campaignStoreModule.ModuleId == 4)
                            {
                                campaignStoreData.Module3 = campaignStoreModule.Content;
                            }
                            else if (campaignStoreModule.ModuleId == 5)
                            {
                                campaignStoreData.Module4 = campaignStoreModule.Content;
                            }
                            else if (campaignStoreModule.ModuleId == 6)
                            {
                                campaignStoreData.Module5 = campaignStoreModule.Content;
                            }
                            else if (campaignStoreModule.ModuleId == 7)
                            {
                                campaignStoreData.Module6 = campaignStoreModule.Content;
                            }
                            else if (campaignStoreModule.ModuleId == 8)
                            {
                                campaignStoreData.Module7 = campaignStoreModule.Content;
                            }
                            else if (campaignStoreModule.ModuleId == 9)
                            {
                                //if(!string.IsNullOrWhiteSpace(campaignStoreModule.Content) && campaignStoreModule.Content.StartsWith("id="))
                                //{
                                //    string[] contentBlockIdStr = campaignStoreModule.Content.Split('=');
                                //    if (contentBlockIdStr.Length > 1 && !string.IsNullOrWhiteSpace(contentBlockIdStr[1]))
                                //    {
                                //        int contentBlockId = Convert.ToInt32(contentBlockIdStr[1]);
                                //        try
                                //        {
                                //            var ContentBlock = _context.ContentBlocks.Find(contentBlockId);
                                //            if (ContentBlock != null)
                                //            {
                                //                campaignStoreData.Module8 = ContentBlock.Content;
                                //            }
                                //        }
                                //        catch { }
                                //    }
                                //}
                                //else
                                //{
                                campaignStoreData.Module8 = campaignStoreModule.Content;
                                //}
                            }
                        }
                        campaignStoresData.Add(campaignStoreData);
                    }
                }
            }
            catch
            { }

            return campaignStoresData;
        }

        // PUT: api/CampaignStore/5
        [HttpPut("updateCampaignStoreModules/{id}")]
        public async Task<IActionResult> UpdateCampaignStoreModules([FromRoute] int id, [FromBody] CampaignStoreData campaignStoreData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != campaignStoreData.Id)
            {
                return BadRequest();
            }

            CampaignStore campaignStore = _context.CampaignStores.FirstOrDefault(s => s.Id == campaignStoreData.Id);
            if (campaignStore != null)
            {
                int storeId = _context.Stores.FirstOrDefault(s => s.StoreId == campaignStoreData.StoreId).Id;

                campaignStore.StoreId = storeId;
                campaignStore.CampaignId = campaignStoreData.CampaignId;
                campaignStore.UniqueUrl = campaignStoreData.UniqueUrl;
                campaignStore.IsActive = campaignStoreData.IsActive;
                _context.Entry(campaignStore).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                List<CampaignStoreModule> campaignStoreModules = new();
                List<CampaignStoreModule> eCampaignStoreModules = _context.CampaignStoreModules.Where(c => c.IsDelete != 1 && c.CampaignStoreId == campaignStore.Id && c.CampaignId == id).ToList();
                
                CampaignStoreModule campaignStoreModule = eCampaignStoreModules.FirstOrDefault(c => c.ModuleId == 1);
                if (!string.IsNullOrEmpty(campaignStoreData.Module1))
                {
                    if (campaignStoreModule == null)
                    {
                        campaignStoreModule = new();
                        campaignStoreModule.CampaignStoreId = campaignStore.Id;
                        campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                        campaignStoreModule.StoreId = campaignStore.StoreId;
                        campaignStoreModule.ModuleId = 1;
                        campaignStoreModule.Content = campaignStoreData.Module1;
                        campaignStoreModule.IsActive = 1;

                        campaignStoreModules.Add(campaignStoreModule);
                    }
                    else
                    {
                        campaignStoreModule.Content = campaignStoreData.Module1;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (campaignStoreModule != null)
                    {
                        campaignStoreModule.IsDelete = 1;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }

                campaignStoreModule = eCampaignStoreModules.FirstOrDefault(c => c.ModuleId == 3);
                if (!string.IsNullOrEmpty(campaignStoreData.Module2))
                {
                    if (campaignStoreModule == null)
                    {
                        campaignStoreModule = new();
                        campaignStoreModule.CampaignStoreId = campaignStore.Id;
                        campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                        campaignStoreModule.StoreId = campaignStore.StoreId;
                        campaignStoreModule.ModuleId = 2;
                        campaignStoreModule.Content = campaignStoreData.Module2;
                        campaignStoreModule.IsActive = 1;

                        campaignStoreModules.Add(campaignStoreModule);
                    }
                    else
                    {
                        campaignStoreModule.Content = campaignStoreData.Module2;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (campaignStoreModule != null)
                    {
                        campaignStoreModule.IsDelete = 1;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }

                campaignStoreModule = eCampaignStoreModules.FirstOrDefault(c => c.ModuleId == 4);
                if (!string.IsNullOrEmpty(campaignStoreData.Module3))
                {
                    if (campaignStoreModule == null)
                    {
                        campaignStoreModule = new();
                        campaignStoreModule.CampaignStoreId = campaignStore.Id;
                        campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                        campaignStoreModule.StoreId = campaignStore.StoreId;
                        campaignStoreModule.ModuleId = 4;
                        campaignStoreModule.Content = campaignStoreData.Module3;
                        campaignStoreModule.IsActive = 1;

                        campaignStoreModules.Add(campaignStoreModule);
                    }
                    else
                    {
                        campaignStoreModule.Content = campaignStoreData.Module3;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (campaignStoreModule != null)
                    {
                        campaignStoreModule.IsDelete = 1;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }

                campaignStoreModule = eCampaignStoreModules.FirstOrDefault(c => c.ModuleId == 5);
                if (!string.IsNullOrEmpty(campaignStoreData.Module4))
                {
                    if (campaignStoreModule == null)
                    {
                        campaignStoreModule = new();
                        campaignStoreModule.CampaignStoreId = campaignStore.Id;
                        campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                        campaignStoreModule.StoreId = campaignStore.StoreId;
                        campaignStoreModule.ModuleId = 5;
                        campaignStoreModule.Content = campaignStoreData.Module4;
                        campaignStoreModule.IsActive = 1;

                        campaignStoreModules.Add(campaignStoreModule);
                    }
                    else
                    {
                        campaignStoreModule.Content = campaignStoreData.Module4;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (campaignStoreModule != null)
                    {
                        campaignStoreModule.IsDelete = 1;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }

                campaignStoreModule = eCampaignStoreModules.FirstOrDefault(c => c.ModuleId == 6);
                if (!string.IsNullOrEmpty(campaignStoreData.Module5))
                {
                    if (campaignStoreModule == null)
                    {
                        campaignStoreModule = new();
                        campaignStoreModule.CampaignStoreId = campaignStore.Id;
                        campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                        campaignStoreModule.StoreId = campaignStore.StoreId;
                        campaignStoreModule.ModuleId = 6;
                        campaignStoreModule.Content = campaignStoreData.Module5;
                        campaignStoreModule.IsActive = 1;

                        campaignStoreModules.Add(campaignStoreModule);
                    }
                    else
                    {
                        campaignStoreModule.Content = campaignStoreData.Module5;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (campaignStoreModule != null)
                    {
                        campaignStoreModule.IsDelete = 1;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }

                campaignStoreModule = eCampaignStoreModules.FirstOrDefault(c => c.ModuleId == 7);
                if (!string.IsNullOrEmpty(campaignStoreData.Module6))
                {
                    if (campaignStoreModule == null)
                    {
                        campaignStoreModule = new();
                        campaignStoreModule.CampaignStoreId = campaignStore.Id;
                        campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                        campaignStoreModule.StoreId = campaignStore.StoreId;
                        campaignStoreModule.ModuleId = 7;
                        campaignStoreModule.Content = campaignStoreData.Module6;
                        campaignStoreModule.IsActive = 1;

                        campaignStoreModules.Add(campaignStoreModule);
                    }
                    else
                    {
                        campaignStoreModule.Content = campaignStoreData.Module6;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (campaignStoreModule != null)
                    {
                        campaignStoreModule.IsDelete = 1;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }

                campaignStoreModule = eCampaignStoreModules.FirstOrDefault(c => c.ModuleId == 8);
                if (!string.IsNullOrEmpty(campaignStoreData.Module7))
                {
                    if (campaignStoreModule == null)
                    {
                        campaignStoreModule = new();
                        campaignStoreModule.CampaignStoreId = campaignStore.Id;
                        campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                        campaignStoreModule.StoreId = campaignStore.StoreId;
                        campaignStoreModule.ModuleId = 8;
                        campaignStoreModule.Content = campaignStoreData.Module7;
                        campaignStoreModule.IsActive = 1;

                        campaignStoreModules.Add(campaignStoreModule);
                    }
                    else
                    {
                        campaignStoreModule.Content = campaignStoreData.Module7;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (campaignStoreModule != null)
                    {
                        campaignStoreModule.IsDelete = 1;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }

                campaignStoreModule = eCampaignStoreModules.FirstOrDefault(c => c.ModuleId == 9);
                if (!string.IsNullOrEmpty(campaignStoreData.Module8))
                {
                    if (campaignStoreModule == null)
                    {
                        campaignStoreModule = new();
                        campaignStoreModule.CampaignStoreId = campaignStore.Id;
                        campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                        campaignStoreModule.StoreId = campaignStore.StoreId;
                        campaignStoreModule.ModuleId = 9;
                        campaignStoreModule.Content = campaignStoreData.Module8;
                        campaignStoreModule.IsActive = 1;

                        campaignStoreModules.Add(campaignStoreModule);
                    }
                    else
                    {
                        campaignStoreModule.Content = campaignStoreData.Module8;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (campaignStoreModule != null)
                    {
                        campaignStoreModule.IsDelete = 1;
                        _context.Entry(campaignStoreModule).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }

                _context.CampaignStoreModules.AddRange(campaignStoreModules);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        // POST: api/CampaignStore
        [HttpPost("createCampaignStoreModules")]
        public async Task<IActionResult> CreateCampaignStoreModules([FromBody] CampaignStoreData campaignStoreData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int storeId = _context.Stores.FirstOrDefault(s => s.StoreId == campaignStoreData.StoreId).Id;

            CampaignStore campaignStore = new CampaignStore();
            campaignStore.StoreId = storeId;
            campaignStore.CampaignId = campaignStoreData.CampaignId;
            campaignStore.UniqueUrl = "";
            campaignStore.ShortCode = "";
            campaignStore.IsActive = 1;

            _context.CampaignStores.Add(campaignStore);
            await _context.SaveChangesAsync();

            string shortCode = Helper.GetRandomString(6) + "_" + campaignStore.Id;
            string uniqueUrl = "https://conversion.mobeomedia.com/#/" + shortCode;
            campaignStore.ShortCode = shortCode;
            campaignStore.UniqueUrl = Helper.GenerateShortUrl(uniqueUrl);
            _context.Entry(campaignStore).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            List<CampaignStoreModule> campaignStoreModules = new();

            if (!string.IsNullOrEmpty(campaignStoreData.Module1))
            {
                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                campaignStoreModule.StoreId = campaignStore.StoreId;
                campaignStoreModule.ModuleId = 1;
                campaignStoreModule.Content = campaignStoreData.Module1;
                campaignStoreModule.IsActive = 1;

                campaignStoreModules.Add(campaignStoreModule);
            }

            if (!string.IsNullOrEmpty(campaignStoreData.Module2))
            {
                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                campaignStoreModule.StoreId = campaignStore.StoreId;
                campaignStoreModule.ModuleId = 2;
                campaignStoreModule.Content = campaignStoreData.Module2;
                campaignStoreModule.IsActive = 1;

                campaignStoreModules.Add(campaignStoreModule);
            }

            if (!string.IsNullOrEmpty(campaignStoreData.Module3))
            {
                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                campaignStoreModule.StoreId = campaignStore.StoreId;
                campaignStoreModule.ModuleId = 3;
                campaignStoreModule.Content = campaignStoreData.Module3;
                campaignStoreModule.IsActive = 1;

                campaignStoreModules.Add(campaignStoreModule);
            }

            if (!string.IsNullOrEmpty(campaignStoreData.Module4))
            {
                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                campaignStoreModule.StoreId = campaignStore.StoreId;
                campaignStoreModule.ModuleId = 4;
                campaignStoreModule.Content = campaignStoreData.Module4;
                campaignStoreModule.IsActive = 1;

                campaignStoreModules.Add(campaignStoreModule);
            }

            if (!string.IsNullOrEmpty(campaignStoreData.Module5))
            {
                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                campaignStoreModule.StoreId = campaignStore.StoreId;
                campaignStoreModule.ModuleId = 5;
                campaignStoreModule.Content = campaignStoreData.Module5;
                campaignStoreModule.IsActive = 1;

                campaignStoreModules.Add(campaignStoreModule);
            }

            if (!string.IsNullOrEmpty(campaignStoreData.Module6))
            {
                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                campaignStoreModule.StoreId = campaignStore.StoreId;
                campaignStoreModule.ModuleId = 6;
                campaignStoreModule.Content = campaignStoreData.Module6;
                campaignStoreModule.IsActive = 1;

                campaignStoreModules.Add(campaignStoreModule);
            }

            if (!string.IsNullOrEmpty(campaignStoreData.Module7))
            {
                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                campaignStoreModule.StoreId = campaignStore.StoreId;
                campaignStoreModule.ModuleId = 7;
                campaignStoreModule.Content = campaignStoreData.Module7;
                campaignStoreModule.IsActive = 1;

                campaignStoreModules.Add(campaignStoreModule);
            }

            if (!string.IsNullOrEmpty(campaignStoreData.Module8))
            {
                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                campaignStoreModule.CampaignId = campaignStoreData.CampaignId;
                campaignStoreModule.StoreId = campaignStore.StoreId;
                campaignStoreModule.ModuleId = 8;
                campaignStoreModule.Content = campaignStoreData.Module8;
                campaignStoreModule.IsActive = 1;

                campaignStoreModules.Add(campaignStoreModule);
            }

            _context.CampaignStoreModules.AddRange(campaignStoreModules);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCampaignStore", new { id = campaignStore.Id }, campaignStore);
        }

        // DELETE: api/CampaignStore/5
        [HttpDelete("deleteCampaignStoreModules/{id}")]
        public async Task<IActionResult> DeleteCampaignStoreModules([FromRoute] int id)
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

            List<CampaignStoreModule> campaignStoreModules = _context.CampaignStoreModules.Where(c => c.IsDelete != 1 && c.CampaignStoreId == id).ToList();
            if (campaignStoreModules.Count() > 0)
            {
                campaignStoreModules.ForEach(c => { c.IsDelete = 1; _context.Entry(c).State = EntityState.Modified; });
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
    }
}
