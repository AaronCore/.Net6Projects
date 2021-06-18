using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace EasyStore.Application.User.Dto
{
    public class UserDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
