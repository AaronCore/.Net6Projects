using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VueAdmin.EntityFrameworkCore.EntityFrameworkCore
{
    /* This is your actual DbContext used on runtime.
     * It includes only your entities.
     * It does not include entities of the used modules, because each module has already
     * its own DbContext class. If you want to share some database tables with the used modules,
     * just create a structure like done for AppUser.
     *
     * Don't use this DbContext for database migrations since it does not contain tables of the
     * used modules (as explained above). See VueAdminMigrationsDbContext for migrations.
     */
    [ConnectionStringName("Default")]
    public class VueAdminDbContext : AbpDbContext<VueAdminDbContext>
    {
        public VueAdminDbContext(DbContextOptions<VueAdminDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /* Configure the shared tables (with included modules) here */

            //builder.Entity<AppUser>(b =>
            //{
            //    b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Users"); //Sharing the same table "AbpUsers" with the IdentityUser

            //    b.ConfigureByConvention();

            //    /* Configure mappings for your additional properties
            //     * Also see the VueAdminEfCoreEntityExtensionMappings class
            //     */
            //});

            /* Configure your own tables/entities inside the ConfigureVueAdmin method */

            builder.ConfigureVueAdmin();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
