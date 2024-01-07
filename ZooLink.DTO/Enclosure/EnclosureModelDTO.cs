using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooLink.Domain.Enums;
using ZooLink.Domain.Models;

namespace ZooLink.DTO
{
    public class EnclosureModelDTO
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public EnclosureSize Size { get; set; }

        public EnclosureLocation Location { get; set; }

        public required IEnumerable<ZooAsset> Objects { get; set; } 

        public required IEnumerable<AnimalModelDTO> Animals { get; set; }
    }
}
