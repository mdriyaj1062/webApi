using CollageApp.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace CollageApp.Data
{
    public class CollegeDBContext : DbContext
    {
        // In this constructor we will recieve the sql server Connection related details
        public CollegeDBContext(DbContextOptions<CollegeDBContext> options) : base(options) 
        { 

        }
       
       public DbSet<Student> Students {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // table 1
           modelBuilder.ApplyConfiguration(new StudentConfig());
            // if new table such as table 2, then again
            //  modelBuilder.ApplyConfiguration(new S tudentConfig()); 


        }
    }
}
  