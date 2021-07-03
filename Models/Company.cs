using System;
using System.Collections.Generic;

#nullable disable

namespace conversion_api.Models
{
    public partial class Company
    {
        public Company()
        {
            Campaigns = new HashSet<Campaign>();
            Stores = new HashSet<Store>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string CalendarUrl { get; set; }
        public string ContactFormUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string HeaderText { get; set; }
        public string FooterText { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public sbyte? IsActive { get; set; }
        public sbyte? IsDelete { get; set; }

        public virtual ICollection<Campaign> Campaigns { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
    }
}
