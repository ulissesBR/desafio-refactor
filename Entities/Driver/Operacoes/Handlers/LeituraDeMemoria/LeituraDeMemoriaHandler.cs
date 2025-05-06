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
        IOperations _operations;

        public List<string> Registros = new List<string>();

        public LeituraDeMemoriaHandler(IMessageHandler messageHandler, ILogger logger, IOperations operations)
        {
            _messageHandler = messageHandler;
            _logger = logger.WithContext(nameof(LeituraDeMemoriaHandler));
            _operations = operations;
        }

        public void LerMemoriaDeMassa(int indiceInicial, int indiceFinal)
        {
            Console.WriteLine("Iniciando leitura dos registros...");
            RegistroHandler registroHandler = new RegistroHandler(_messageHandler, _logger, _operations);

            for (int indice = indiceInicial; indice <= indiceFinal; indice++)
            {
                if (registroHandler.DefinirIndiceRegistro(indice))
                {
                    var dataHora = new DataHoraHandler(_messageHandler, _logger, _operations); //LerDataHora();
                    string dataHoraFormatada = dataHora.DataHora;

                    var valorEnergia = new EnergiaHandler(_messageHandler, _logger, _operations); //LerValorEnergia();
                    string valorEnergiaFormatado = valorEnergia.ValorEnergia;

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
