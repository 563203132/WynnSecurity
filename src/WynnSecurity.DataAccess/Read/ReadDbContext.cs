using Microsoft.EntityFrameworkCore;
using WynnSecurity.DataAccess.Write;

namespace WynnSecurity.DataAccess.Read
{
    public class ReadDbContext : WynnDbContext
    {
        public ReadDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
