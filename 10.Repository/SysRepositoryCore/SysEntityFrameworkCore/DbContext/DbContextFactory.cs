using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SysCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace SysEntityFrameworkCore
{
    public class DbContextFactory : IDesignTimeDbContextFactory<SysDbContext>
    {
        public SysDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SysDbContext>();
            var connection = JsonConfigurationHelper.GetAppSettings<ConnectionService>("appsettings.json", "ConnectionStrings");
            builder.UseSqlServer(connection.SqlConnection);
            return new SysDbContext(builder.Options);
        }
    }
}
