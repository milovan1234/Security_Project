using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityProject.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int NumberOfInvalidLogin { get; set; }
        public DateTime LastInvalidLogin { get; set; }
    }
}
