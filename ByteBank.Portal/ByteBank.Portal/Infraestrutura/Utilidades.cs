using System;
using System.Linq;

namespace ByteBank.Portal.Infraestrutura
{
    class Utilidades
    {
        public static bool EhArquivo(string path)
        {
            // split com remoção de partes vaias.
            var partesPath = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // buscando ultima parte
            var ultimaParte = partesPath.Last();

            return ultimaParte.Contains('.');
        }

        public static string ConvertPathParaNomeAssembly(string path)
        {
            var prefixoAssembly = "ByteBank.Portal";
            var pathComPontos = path.Replace('/', '.');
            var nomeCompleto = $"{prefixoAssembly}{pathComPontos}";
            return nomeCompleto;
        }

        public static string ObterTipoDeConteudo(string path)
        {

            if (path.EndsWith(".css"))
            {
                return "text/css; charset=utf-8"; ;
            }

            if (path.EndsWith(".js"))
            {
                return "application/js; charset=utf-8";
            }

            if (path.EndsWith(".html"))
            {
                return "text/html; charset=utf-8"; ;
            }

            throw new NotImplementedException("Tipo de conteudo não previsto");
        }
    }
}
