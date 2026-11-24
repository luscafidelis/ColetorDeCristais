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

        [Header("Carne estragada (pode cair a cada dano)")]
        [Tooltip("Chance, de 0 a 1, de uma batida que não derrota derrubar uma carne estragada (0,33 = 33%).")]
        [Range(0f, 1f)]
        [SerializeField] private float chanceDeCarneEstragada = 0.33f;

        [Tooltip("Prefab da carne estragada (sem prefab, nada cai).")]
        [SerializeField] private GameObject prefabDaCarneEstragada;

        [Tooltip("Altura, em metros, em que a carne estragada fica no chão (a mesma dos outros itens).")]
        [SerializeField] private float alturaDaCarneEstragada = 0.8f;

        [Tooltip("Duração, em segundos, da queda da carne estragada.")]
        [Min(0f)]
        [SerializeField] private float duracaoDaQuedaDaCarneEstragada = 0.4f;

        [Tooltip("Quanto a carne estragada sobe, em metros, antes de cair.")]
        [Min(0f)]
        [SerializeField] private float alturaDoArcoDaCarneEstragada = 0.6f;

        /// <summary>Tamanho no começo da partida (multiplicador da escala do modelo).</summary>
        public float TamanhoInicial => tamanhoInicial;

        /// <summary>Quantas vezes por segundo o personagem pisca durante a invulnerabilidade.</summary>
        public float PiscadasPorSegundo => piscadasPorSegundo;

        /// <summary>Chance de uma batida que não derrota derrubar uma carne estragada (0 a 1).</summary>
        public float ChanceDeCarneEstragada => chanceDeCarneEstragada;

        /// <summary>Prefab da carne estragada (nulo: nada cai).</summary>
        public GameObject PrefabDaCarneEstragada => prefabDaCarneEstragada;

        /// <summary>Altura da carne estragada no chão.</summary>
        public float AlturaDaCarneEstragada => alturaDaCarneEstragada;

        /// <summary>Duração da queda da carne estragada, em segundos.</summary>
        public float DuracaoDaQuedaDaCarneEstragada => duracaoDaQuedaDaCarneEstragada;

        /// <summary>Quanto a carne estragada sobe antes de cair.</summary>
        public float AlturaDoArcoDaCarneEstragada => alturaDoArcoDaCarneEstragada;

        /// <summary>Valores do dano.</summary>
        /// <exception cref="System.ArgumentException">Se algum valor for inválido.</exception>
        public ParametrosDeDano ParametrosDeDano =>
            new ParametrosDeDano(danoDaBatida, tamanhoMinimo, distanciaDoTranco, duracaoDoTranco, tempoDeInvulnerabilidade);
    }
}
