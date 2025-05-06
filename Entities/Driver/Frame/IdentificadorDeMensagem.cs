using MedidorTCP.Entities.Enums;
using MedidorTCP.Entities.Driver.TamanhosDasMensagens;
using MedidorTCP.Entities.Driver.FuncoesDasMensagens;
using MedidorTCP.Entities.Driver.BytesDeStatusDasMensagens;

namespace MedidorTCP.Entities.Driver.Frame
{
    public static class IdentificadorDeMensagem
    {
        public static TipoMensagem Identificar(Frame frame, int bufferLength, byte[] buffer)
        {
            if (frame.Function == FuncoesMensagem.FuncErro && frame.FrameLength == TamanhoMensagem.Erro)
            {
                return TipoMensagem.Erro;
            }

            if (frame.Function == FuncoesMensagem.FuncValorEnergia && bufferLength == TamanhoMensagem.ValorEnergia)
            {
                return TipoMensagem.ValorEnergiaTipo;
            }

            if (frame.Function == FuncoesMensagem.FuncDataHora && bufferLength == TamanhoMensagem.DataHora)
            {
                return TipoMensagem.DataHoraTipo;
            }

            if (frame.Function == FuncoesMensagem.FuncRegistroStatus && bufferLength == TamanhoMensagem.RegistroStatus)
            {
                return TipoMensagem.RegistroStatus;
            }

            if (frame.Function == FuncoesMensagem.FuncNumeroDeSerie && bufferLength > TamanhoMensagem.TamanhoMinimo)
            {
                return TipoMensagem.NumeroDeSerieTipo;
            }

            if (frame.Function == FuncoesMensagem.FuncIndiceRegistro && bufferLength == TamanhoMensagem.DefinirIndiceRegistro && frame.IndiceSucessoOuErro == BytesStatus.SucessoAoDefinirRegistro)
            {
                return TipoMensagem.IndiceRegistro;
            }

            return TipoMensagem.Desconhecida;
        }
    }
}
