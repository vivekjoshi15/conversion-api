using System;
using System.Collections.Generic;

#nullable disable

namespace conversion_api.Models
{
    public partial class ModuleContact
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int CampaignId { get; set; }
        public int ModuleId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual Store Store { get; set; }
    }
}
