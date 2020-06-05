using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ByteBank.Portal.Infraestrutura.Binding
{
    class ActionBinder
    {
        public ActionBindingInfo ObterActionBinderMethodInfo(object controller, string path)
        {
            var idxInterrogacao = path.IndexOf('?');
            var existeInterregacao = idxInterrogacao >= 0;

            if (!existeInterregacao)
            {
                var nomeAction = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1];
                var metodoInfo = controller.GetType().GetMethod(nomeAction);
                return new ActionBindingInfo(metodoInfo, Enumerable.Empty<ArgumentoNomeValor>());
            }
            else
            {
                var nomeControllerComAction = path.Substring(0, idxInterrogacao);
                var nomeAction = nomeControllerComAction.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1];
                var queryString = path.Substring(idxInterrogacao + 1, path.Length);
                var tuplasNomeValor = ObterAgrumentoNomeValores(queryString);
                var nomeArgumentos = tuplasNomeValor.Select(tupla => tupla.Nome).ToArray();
                var metodoInfo = ObterMethodInfoAPartirDeNomesEArgumentos(nomeAction, nomeArgumentos, controller);
                return new ActionBindingInfo(metodoInfo,tuplasNomeValor);
            }
        }

        private IEnumerable<ArgumentoNomeValor> ObterAgrumentoNomeValores(string queryString)
        {
            var tuplasNomeValor = queryString.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var tupla in tuplasNomeValor)
            {
                var partesTupla = tupla.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                yield return new ArgumentoNomeValor(partesTupla[0], partesTupla[1]);
            }

        }

        private MethodInfo ObterMethodInfoAPartirDeNomesEArgumentos(string nomeAction, string[] argumentos, object controller)
        {

            var argumentosCount = argumentos.Length;

            var bindingFlags = BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.DeclaredOnly;

            var metodos = controller.GetType().GetMethods(bindingFlags);
            var sobreCargas = metodos.Where(metodo => metodo.Name == nomeAction);

            foreach (var sobreCarga in sobreCargas)
            {
                var parametros = sobreCarga.GetParameters();
                if (argumentosCount != parametros.Length)
                {
                    continue;
                }

                var match = parametros.All(parametro => argumentos.Contains(parametro.Name));

                if (match)
                {
                    return sobreCarga;
                }
            }

            throw new ArgumentException($"A sobrecarga do método {nomeAction} não foi encontrada!");
        }
    }
}
