namespace ByteBank.Service
{
    public interface ICambioService
    {
        decimal calcular(string moedaOrigem,string moedaDestino, decimal valor);
    }
}
