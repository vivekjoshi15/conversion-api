using System;
using System.Collections.Generic;

#nullable disable

namespace conversion_api.Models
{
    public partial class Store
    {
        public Store()
        {
            CampaignStoreModules = new HashSet<CampaignStoreModule>();
            CampaignStores = new HashSet<CampaignStore>();
            ModuleContacts = new HashSet<ModuleContact>();
        }

        public int Id { get; set; }
        public string StoreId { get; set; }
        public int CompanyId { get; set; }
        public string UniqueUrl { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string LogoUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string CalendarUrl { get; set; }
        public string ContactFormUrl { get; set; }
        public string FacebookUrl { get; set; }
        public sbyte? IsActive { get; set; }
        public sbyte? IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<CampaignStoreModule> CampaignStoreModules { get; set; }
        public virtual ICollection<CampaignStore> CampaignStores { get; set; }
        public virtual ICollection<ModuleContact> ModuleContacts { get; set; }
    }
}
