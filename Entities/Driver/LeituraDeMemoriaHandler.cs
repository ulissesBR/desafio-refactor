using System.Collections.Generic;
using System;
using MedidorTCP.Entities.Protocol;
using MedidorTCP.Entities.Logging;

namespace MedidorTCP.Entities.Driver
{
    public class LeituraDeMemoriaHandler
    {
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger _logger;
        
        public List<string> Registros = new List<string>();

        public LeituraDeMemoriaHandler(IMessageHandler messageHandler, ILogger logger)
        {
            _messageHandler = messageHandler;
            _logger = logger.WithContext(nameof(LeituraDeMemoriaHandler));
        }

        public void LerMemoriaDeMassa(ushort indiceInicial, ushort indiceFinal)
        {
            Console.WriteLine("Iniciando leitura dos registros...");
            RegistroHandler registroHandler = new RegistroHandler(_messageHandler, _logger);

            for (ushort indice = indiceInicial; indice <= indiceFinal; indice++)
            {
                if (registroHandler.DefinirIndiceRegistro(indice))
                {
                    var dataHora = new DataHoraHandler(_messageHandler, _logger); //LerDataHora();
                    string dataHoraFormatada = dataHora.DataHora;

                    var valorEnergia = new EnergiaHandler(_messageHandler, _logger); //LerValorEnergia();
                    string valorEnergiaFormatado = valorEnergia.ValorEnergia;

                    //Console.WriteLine("Indice: {0}; Data: {1}; Energia: {2}", indice, dataHoraFormatada, valorEnergiaFormatado);
                    _logger.Info($"Indice: {indice}; Data: {dataHoraFormatada}; Energia: {valorEnergiaFormatado}");
                    this.Registros.Add(string.Format("{0};{1};{2}", indice, dataHoraFormatada, valorEnergiaFormatado));
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
