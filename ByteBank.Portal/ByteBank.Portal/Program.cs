using ByteBank.Portal.Infraestrutura;

namespace ByteBank.Portal
{
    class Program
    {
        static void Main(string[] args)
        {
            var prefixos = new string[] {"http://localhost:5341/"};
            var webApplication = new WebApplication(prefixos);

            webApplication.Iniciar();
        }
    }
}
