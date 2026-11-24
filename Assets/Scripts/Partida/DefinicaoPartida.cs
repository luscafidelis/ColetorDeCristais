using System.Collections.Generic;
using UnityEngine;

namespace ColetorDeCristais.Partida
{
    /// <summary>
    /// Valores da partida, guardados como asset (ScriptableObject) e ajustáveis no Inspector:
    /// cronômetro, ondas e os itens novos de cada onda.
    /// Crie pelo menu <i>Assets &gt; Create &gt; Coletor de Cristais &gt; Definição de Partida</i>.
    /// </summary>
    [CreateAssetMenu(fileName = "Partida", menuName = "Coletor de Cristais/Definição de Partida")]
    public class DefinicaoPartida : ScriptableObject
    {
        [Header("Ondas")]
        [Tooltip("Duração de cada onda, em segundos (cronômetro regressivo).")]
        [Min(1f)]
        [SerializeField] private float duracaoDaOnda = 120f;

        [Tooltip("Ondas por arena. O chefe da última onda é o chefe da arena.")]
        [Min(1)]
        [SerializeField] private int quantidadeDeOndas = 3;

        [Tooltip("Segundos, depois de uma mudança de fase, em que as teclas são ignoradas (evita pular a mensagem sem querer).")]
        [Min(0f)]
        [SerializeField] private float esperaAntesDeContinuar = 0.5f;

        [Header("Itens novos a cada onda (a partir da segunda)")]
        [Tooltip("Cristais de experiência na primeira onda; dobram a cada onda (4, 8, 16...).")]
        [Min(0)]
        [SerializeField] private int cristaisDeExperienciaNaPrimeiraOnda = 4;

        [Tooltip("Prefab do cristal de experiência.")]
        [SerializeField] private GameObject prefabDeExperiencia;

        [Tooltip("Comidas que entram a cada onda (as mesmas da primeira onda). Repita o prefab para mais de uma.")]
        [SerializeField] private List<GameObject> comidasPorOnda = new List<GameObject>();

        [Tooltip("Itens de habilidade; um deles é sorteado a cada onda.")]
        [SerializeField] private List<GameObject> itensDeHabilidade = new List<GameObject>();

        [Header("Posições sorteadas")]
        [Tooltip("Os itens novos ficam entre -limite e +limite em x e z (a arena vai de -10 a 10; 8 deixa 2 m das paredes).")]
        [Min(0.1f)]
        [SerializeField] private float limiteDaArea = 8f;

        [Tooltip("Distância mínima, em metros, de um item novo até o jogador, os outros itens e entre si.")]
        [Min(0f)]
        [SerializeField] private float distanciaMinima = 1.5f;

        [Tooltip("Altura, em metros, em que os itens novos aparecem.")]
        [SerializeField] private float alturaDosItens = 0.8f;

        /// <summary>Duração e quantidade de ondas.</summary>
        /// <exception cref="System.ArgumentException">Se algum valor for inválido.</exception>
        public ParametrosDaPartida Parametros => new ParametrosDaPartida(duracaoDaOnda, quantidadeDeOndas);

        /// <summary>Segundos em que as teclas são ignoradas depois de uma mudança de fase.</summary>
        public float EsperaAntesDeContinuar => esperaAntesDeContinuar;

        /// <summary>Cristais de experiência na primeira onda.</summary>
        public int CristaisDeExperienciaNaPrimeiraOnda => cristaisDeExperienciaNaPrimeiraOnda;

        /// <summary>Prefab do cristal de experiência.</summary>
        public GameObject PrefabDeExperiencia => prefabDeExperiencia;

        /// <summary>Comidas que entram a cada onda.</summary>
        public IReadOnlyList<GameObject> ComidasPorOnda => comidasPorOnda;

        /// <summary>Itens de habilidade sorteados (um por onda).</summary>
        public IReadOnlyList<GameObject> ItensDeHabilidade => itensDeHabilidade;

        /// <summary>Metade do lado da área onde os itens novos aparecem.</summary>
        public float LimiteDaArea => limiteDaArea;

        /// <summary>Distância mínima entre um item novo e o jogador ou os outros itens.</summary>
        public float DistanciaMinima => distanciaMinima;

        /// <summary>Altura dos itens novos.</summary>
        public float AlturaDosItens => alturaDosItens;
    }
}
