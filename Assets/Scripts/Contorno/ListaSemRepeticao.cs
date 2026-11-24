using System.Collections.Generic;

namespace ColetorDeCristais.Contorno
{
    /// <summary>
    /// Lista em C# puro que não aceita nulo nem item repetido. Guarda os objetos com contorno ligados
    /// (<see cref="ContornoDoObjeto.Ativos"/>).
    /// </summary>
    /// <typeparam name="T">Tipo dos itens.</typeparam>
    public class ListaSemRepeticao<T> where T : class
    {
        private readonly List<T> _itens = new List<T>();

        /// <summary>Os itens, na ordem em que entraram.</summary>
        public IReadOnlyList<T> Itens => _itens;

        /// <summary>Adiciona o item, se não for nulo nem já estiver na lista.</summary>
        /// <returns><c>true</c> se entrou.</returns>
        public bool Adicionar(T item)
        {
            if (item == null || _itens.Contains(item))
            {
                return false;
            }

            _itens.Add(item);
            return true;
        }

        /// <summary>Remove o item, se estiver na lista.</summary>
        /// <returns><c>true</c> se saiu.</returns>
        public bool Remover(T item) => item != null && _itens.Remove(item);
    }
}
