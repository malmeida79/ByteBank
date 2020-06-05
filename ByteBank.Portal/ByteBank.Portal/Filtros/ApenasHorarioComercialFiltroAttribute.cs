using ByteBank.Portal.Infraestrutura.Filters;
using System;

namespace ByteBank.Portal.Filtros
{
    public class ApenasHorarioComercialFiltroAttribute : FiltroAttribute
    {
        public override bool PodeContinuar()
        {
            var hora = DateTime.Now.Hour;
            return hora >= 9 && hora < 16;
        }
    }
}
