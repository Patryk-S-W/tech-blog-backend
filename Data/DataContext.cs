namespace tech_blog_backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Announcement> Announcements => Set<Announcement>();
        public DbSet<User> Users => Set<User>();
    }
}