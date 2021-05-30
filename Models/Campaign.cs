using System;
using System.Collections.Generic;

#nullable disable

namespace conversion_api.Models
{
    public partial class Campaign
    {
        public Campaign()
        {
            CampaignStoreModules = new HashSet<CampaignStoreModule>();
            CampaignStores = new HashSet<CampaignStore>();
            ModuleContacts = new HashSet<ModuleContact>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public string UniqueUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TextColor { get; set; }
        public string HeaderColor { get; set; }
        public string HeaderText { get; set; }
        public sbyte IsActive { get; set; }
        public sbyte? IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<CampaignStoreModule> CampaignStoreModules { get; set; }
        public virtual ICollection<CampaignStore> CampaignStores { get; set; }
        public virtual ICollection<ModuleContact> ModuleContacts { get; set; }
    }
}
