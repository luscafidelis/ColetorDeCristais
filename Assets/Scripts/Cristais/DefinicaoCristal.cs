using System.Collections.Generic;
using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Dados de um tipo de cristal, guardados como asset (ScriptableObject).
    /// Vários cristais na cena podem apontar para a mesma definição: mudar o asset muda todos.
    /// Crie pelo menu <i>Assets &gt; Create &gt; Coletor de Cristais &gt; Definição de Cristal</i>.
    /// </summary>
    [CreateAssetMenu(fileName = "NovoCristal", menuName = "Coletor de Cristais/Definição de Cristal")]
    public class DefinicaoCristal : ScriptableObject
    {
        [Tooltip("Identificador único e estável do tipo de cristal (usado na contagem). Ex.: cristal-base")]
        [SerializeField] private string identificador = "cristal-base";

        [Tooltip("Nome legível do cristal.")]
        [SerializeField] private string nomeExibicao = "Cristal";

        [Tooltip("Efeitos aplicados ao jogador na coleta. Vazio por enquanto: os efeitos ainda não existem.")]
        [SerializeField] private List<EfeitoDeCristal> efeitos = new List<EfeitoDeCristal>();

        /// <summary>Identificador único do tipo de cristal.</summary>
        public string Identificador => identificador;

        /// <summary>Nome legível do cristal.</summary>
        public string NomeExibicao => nomeExibicao;

        /// <summary>Efeitos aplicados ao jogador quando um cristal deste tipo é coletado.</summary>
        public IReadOnlyList<EfeitoDeCristal> Efeitos => efeitos;
    }
}
