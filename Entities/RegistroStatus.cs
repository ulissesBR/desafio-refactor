namespace MedidorTCP.Entities
{
    public class RegistroStatus
    {
        public ushort IndiceAntigo { get; private set; }
        public ushort IndiceNovo { get; private set; }

        public RegistroStatus(ushort indiceAntigo, ushort indiceNovo)
        {
            IndiceAntigo = indiceAntigo;
            IndiceNovo = indiceNovo;
        }
    }
}
