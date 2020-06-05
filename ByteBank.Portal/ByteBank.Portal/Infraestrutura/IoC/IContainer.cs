using System;

namespace ByteBank.Portal.Infraestrutura.IoC
{
    public interface IContainer
    {
        void Registrar(Type tipoOrigem, Type tipoDestino);

        // Criando uma sobrecarga do metodo usando o compilador a nosso favor
        // estou dizendo que quero que o resgitrar receba os dois tipos como 
        // antes mas usando a contraint where TDestino:TOrigem garanto que 
        // destino tenha herança de origem.
        void Registrar<TOrigem, TDestino>() where TDestino : TOrigem;
        Object Recuperar(Type tipoOrigem);
    }
}
