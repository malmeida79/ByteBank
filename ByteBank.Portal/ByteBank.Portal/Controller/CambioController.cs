using ByteBank.Portal.Infraestrutura;
using ByteBank.Service;

using ByteBank.Portal.Filtros;

namespace ByteBank.Portal.Controller
{
    public class CambioController : ControllerBase
    {
        private ICambioService _cambioService;
        private ICartaoService _cartaoService;

        public CambioController(ICambioService cambioService, ICartaoService cartaoService)
        {
            _cambioService = cambioService;
            _cartaoService = cartaoService;
        }

        [ApenasHorarioComercialFiltro]
        public string MXN()
        {
            var valorFinal = _cambioService.calcular("MXN", "BRL", 1);
            return View(new
            {
                Valor = valorFinal
            });
        }

        [ApenasHorarioComercialFiltro]
        public string USD()
        {
            var valorFinal = _cambioService.calcular("USD", "BRL", 1);
            return View(new
            {
                Valor = valorFinal
            });
        }

        [ApenasHorarioComercialFiltro]
        public string Calculo(string moedaOrigem, string moedaDestino, decimal valor)
        {
            var valorFinal = _cambioService.calcular(moedaOrigem, moedaDestino, valor);
            var cartaoPromocao = _cartaoService.ObterCartaoDeCreditoDeDestaque();

            // tipo anonimo, na sitaxe do C# 3 em diante ele aceita fazer isso ... 
            var modelo = new
            {
                MoedaDestino = moedaDestino,
                MoedaOrigem = moedaOrigem,
                ValorOrigem = valor,
                ValorDestino = valorFinal,
                CartaoPromocao = cartaoPromocao
            };

            return View(modelo);
        }
    }
}
