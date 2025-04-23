using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedidorTCP.Entities
{
    public interface IOutputHandler
    {
        void save(List<String> registers, String serialNumber);
    }
}
