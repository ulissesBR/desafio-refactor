using System.Collections.Generic;
using System;
using MedidorTCP.Entities.Protocol;

namespace MedidorTCP.Entities.Driver
{
    public class LeituraDeMemoriaHandler
    {
        private readonly IMessageHandler _messageHandler;
        private readonly IOutputHandler _outputHandler;
        List<string> registros = new List<string>();

        public LeituraDeMemoriaHandler(IMessageHandler messageHandler, IOutputHandler outputHandler)
        {
            _messageHandler = messageHandler;
            _outputHandler = outputHandler;
        }

        public void LerMemoriaDeMassa(ushort indiceInicial, ushort indiceFinal)
        {
            Console.WriteLine("Iniciando leitura dos registros...");
            RegistroHandler registroHandler = new RegistroHandler(_messageHandler);

            for (ushort indice = indiceInicial; indice <= indiceFinal; indice++)
            {
                if (registroHandler.DefinirIndiceRegistro(indice))
                {
                    var dataHora = new DataHoraHandler(_messageHandler); //LerDataHora();
                    string dataHoraFormatada = dataHora.DataHora;

                    var valorEnergia = new EnergiaHandler(_messageHandler); //LerValorEnergia();
                    string valorEnergiaFormatado = valorEnergia.ValorEnergia;

                    Console.WriteLine("Indice: {0}; Data: {1}; Energia: {2}", indice, dataHoraFormatada, valorEnergiaFormatado);
                    this.registros.Add(string.Format("{0};{1};{2}", indice, dataHoraFormatada, valorEnergiaFormatado));
                }
            }


            //if (registros.Count > 0)
            //{
            //    this._outputHandler.Save(registros, this.numeroDeSerie);
            //}
            //else
            //{
            //    Console.WriteLine("Nenhum registro válido encontrado.");
            //}

        }
    }
}
