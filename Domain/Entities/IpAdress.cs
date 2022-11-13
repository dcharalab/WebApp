using Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class IpAdress: Entity
    {

        public string Ip { get; set; } = null!;

        public IpAdress()
        {
            this.Id = new Guid();
        }
    }
}
