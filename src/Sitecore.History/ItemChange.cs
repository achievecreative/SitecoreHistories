using System;
using System.Collections.Generic;

namespace SitecoreHistory
{
    public class ItemChange
    {
        public string ID { get; set; }

        public string Path { get; set; }

        public string Editor { get; set; }

        public string Language { get; set; }

        public DateTime Date { get; set; }

        public string ServerName { get; set; }

        public string UserAgent { get; set; }

        public string IpAddress { get; set; }

        public List<FieldChangeDetail> Fields { get; set; }
    }
}
