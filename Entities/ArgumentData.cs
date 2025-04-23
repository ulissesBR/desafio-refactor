using System;

namespace MedidorTCP.Entities
{
    public class ArgumentData
    {

        public String ip { get; set; }
        public int port { get; set; }
        public int firstIndex { get; set; }
        public int lastIndex { get; set; }

        public ArgumentData(String ip, String port, String firstIndex, String lastIndex)
        {
            this.ip = ip;
            this.port = Int32.Parse(port);
            this.firstIndex = Int32.Parse(firstIndex);
            this.lastIndex = Int32.Parse(lastIndex);
        }

        public override string ToString()
        {
            return String.Format("IP: {0}\nPort: {1}\nFirstIndex: {2}\nLastIndex: {3}", this.ip, this.port, this.firstIndex, this.lastIndex);
        }
    }
}
