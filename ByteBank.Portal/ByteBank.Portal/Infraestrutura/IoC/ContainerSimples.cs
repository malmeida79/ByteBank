using System;
using System.Collections.Generic;
using System.Linq;

namespace ByteBank.Portal.Infraestrutura.IoC
{
    public class ContainerSimples : IContainer
    {
        private readonly Dictionary<Type, Type> _mapaDeTipos = new Dictionary<Type, Type>();

        // registrar (typeof(ICambioServie)), registrar (typeof(CambioTesteService))
        // registrar(typeof(ICambioServie))
        // retorna uma instancia do tipo CambioTesteService
        public void Registrar(Type tipoOrigem, Type tipoDestino)
        {
            if (_mapaDeTipos.ContainsKey(tipoOrigem))
            {
                throw new InvalidOperationException("Tipo já mapeado!");
            }

            VerificarHieraquiaOuLacarExcecao(tipoOrigem, tipoDestino);

            _mapaDeTipos.Add(tipoOrigem, tipoDestino);

        }

        public void Registrar<TOrigem, TDestino>() where TDestino : TOrigem
        {
            if (_mapaDeTipos.ContainsKey(typeof(TOrigem)))
            {
                throw new InvalidOperationException("Tipo já mapeado!");
            }

            VerificarHieraquiaOuLacarExcecao(typeof(TOrigem), typeof(TDestino));

            _mapaDeTipos.Add(typeof(TOrigem), typeof(TDestino));
        }

        private void VerificarHieraquiaOuLacarExcecao(Type tipoOrigem, Type tipoDestino)
        {
            if (tipoDestino.IsInterface)
            {
                var tipoDestinoImplementaInterface = tipoDestino.GetInterfaces().Any(tipoInterface => tipoInterface == tipoOrigem);
                if (!tipoDestinoImplementaInterface)
                {
                    throw new InvalidOperationException("O Tipo destino não implementa a interce!");
                }
            }
            else
            {
                var tipoDestinoHerdaTipoOrigem = tipoDestino.IsSubclassOf(tipoOrigem);
                if (!tipoDestinoHerdaTipoOrigem)
                {
                    throw new InvalidOperationException("O Tipo destino não herda o tipo de origem!");
                }
            }
        }

        public Object Recuperar(Type tipoOrigem)
        {
            var tipoOrigemMapeado = _mapaDeTipos.ContainsKey(tipoOrigem);
            if (tipoOrigemMapeado)
            {
                var tipoDestino = _mapaDeTipos[tipoOrigem];
                return Recuperar(tipoDestino);
            }

            // buscamos entao os construtores
            var construtores = tipoOrigem.GetConstructors();

            // dos construtores, buscamos o que nao tem parametros por ser o mais facil de implementar sempre;
            var construtorSemParametros = construtores.FirstOrDefault(cons => cons.GetParameters().Any() == false);
            if (construtorSemParametros != null)
            {
                var instanciaSemParametros = construtorSemParametros.Invoke(new object[0]);
                return instanciaSemParametros;
            }

            // caso tenha parametros no construtor entao sera a partir daqui que o campo seguira.
            var valoresParametros = new object();

            var construtorQueVamosUsar = construtores[0];
            var parametrosDoConstrutor = construtorQueVamosUsar.GetParameters();
            var valoresDeParametros = new object[parametrosDoConstrutor.Count()];

            for (int i = 0; i < parametrosDoConstrutor.Length; i++)
            {
                var parametro = parametrosDoConstrutor[i];
                var tipoParametro = parametro.ParameterType;
                valoresDeParametros[i] = Recuperar(tipoParametro);
            }

            var instanciaComParametros = construtorQueVamosUsar.Invoke(valoresDeParametros);
            return instanciaComParametros;
        }


    }
}
