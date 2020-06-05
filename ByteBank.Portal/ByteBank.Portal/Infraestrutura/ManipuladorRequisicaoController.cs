using ByteBank.Portal.Infraestrutura.Binding;
using ByteBank.Portal.Infraestrutura.Filters;
using ByteBank.Portal.Infraestrutura.IoC;
using System;
using System.Net;
using System.Text;

namespace ByteBank.Portal.Infraestrutura
{
    public class ManipuladorRequisicaoController
    {
        private readonly ActionBinder _actionBinder = new ActionBinder();
        private readonly FilterResolver _filterResolver = new FilterResolver();
        private readonly ControllerResolver _controllerResolver;

        public ManipuladorRequisicaoController(IContainer container)
        {
            _controllerResolver = new ControllerResolver(container);
        }

        public void Manipular(HttpListenerResponse resposta, string path)
        {
            // split com remoção de partes vaias.
            var partesPath = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            var controllerNome = partesPath[0];
            var actionNome = partesPath[1];

            var nomeCompleto = $"ByteBank.Portal.Controller{controllerNome}.{actionNome}";

            // criamos uma instancia da classe (Foi comentando quando implementamos injecao de dependecias)
            // pois nao preciramos mais unsar o get instance. 
            //var controllerWrapper = Activator.CreateInstance("ByteBank.Portal", nomeCompleto, new object[0]);

            //// captura e retorna o metodo original que criamos
            //var controller = controllerWrapper.Unwrap();

            var controller = _controllerResolver.ObterController(nomeCompleto);

            // buscando informações dos metodos do nosso controller
            // criamos o novo tipo para poder passar diretamente para os dois casos            
            var actionBindInfo = _actionBinder.ObterActionBinderMethodInfo(controller, path);
            var filterResult = _filterResolver.VerficarFiltros(actionBindInfo);

            if (filterResult.PodeContinuar)
            {
                // executar o metodo e recuperar uma string
                var resultadoAction = (string)actionBindInfo.Invoke(controller);

                var bufferArquivo = Encoding.UTF8.GetBytes(resultadoAction);

                resposta.StatusCode = 200;
                resposta.ContentType = "text/html; charset=utf-8";
                resposta.ContentLength64 = bufferArquivo.Length;
                resposta.OutputStream.Write(bufferArquivo, 0, bufferArquivo.Length);
                resposta.OutputStream.Close();
            }
            else
            {
                resposta.StatusCode = 307;
                resposta.RedirectLocation = "/Erro/Inesperado";
                resposta.OutputStream.Close();
            }
        }
    }
}
