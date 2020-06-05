using System;

namespace ByteBank.Portal.Infraestrutura.Filters
{
    public abstract class FiltroAttribute: Attribute
    {
        public abstract bool PodeContinuar();
    }
}
