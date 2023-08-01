using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tech_blog_backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Announcement> Announcements => Set<Announcement>();
    }
}