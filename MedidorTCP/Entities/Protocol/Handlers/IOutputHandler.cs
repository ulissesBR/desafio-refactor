using System;
using System.Collections.Generic;

namespace MedidorTCP.Entities.Protocol
{
    public interface IOutputHandler
    {
        void Save(List<String> registers, String serialNumber);
    }
}
