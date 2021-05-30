using System;
using System.Collections.Generic;

#nullable disable

namespace conversion_api.Models
{
    public partial class Module
    {
        public Module()
        {
            CampaignStoreModules = new HashSet<CampaignStoreModule>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public sbyte IsActive { get; set; }
        public sbyte? IsDelete { get; set; }

        public virtual ICollection<CampaignStoreModule> CampaignStoreModules { get; set; }
    }
}
