using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Identity.Models
{
    public class Role : IdentityRole
    {
        public Role() { }
        public Role(string name) : base(name) { }
    }
}
