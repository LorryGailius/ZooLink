using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooLink.DTO
{
    public class AnimalsImportDTO
    {
        public IEnumerable<AnimalGroupDTO> Animals { get; set; }
    }
}
