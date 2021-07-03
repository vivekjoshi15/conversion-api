using System;
using System.Collections.Generic;

#nullable disable

namespace conversion_api.Models
{
    public partial class CampaignStatistic
    {
        public int Id { get; set; }
        public int? CampaignId { get; set; }
        public int? StoreId { get; set; }
        public int? ModuleId { get; set; }
        public int? CampaignStoreId { get; set; }
        public int? CampaignStoreModuleId { get; set; }
        public string Browser { get; set; }
        public string IpAddress { get; set; }
        public string Os { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual CampaignStore CampaignStore { get; set; }
        public virtual CampaignStoreModule CampaignStoreModule { get; set; }
        public virtual Module Module { get; set; }
        public virtual Store Store { get; set; }
    }
}
