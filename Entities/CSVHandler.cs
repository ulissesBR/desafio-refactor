using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MedidorTCP.Entities
{
    class CSVHandler : IOutputHandler
    {
        private String fileName;

        public CSVHandler(String fileName)
        {
            this.fileName = fileName;
        }

        public void save(List<String> registros, String serialNumber)
        {
            string fileName = string.Format("{0}.csv", this.fileName);
            fileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine(serialNumber);
                foreach (var registro in registros)
                {
                    writer.WriteLine(registro);
                }
            }
            Console.WriteLine(string.Format("Dados salvos em {0}", fileName));
        }
    }
}
