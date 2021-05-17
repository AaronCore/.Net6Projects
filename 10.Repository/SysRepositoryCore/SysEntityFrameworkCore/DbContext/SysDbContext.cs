using Microsoft.EntityFrameworkCore;
using SysEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SysEntityFrameworkCore
{
    public class SysDbContext : DbContext
    {
        public SysDbContext(DbContextOptions<SysDbContext> options) : base(options)
        {
            // Database.EnsureCreated();
        }
        public DbSet<PersonEntity> Persons { get; set; }
    }
}
