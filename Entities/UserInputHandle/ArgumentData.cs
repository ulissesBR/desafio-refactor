using System;

namespace MedidorTCP.Entities.UserInputHandle
{
    public class ArgumentData
    {

        public String Ip { get; set; }
        public int Port { get; set; }
        public int FirstIndex { get; set; }
        public int LastIndex { get; set; }

        public ArgumentData(String ip, String port, String firstIndex, String lastIndex)
        {
            this.Ip = ip;
            this.Port = Int32.Parse(port);
            this.FirstIndex = Int32.Parse(firstIndex);
            this.LastIndex = Int32.Parse(lastIndex);
        }

        public override string ToString()
        {
            return String.Format("IP: {0}\nPort: {1}\nFirstIndex: {2}\nLastIndex: {3}", this.Ip, this.Port, this.FirstIndex, this.LastIndex);
        }
    }
}
