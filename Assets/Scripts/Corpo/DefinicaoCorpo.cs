using UnityEngine;

namespace ColetorDeCristais.Corpo
{
    /// <summary>
    /// Valores do corpo do personagem, guardados como asset (ScriptableObject) e ajustáveis no Inspector.
    /// Crie pelo menu <i>Assets &gt; Create &gt; Coletor de Cristais &gt; Definição de Corpo</i>.
    /// </summary>
    [CreateAssetMenu(fileName = "Corpo", menuName = "Coletor de Cristais/Definição de Corpo")]
    public class DefinicaoCorpo : ScriptableObject
    {
        [Header("Tamanho")]
        [Tooltip("Tamanho no começo da partida, como multiplicador do modelo (1 = o montrengo original, cerca de 2 m).")]
        [Min(0.01f)]
        [SerializeField] private float tamanhoInicial = 1f / 3f;

        /// <summary>Tamanho no começo da partida (multiplicador da escala do modelo).</summary>
        public float TamanhoInicial => tamanhoInicial;
    }
}
