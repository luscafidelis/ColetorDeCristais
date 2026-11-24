using System.Collections.Generic;
using UnityEngine;

namespace ColetorDeCristais.Nivel
{
    /// <summary>
    /// Valores de um nível (o vão, os carrinhos, o segredo das prateleiras e onde não entram itens novos),
    /// guardados como asset (ScriptableObject) e ajustáveis no Inspector.
    /// Crie pelo menu <i>Assets &gt; Create &gt; Coletor de Cristais &gt; Definição de Nível</i>.
    /// </summary>
    [CreateAssetMenu(fileName = "Nivel", menuName = "Coletor de Cristais/Definição de Nível")]
    public class DefinicaoNivel : ScriptableObject
    {
        [Header("Vão")]
        [Tooltip("Áreas do chão onde o personagem cai se não estiver numa investida (fim de jogo).")]
        [SerializeField] private List<Retangulo> vaos = new List<Retangulo>();

        [Tooltip("Aceleração da descida mostrada depois da queda, em m/s² (só o desenho).")]
        [Min(0.1f)]
        [SerializeField] private float gravidadeDaQueda = 20f;

        [Tooltip("Quanto o personagem desce depois da queda, em metros (só o desenho).")]
        [Min(0f)]
        [SerializeField] private float profundidadeDaQueda = 5f;

        [Header("Itens novos de cada onda")]
        [Tooltip("Áreas onde nenhum item novo é sorteado, além dos vãos (ex.: prateleiras, área bônus). Desenhe com folga.")]
        [SerializeField] private List<Retangulo> areasSemItensNovos = new List<Retangulo>();

        [Tooltip("Áreas cujos itens não contam para o fim antecipado da onda (ex.: a área bônus). Só valem com " +
                 "'Encerrar Ao Coletar Tudo' ligado na DefinicaoPartida.")]
        [SerializeField] private List<Retangulo> areasForaDoFimAntecipado = new List<Retangulo>();

        [Header("Carrinhos")]
        [Tooltip("Velocidade dos carrinhos no vai e vem, em m/s.")]
        [Min(0.1f)]
        [SerializeField] private float velocidadeDosCarrinhos = 2f;

        [Header("Segredo das prateleiras")]
        [Tooltip("Produtos que podem cair de uma prateleira atingida pela investida; um deles é sorteado.")]
        [SerializeField] private List<GameObject> produtos = new List<GameObject>();

        [Tooltip("Distância da face da prateleira até onde o produto cai, em metros.")]
        [Min(0f)]
        [SerializeField] private float afastamentoDoProduto = 0.8f;

        [Tooltip("Deslocamento do produto ao longo da face, para um lado sorteado, em metros (longe do personagem).")]
        [Min(0f)]
        [SerializeField] private float deslocamentoLateralDoProduto = 1.5f;

        [Tooltip("Altura, em metros, em que o produto aparece.")]
        [SerializeField] private float alturaDoProduto = 0.8f;

        [Tooltip("Duração, em segundos, da queda do produto do alto da prateleira até o chão (zero: aparece direto no chão).")]
        [Min(0f)]
        [SerializeField] private float duracaoDaQuedaDoProduto = 0.4f;

        [Tooltip("Quanto o produto sobe, em metros, acima da linha reta no meio da queda.")]
        [Min(0f)]
        [SerializeField] private float alturaDoArcoDoProduto = 0.3f;

        /// <summary>Áreas onde o personagem cai.</summary>
        public IReadOnlyList<Retangulo> Vaos => vaos;

        /// <summary>Aceleração da descida depois da queda.</summary>
        public float GravidadeDaQueda => gravidadeDaQueda;

        /// <summary>Descida máxima depois da queda.</summary>
        public float ProfundidadeDaQueda => profundidadeDaQueda;

        /// <summary>Áreas sem itens novos: os vãos mais as áreas da lista.</summary>
        public IReadOnlyList<Retangulo> ZonasSemItensNovos
        {
            get
            {
                var zonas = new List<Retangulo>(vaos.Count + areasSemItensNovos.Count);
                zonas.AddRange(vaos);
                zonas.AddRange(areasSemItensNovos);
                return zonas;
            }
        }

        /// <summary>Áreas cujos itens não contam para o fim antecipado da onda.</summary>
        public IReadOnlyList<Retangulo> AreasForaDoFimAntecipado => areasForaDoFimAntecipado;

        /// <summary>Velocidade dos carrinhos.</summary>
        public float VelocidadeDosCarrinhos => velocidadeDosCarrinhos;

        /// <summary>Produtos que podem cair das prateleiras.</summary>
        public IReadOnlyList<GameObject> Produtos => produtos;

        /// <summary>Distância da face da prateleira até o produto.</summary>
        public float AfastamentoDoProduto => afastamentoDoProduto;

        /// <summary>Deslocamento do produto ao longo da face.</summary>
        public float DeslocamentoLateralDoProduto => deslocamentoLateralDoProduto;

        /// <summary>Altura do produto.</summary>
        public float AlturaDoProduto => alturaDoProduto;

        /// <summary>Duração da queda do produto, em segundos.</summary>
        public float DuracaoDaQuedaDoProduto => duracaoDaQuedaDoProduto;

        /// <summary>Quanto o produto sobe acima da linha reta no meio da queda.</summary>
        public float AlturaDoArcoDoProduto => alturaDoArcoDoProduto;
    }
}
