using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooLinkDTOs.Animal
{
    public class AnimalsImportDTO
    {
        public IEnumerable<AnimalGroupDTO> Animals { get; set; }
    }
}
