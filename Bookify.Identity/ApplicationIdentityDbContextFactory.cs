using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Identity
{
    public class ApplicationIdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
    {
        public ApplicationIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();
            optionsBuilder.UseSqlServer("data source=127.0.0.1:1434; initial catalog=Bookify.Identity; User Id=sa; Password=1234512345Aa$; TrustServerCertificate=true");
            return new ApplicationIdentityDbContext(optionsBuilder.Options);
        }
    }
}