using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using VueAdmin.Domain.Shared;
using VueAdmin.Domain.System;

namespace VueAdmin.EntityFrameworkCore.EntityFrameworkCore
{
    public static class VueAdminDbContextModelCreatingExtensions
    {
        // 自定义 Code First 约定：https://docs.microsoft.com/zh-cn/ef/ef6/modeling/code-first/conventions/custom
        public static void ConfigureVueAdmin(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Entity<AccountEntity>(b =>
            {
                b.ToTable(VueAdminConsts.DbTablePrefix + DbTableName.Account);
                b.HasKey(x => x.Id);
                b.Property(x => x.Account).HasMaxLength(200).IsRequired();
                b.Property(x => x.Password).HasMaxLength(100);
            });

            builder.Entity<RoleEntity>(b =>
            {
                b.ToTable(VueAdminConsts.DbTablePrefix + DbTableName.Role);
                b.Property(x => x.Name).IsRequired();
                b.HasKey(x => x.Id);
            });

            builder.Entity<MenuEntity>(b =>
            {
                b.ToTable(VueAdminConsts.DbTablePrefix + DbTableName.Menu);
                b.HasKey(x => x.Id);
            });

            builder.Entity<RoleMenuEntity>(b =>
            {
                b.ToTable(VueAdminConsts.DbTablePrefix + DbTableName.RoleMenu);
                b.HasKey(x => x.Id);
            });
        }
    }
}