using System;
using System.Collections.Generic;

#nullable disable

namespace conversion_api.Models
{
    public partial class CampaignStore
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public int StoreId { get; set; }
        public string UniqueUrl { get; set; }
        public string ShortCode { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual Store Store { get; set; }
        public sbyte? IsActive { get; set; }
        public sbyte? IsDelete { get; set; }
    }
}
