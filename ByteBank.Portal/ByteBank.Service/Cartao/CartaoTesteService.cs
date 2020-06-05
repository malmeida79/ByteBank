namespace ByteBank.Service.Cartao
{
    public class CartaoTesteService : ICartaoService
    {
        public string ObterCartaoDeCreditoDeDestaque() => "ByteBak Gold Special";

        public string ObterCartaoDeDebitoDeDestaque() => "ByteBak Student";
    }
}
