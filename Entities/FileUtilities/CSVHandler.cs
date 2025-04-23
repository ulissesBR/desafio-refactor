using System;
using System.Collections.Generic;
using System.IO;
using MedidorTCP.Entities.Protocol;

namespace MedidorTCP.Entities.FileUtilities
{
    class CSVHandler : IOutputHandler
    {
        private String _fileName;

        public CSVHandler(String fileName)
        {
            this._fileName = fileName;
        }

        public void Save(List<String> registros, String serialNumber)
        {
            string fileName = string.Format("{0}.csv", this._fileName);
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
