using ByteBank.Portal.Infraestrutura.Binding;

namespace ByteBank.Portal.Infraestrutura.Filters
{
    public class FilterResolver
    {
        public FilterResult VerficarFiltros(ActionBindingInfo actionBindInfo)
        {
            var methodInfo = actionBindInfo.metodoInfo;

            // setamos inherit para false pois nao queremos que suba a arvore de 
            // heranças.
            var atributos = methodInfo.GetCustomAttributes(typeof(FiltroAttribute), false);

            foreach (FiltroAttribute filtro in atributos)
            {
                if (!filtro.PodeContinuar())
                {
                    return new FilterResult(false);
                }
            }

            return new FilterResult(true);
        }
    }
}
