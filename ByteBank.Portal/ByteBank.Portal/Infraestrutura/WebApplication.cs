using ByteBank.Portal.Infraestrutura.IoC;
using ByteBank.Service;
using ByteBank.Service.Cambio;
using ByteBank.Service.Cartao;
using System;
using System.Net;

namespace ByteBank.Portal.Infraestrutura
{
    class WebApplication
    {
        private readonly string[] _prefixos;
        private readonly IContainer _container = new ContainerSimples();

        public WebApplication(string[] prefixos)
        {
            if (prefixos == null)
            {
                throw new ArgumentNullException(nameof(prefixos));
            }
            _prefixos = prefixos;

            Configurar();
        }

        private void Configurar()
        {
            // posso fazer dessa forma sem os tipos generico
            //_container.Registrar(typeof(ICambioService), typeof(CambioTesteService));
            //_container.Registrar(typeof(ICartaoService), typeof(CartaoTesteService));

            //posso usar entao tambem da forma abaixom com o generico
            _container.Registrar<ICambioService, CambioTesteService>();
            _container.Registrar<ICartaoService, CartaoTesteService>();

        }

        public void Iniciar()
        {
            while (true)
            {
                ManipulaRequisicao();
            }
        }

        public void ManipulaRequisicao()
        {
            var httpLisnter = new HttpListener();

            foreach (var prefixo in _prefixos)
            {
                httpLisnter.Prefixes.Add(prefixo);
            }

            // start listener
            httpLisnter.Start();

            // capturando e configurando o cntext
            var contexto = httpLisnter.GetContext();
            var requisicao = contexto.Request;
            var resposta = contexto.Response;

            var path = requisicao.Url.PathAndQuery;

            if (Utilidades.EhArquivo(path))
            {
                var manipulador = new ManipuladorRequisicaoArquivo();
                manipulador.Manipular(resposta, path);
            }
            else
            {
                var manipulador = new ManipuladorRequisicaoController(_container);
                manipulador.Manipular(resposta, path);
            }

            httpLisnter.Stop();
        }
    }
}
