using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace VTTests
{
    public class VTTestContext : DbContext, IDisposable
    {
        public VTTestContext() : base("name=VTTestsProject")
        {
        }
        public DbSet<IntroducerCodes> IntroducerCodes { get; set; }
        public DbSet<DebtorsList> DebtorsList { get; set; }
        public DbSet<Suspense> Suspense { get; set; }
    }
    
}
