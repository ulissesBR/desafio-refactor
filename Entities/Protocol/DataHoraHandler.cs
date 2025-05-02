using System;
using MedidorTCP.Entities.Driver;
using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Exceptions;

namespace MedidorTCP.Entities.Protocol
{
    public class DataHoraHandler
    {
        private readonly IMessageHandler _messageHandler;
        public string DataHora { get; private set; }
        var mensagemRx;

        public DataHoraHandler(IMessageHandler messageHandler)
        {
            this._messageHandler = messageHandler;
            this.DataHora = LerDataHora();
        }

        public string LerDataHora()
        {
            byte[] rawPayload = { 0x7D, 0x00, 0x04 };
            try
            {
                mensagemRx = Operations.TryExchangeMessage(_messageHandler, rawPayload, (int)FunctionLength.LerDataHoraLength);
                if (IsDataHora())
                {
                    this.DataHora = GetDataHora();
                    return this.DataHora;
                }
            }
            catch (Exception ex) when (ex is ChecksumMismatchException || ex is MessageNotReceivedException)
            {
                Console.WriteLine("ERRO [Data e Hora]: " + ex.Message);
            }
            return "Erro na leitura da data/hora.";
        }

        public bool IsDataHora()
        {
            return mensagemRx.BufferLength == 9 && this.GetFunction() == 0x84;
        }
        public String GetDataHora()
        {
            int ano = (((this.Buffer[3] << 8) | this.Buffer[4]) >> 4);
            int mes = this.Buffer[4] & 0x0F;
            int dia = this.Buffer[5] >> 3;
            int hora = ((this.Buffer[5] & 0x07) << 2) | (this.Buffer[6] >> 6);
            int minuto = this.Buffer[6] & 0x3F;
            int segundo = this.Buffer[7] >> 2;

            Console.WriteLine("Data/Hora: {0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}",
                                    ano, mes, dia, hora, minuto, segundo);

            return ano.ToString("D4") + "-" +
                mes.ToString("D2") + "-" +
                dia.ToString("D2") + " " +
                hora.ToString("D2") + ":" +
                minuto.ToString("D2") + ":" +
                segundo.ToString("D2");
        }
    }
}