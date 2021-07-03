using System;
using System.Collections.Generic;

#nullable disable

namespace conversion_api.Models
{
    public partial class CampaignStatistic
    {
        public CampaignStatistic()
        {
            Campaigns = new HashSet<Campaign>();
            Stores = new HashSet<Store>();
            Modules = new HashSet<Module>();
            CampaignStores = new HashSet<CampaignStore>();
            CampaignStoreModules = new HashSet<CampaignStoreModule>();
        }

        public int Id { get; set; }
        public int CampaignId { get; set; }
        public int StoreId { get; set; }
        public int ModuleId { get; set; }
        public int CampaignStoreId { get; set; }
        public int CampaignStoreModuleId { get; set; }
        public string Browser { get; set; }
        public string IPAddress { get; set; }
        public string OS { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Store> Stores { get; set; }
        public virtual ICollection<Campaign> Campaigns { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
        public virtual ICollection<CampaignStore> CampaignStores { get; set; }
        public virtual ICollection<CampaignStoreModule> CampaignStoreModules { get; set; }
    }
}
