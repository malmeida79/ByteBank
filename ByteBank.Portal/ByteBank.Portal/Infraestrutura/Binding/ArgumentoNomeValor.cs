using System;

namespace ByteBank.Portal.Infraestrutura.Binding
{
    public class ArgumentoNomeValor
    {
        public string Nome { get; private set; }
        public string Valor { get; private set; }

        public ArgumentoNomeValor(string nome, string valor)
        {
            if (nome == null) {
                throw new ArgumentNullException(nameof(nome));
            }
            if (valor== null)
            {
                throw new ArgumentNullException(nameof(valor));
            }
            this.Nome = nome;
            this.Valor = valor;
        }

    }
}
