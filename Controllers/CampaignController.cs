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
            return _context.Campaigns.Include(i => i.CampaignStores).Where(c => c.IsDelete != 1 && c.CompanyId == id).ToList();
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

            Campaign campaignObj = _context.Campaigns.FirstOrDefault(e => e.Id == id);
            if (campaignObj != null)
            {
                Campaign.CreatedDate = campaignObj.CreatedDate;
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

        // POST: api/Campaign/bulkCampaignStoreUpload
        [HttpPut("bulkCampaignStoreUpload/{companyId}/{id}")]
        public async Task<CampaignStoreResult> BulkCampaignStoreUpload([FromRoute] int companyId, [FromRoute] int id, [FromBody] List<CampaignStoreData> stores)
        {
            CampaignStoreResult result = new CampaignStoreResult();
            try
            {
                if (id > 0 && stores.Count > 0)
                {
                    List<CampaignStoreData> validCampaignStores = new();
                    List<CampaignStoreData> invalidCampaignStores = new();
                    List<string> storeIds = new();

                    List<Store> cStores = _context.Stores.Where(c => c.IsDelete != 1 && c.CompanyId == companyId).ToList();
                    List<CampaignStore> campStores = _context.CampaignStores.Where(c => c.IsDelete != 1 && c.CampaignId == id).ToList();
                    
                    foreach (var store in stores)
                    {
                        int storeId = cStores.FirstOrDefault(s => s.StoreId == store.StoreId).Id;
                        CampaignStore cs = CampaignStoreExists(storeId, id, campStores);
                        if (!storeIds.Contains(store.StoreId) && StoreIdExists(store.StoreId, companyId, cStores) && cs == null)
                        {
                            store.UniqueUrl = "";
                            store.IsActive = 1;
                            validCampaignStores.Add(store);
                            storeIds.Add(store.StoreId);
                        }
                        else
                        {
                            if (cs != null)
                            {
                                store.UniqueUrl = cs.UniqueUrl;
                                store.Message = "StoreId already added to this campaign";
                            }
                            else
                            {
                                store.Message = "Invalid StoreId";
                            }
                            invalidCampaignStores.Add(store);
                        }
                    }

                    if (validCampaignStores.Count > 0)
                    {
                        List<Module> modules = _context.Modules.Where(c => c.IsDelete != 1).ToList();

                        foreach (var store in validCampaignStores)
                        {
                            int storeId = cStores.FirstOrDefault(s => s.StoreId == store.StoreId).Id;

                            CampaignStore campaignStore = new CampaignStore();
                            campaignStore.StoreId = storeId;
                            campaignStore.CampaignId = id;
                            campaignStore.UniqueUrl = "";
                            campaignStore.IsActive = 1;

                            _context.CampaignStores.Add(campaignStore);
                            await _context.SaveChangesAsync();

                            string shortCode = Helper.GetRandomString(6) + "_" + campaignStore.Id;
                            string uniqueUrl = "https://conversion.mobeomedia.com/#/" + shortCode;
                            campaignStore.ShortCode = shortCode;
                            campaignStore.UniqueUrl = Helper.GenerateShortUrl(uniqueUrl);
                            _context.Entry(campaignStore).State = EntityState.Modified;
                            await _context.SaveChangesAsync();

                            store.UniqueUrl = Helper.GenerateShortUrl(uniqueUrl);

                            List<CampaignStoreModule> campaignStoreModules = new();

                            if (!string.IsNullOrEmpty(store.Module1))
                            {
                                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                                campaignStoreModule.CampaignId = id;
                                campaignStoreModule.StoreId = campaignStore.StoreId;
                                campaignStoreModule.ModuleId = 1;
                                campaignStoreModule.Content = store.Module1;
                                campaignStoreModule.IsActive = 1;

                                campaignStoreModules.Add(campaignStoreModule);
                            }

                            if (!string.IsNullOrEmpty(store.Module2))
                            {
                                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                                campaignStoreModule.CampaignId = id;
                                campaignStoreModule.StoreId = campaignStore.StoreId;
                                campaignStoreModule.ModuleId = 2;
                                campaignStoreModule.Content = store.Module2;
                                campaignStoreModule.IsActive = 1;

                                campaignStoreModules.Add(campaignStoreModule);
                            }

                            if (!string.IsNullOrEmpty(store.Module3))
                            {
                                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                                campaignStoreModule.CampaignId = id;
                                campaignStoreModule.StoreId = campaignStore.StoreId;
                                campaignStoreModule.ModuleId = 3;
                                campaignStoreModule.Content = store.Module3;
                                campaignStoreModule.IsActive = 1;

                                campaignStoreModules.Add(campaignStoreModule);
                            }

                            if (!string.IsNullOrEmpty(store.Module4))
                            {
                                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                                campaignStoreModule.CampaignId = id;
                                campaignStoreModule.StoreId = campaignStore.StoreId;
                                campaignStoreModule.ModuleId = 4;
                                campaignStoreModule.Content = store.Module4;
                                campaignStoreModule.IsActive = 1;

                                campaignStoreModules.Add(campaignStoreModule);
                            }

                            if (!string.IsNullOrEmpty(store.Module5))
                            {
                                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                                campaignStoreModule.CampaignId = id;
                                campaignStoreModule.StoreId = campaignStore.StoreId;
                                campaignStoreModule.ModuleId = 5;
                                campaignStoreModule.Content = store.Module5;
                                campaignStoreModule.IsActive = 1;

                                campaignStoreModules.Add(campaignStoreModule);
                            }

                            if (!string.IsNullOrEmpty(store.Module6))
                            {
                                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                                campaignStoreModule.CampaignId = id;
                                campaignStoreModule.StoreId = campaignStore.StoreId;
                                campaignStoreModule.ModuleId = 6;
                                campaignStoreModule.Content = store.Module6;
                                campaignStoreModule.IsActive = 1;

                                campaignStoreModules.Add(campaignStoreModule);
                            }

                            if (!string.IsNullOrEmpty(store.Module7))
                            {
                                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                                campaignStoreModule.CampaignId = id;
                                campaignStoreModule.StoreId = campaignStore.StoreId;
                                campaignStoreModule.ModuleId = 7;
                                campaignStoreModule.Content = store.Module7;
                                campaignStoreModule.IsActive = 1;

                                campaignStoreModules.Add(campaignStoreModule);
                            }

                            if (!string.IsNullOrEmpty(store.Module8))
                            {
                                CampaignStoreModule campaignStoreModule = new CampaignStoreModule();
                                campaignStoreModule.CampaignStoreId = campaignStore.Id;
                                campaignStoreModule.CampaignId = id;
                                campaignStoreModule.StoreId = campaignStore.StoreId;
                                campaignStoreModule.ModuleId = 8;
                                campaignStoreModule.Content = store.Module8;
                                campaignStoreModule.IsActive = 1;

                                campaignStoreModules.Add(campaignStoreModule);
                            }

                            _context.CampaignStoreModules.AddRange(campaignStoreModules);
                            await _context.SaveChangesAsync();
                        }

                    }

                    result.message = "success";
                    result.validCampaignStores = validCampaignStores;
                    result.invalidCampaignStores = invalidCampaignStores;
                }
                else
                {
                    result.message = "no valid campaign store";
                    result.validCampaignStores = new();
                    result.invalidCampaignStores = new();
                }
            }
            catch (Exception ex)
            {
                result.message = "error while uploading campaign stores, " + ex.Message;
                result.validCampaignStores = new();
                result.invalidCampaignStores = new();
            }

            return result;
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

        private bool StoreIdExists(string storeId, int companyId, List<Store> cStores)
        {
            return cStores.Any(e => e.CompanyId == companyId && e.StoreId == storeId);
        }
        private CampaignStore CampaignStoreExists(int storeId, int campaignId, List<CampaignStore> campStores)
        {
            return campStores.FirstOrDefault(e => e.CampaignId == campaignId && e.StoreId == storeId);
        }

        private bool CampaignExists(long id)
        {
            return _context.Campaigns.Any(e => e.Id == id);
        }
    }

    public class CampaignStoreResult
    {
        public string message { get; set; }
        public List<CampaignStoreData> validCampaignStores { get; set; }
        public List<CampaignStoreData> invalidCampaignStores { get; set; }
    }

    public class CampaignStoreData
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string StoreId { get; set; }
        public string UniqueUrl { get; set; }
        public string Module1 { get; set; }
        public string Module2 { get; set; }
        public string Module3 { get; set; }
        public string Module4 { get; set; }
        public string Module5 { get; set; }
        public string Module6 { get; set; }
        public string Module7 { get; set; }
        public string Module8 { get; set; }
        public sbyte? IsActive { get; set; }
        public string Message { get; set; }
    }
}
