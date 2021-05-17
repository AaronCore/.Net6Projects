using SysEntity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysEntityFrameworkCore
{
    public class SysDbContext : DbContext
    {
        public SysDbContext() : base("name=SysDb")
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<SysDbContext>(null);
            modelBuilder.Entity<UserEntity>().ToTable("Users", "dbo");
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<UserEntity> Users { set; get; }
    }
}
