using System.Net;
using System.Reflection;

namespace ByteBank.Portal.Infraestrutura
{
    public class ManipuladorRequisicaoArquivo
    {
        public void Manipular(HttpListenerResponse resposta, string path)
        {

            // retornar o ducmento css
            var assembly = Assembly.GetExecutingAssembly();
            var nomeResource = Utilidades.ConvertPathParaNomeAssembly(path);
            var resourceStream = assembly.GetManifestResourceStream(nomeResource);

            // caso nada tenha sido carregado, entao devolvemos erro (404)
            // erro nesse tipo de programa.
            if (resourceStream == null)
            {
                resposta.StatusCode = 404;
                resposta.OutputStream.Close();
            }
            else
            {
                using (resourceStream)
                {
                    // nesse tipo de protocolo nao conseguimos trafegar dados string
                    // somente streaming (array de bytes) entao vamos converter
                    var bytesResouce = new byte[resourceStream.Length];
                    resourceStream.Read(bytesResouce, 0, (int)resourceStream.Length);

                    // configurando o response
                    resposta.ContentType = Utilidades.ObterTipoDeConteudo(path);
                    resposta.StatusCode = 200;
                    resposta.ContentLength64 = resourceStream.Length;
                    resposta.OutputStream.Write(bytesResouce, 0, bytesResouce.Length);
                    resposta.OutputStream.Close();
                }
            }
        }
    }
}
