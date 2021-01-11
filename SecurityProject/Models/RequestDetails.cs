using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityProject.Models
{
    public class RequestDetails
    {
        public DateTime LastRequest { get; set; }
        public int NumberOfRequests { get; set; }
        public bool isBlocked { get; set; }
    }
}
