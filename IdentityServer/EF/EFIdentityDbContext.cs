using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.EF
{
    public class EFIdentityDbContext: IdentityDbContext<ApplicationUser,ApplicationRole, string>
    {

        public EFIdentityDbContext(DbContextOptions<EFIdentityDbContext> dbContextOptions):base(dbContextOptions)
        {

        }
    }
}
