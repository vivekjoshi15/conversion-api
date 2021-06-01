using System;
using System.Collections.Generic;

#nullable disable

namespace conversion_api.Models
{
    public partial class CampaignStoreModule
    {
        public int Id { get; set; }
        public int CampaignStoreId { get; set; }
        public int CampaignId { get; set; }
        public int StoreId { get; set; }
        public int ModuleId { get; set; }
        public string Content { get; set; }
        public sbyte? IsActive { get; set; }
        public sbyte? IsDelete { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual Module Module { get; set; }
        public virtual Store Store { get; set; }
    }
}
