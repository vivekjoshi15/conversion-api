using System;
using System.Collections.Generic;

#nullable disable

namespace conversion_api.Models
{
    public partial class ContentBlock
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public sbyte? IsDelete { get; set; }
    }
}
