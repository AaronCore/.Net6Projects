using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using EasyStore.Domain;

namespace EasyStore.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public class EasyStoreDbContext : AbpDbContext<EasyStoreDbContext>
    {
        public DbSet<Users> Users { get; set; }

        public EasyStoreDbContext(DbContextOptions<EasyStoreDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
