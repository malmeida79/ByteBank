using System;

namespace ByteBank.Service.Cambio
{
    public class CambioTesteService : ICambioService
    {
        private readonly Random _rdm = new Random();

        public decimal calcular(string moedaOrigem, string moedaDestino, decimal valor) => valor * (decimal)_rdm.NextDouble();

    }
}
