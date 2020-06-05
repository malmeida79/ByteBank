namespace ByteBank.Portal.Infraestrutura.Filters
{
    public class FilterResult
    {
        public bool PodeContinuar { get; private set; }

        public FilterResult(bool podeContinuar)
        {
            PodeContinuar = podeContinuar;
        }
    }
}
