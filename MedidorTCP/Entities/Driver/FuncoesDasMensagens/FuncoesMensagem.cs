namespace MedidorTCP.Entities.Driver.FuncoesDasMensagens
{
    public static class FuncoesMensagem
    {
        public const byte FuncErro = 0xFF;
        public const byte FuncValorEnergia = 0x85;
        public const byte FuncDataHora = 0x84;
        public const byte FuncRegistroStatus = 0x82;
        public const byte FuncNumeroDeSerie = 0x81;
        public const byte FuncIndiceRegistro = 0x83;
    }
}
