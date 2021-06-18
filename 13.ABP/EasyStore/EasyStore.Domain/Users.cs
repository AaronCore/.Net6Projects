using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace EasyStore.Domain
{
    public class Users : Entity<int>
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
