using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ByteBank.Portal.Infraestrutura
{
    // nao pode ser instanciada apeanas herdada
    public abstract class ControllerBase
    {
        // CallerMemberName implementacao nova do C# 5.0 que server para
        // informar qual foi o chamador.
        protected string View([CallerMemberName]string arquivo = null)
        {
            // recupera o tipo do chamador
            var type = GetType();

            // remove o nome controller do nome do tipo
            var diretorioNome = type.Name.Replace("Controller", "");

            // controi o nome completo atraves totalmente de reflection
            var nomeCompletoResource = $"ByteBank.Portal.View.{diretorioNome}.{arquivo}.html";
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream(nomeCompletoResource);

            var streamLeitura = new StreamReader(resourceStream);
            var textoPagina = streamLeitura.ReadToEnd();
            return textoPagina;
        }

        protected string View(object modelo, [CallerMemberName]string arquivo = null)
        {
            var viewBruta = View(arquivo);
            var todasPropriedadesDoModelo = modelo.GetType().GetProperties();

            var regex = new Regex("\\{{(.*?)\\}}");

            // dellegate que recebe por parametro um math e retorna uma string.
            var viewProcessada = regex.Replace(viewBruta, (match) =>
            {
                var nomePropriedade = match.Groups[1].Value;
                var propriedade = todasPropriedadesDoModelo.Single(prop => prop.Name == nomePropriedade);
                var valorbruto = propriedade.GetValue(modelo);
                return valorbruto.ToString();
            });

            return viewProcessada;
        }
    }
}
