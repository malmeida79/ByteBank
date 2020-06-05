using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ByteBank.Portal.Infraestrutura.Binding
{
    public class ActionBindingInfo
    {
        public MethodInfo metodoInfo { get; private set; }
        public IReadOnlyCollection<ArgumentoNomeValor> TuplasArgumentoNomeValor { get; private set; }

        public ActionBindingInfo(MethodInfo methodInfo, IEnumerable<ArgumentoNomeValor> tuplasArgumentoNomeValor)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            this.metodoInfo = methodInfo;

            if (tuplasArgumentoNomeValor == null)
            {
                throw new ArgumentNullException(nameof(tuplasArgumentoNomeValor));
            }

            this.TuplasArgumentoNomeValor = new ReadOnlyCollection<ArgumentoNomeValor>(tuplasArgumentoNomeValor.ToList());
        }

        public object Invoke(object controller)
        {

            var countParametros = TuplasArgumentoNomeValor.Count;
            var possuiArgumento = countParametros > 0;
            var parametrosMethodInfo = metodoInfo.GetParameters();

            if (!possuiArgumento)
            {
                return metodoInfo.Invoke(controller, new object[0]);
            }
            else
            {

                var parametrosInvoke = new object[countParametros];

                // é necessario ordenar seus parametros para podermos passar e usar adiante
                for (int i = 0; i < countParametros; i++)
                {
                    // pegando parametro da posicao "I"
                    var parametro = parametrosMethodInfo[i];

                    // recuperando seu nome para buscar seu valor usando o nome como
                    // chave
                    var parametroNome = parametro.Name;

                    // recuperando o valor do parametro passado como nome
                    var argumento = TuplasArgumentoNomeValor.Single(tupla => tupla.Nome == parametroNome);

                    // adicionando o valor recuperado a listagem de valores de parametros
                    parametrosInvoke[i] = Convert.ChangeType(argumento.Valor,parametro.ParameterType);
                }

                return metodoInfo.Invoke(controller, parametrosInvoke);
            }
        }

    }
}
