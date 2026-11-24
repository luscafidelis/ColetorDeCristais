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

        [Tooltip("Abaixo deste tamanho, o personagem é derrotado.")]
        [Min(0f)]
        [SerializeField] private float tamanhoMinimo = 1f / 16f;

        [Header("Dano (batida de um obstáculo, como o carrinho)")]
        [Tooltip("Quanto cada batida tira do tamanho.")]
        [Min(0f)]
        [SerializeField] private float danoDaBatida = 1f / 6f;

        [Tooltip("Distância, em metros, que o personagem é jogado para trás.")]
        [Min(0f)]
        [SerializeField] private float distanciaDoTranco = 3f;

        [Tooltip("Duração do tranco, em segundos.")]
        [Min(0.01f)]
        [SerializeField] private float duracaoDoTranco = 0.25f;

        [Tooltip("Segundos sem receber outro dano depois de uma batida (o personagem pisca).")]
        [Min(0f)]
        [SerializeField] private float tempoDeInvulnerabilidade = 1f;

        [Tooltip("Quantas vezes por segundo o personagem some e volta durante a invulnerabilidade.")]
        [Min(0f)]
        [SerializeField] private float piscadasPorSegundo = 10f;

        /// <summary>Tamanho no começo da partida (multiplicador da escala do modelo).</summary>
        public float TamanhoInicial => tamanhoInicial;

        /// <summary>Quantas vezes por segundo o personagem pisca durante a invulnerabilidade.</summary>
        public float PiscadasPorSegundo => piscadasPorSegundo;

        /// <summary>Valores do dano.</summary>
        /// <exception cref="System.ArgumentException">Se algum valor for inválido.</exception>
        public ParametrosDeDano ParametrosDeDano =>
            new ParametrosDeDano(danoDaBatida, tamanhoMinimo, distanciaDoTranco, duracaoDoTranco, tempoDeInvulnerabilidade);
    }
}
