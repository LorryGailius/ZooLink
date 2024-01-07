using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooLink.DTO
{
    public class AnimalGroupDTO
    {
        public required string Species { get; set; }

        public required string Food { get; set; }

        public int Amount { get; set; }
    }
}
