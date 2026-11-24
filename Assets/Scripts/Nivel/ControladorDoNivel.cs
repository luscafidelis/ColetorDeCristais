using ColetorDeCristais.Habilidades;
using ColetorDeCristais.Partida;
using UnityEngine;
using Random = System.Random;

namespace ColetorDeCristais.Nivel
{
    /// <summary>
    /// Ponte entre o nível e a cena: confere a queda no vão, mostra a descida e dá aos carrinhos e às
    /// prateleiras os valores e as referências de que eles precisam.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Fica na raiz do nível; <see cref="CarrinhoVaiEVem"/> e <see cref="PrateleiraSecreta"/> o acham no pai.</item>
    /// <item>A queda (<see cref="RegraDeQueda"/>) é conferida no <c>LateUpdate</c>, depois da investida andar no quadro,
    /// e só durante uma onda. Ao cair, registra a derrota na partida e afunda o personagem no vão.</item>
    /// <item>Fora de uma onda, os carrinhos param (<see cref="EmJogo"/>).</item>
    /// </list>
    /// </remarks>
    public class ControladorDoNivel : MonoBehaviour
    {
        [Tooltip("Valores do nível (asset DefinicaoNivel).")]
        [SerializeField] private DefinicaoNivel definicao;

        [Tooltip("O jogador (objeto com CharacterController e InvestidaDoJogador).")]
        [SerializeField] private GameObject jogador;

        [Tooltip("Partida da cena (opcional). Sem ela, os carrinhos nunca param e a queda não encerra nada.")]
        [SerializeField] private ControladorDaPartida partida;

        [Tooltip("Pai dos produtos que caem das prateleiras (o objeto Cristais da cena).")]
        [SerializeField] private Transform grupoDeItens;

        private float _tempoDesdeAQueda;
        private float _alturaAntesDaQueda;

        /// <summary>Valores do nível.</summary>
        public DefinicaoNivel Definicao => definicao;

        /// <summary>Controle de física do jogador (nulo se faltar).</summary>
        public CharacterController ControleDoJogador { get; private set; }

        /// <summary>Investida do jogador (nula se faltar).</summary>
        public InvestidaDoJogador InvestidaDoJogador { get; private set; }

        /// <summary>Pai dos produtos que caem das prateleiras.</summary>
        public Transform GrupoDeItens => grupoDeItens;

        /// <summary>Sorteios do nível (produto e lado em que ele cai).</summary>
        public Random Sorteio { get; } = new Random();

        /// <summary>Indica se o personagem caiu no vão.</summary>
        public bool Caiu { get; private set; }

        /// <summary>Indica se o jogo está correndo (numa onda, ou sem partida na cena).</summary>
        public bool EmJogo => partida == null || partida.Ciclo == null || partida.Ciclo.EmOnda;

        private void Awake()
        {
            if (definicao == null || jogador == null)
            {
                Debug.LogError($"{name}: o nível precisa de DefinicaoNivel e do jogador; ele não vai funcionar.", this);
                enabled = false;
                return;
            }

            ControleDoJogador = jogador.GetComponent<CharacterController>();
            InvestidaDoJogador = jogador.GetComponent<InvestidaDoJogador>();
        }

        private void LateUpdate()
        {
            if (Caiu)
            {
                _tempoDesdeAQueda += Time.deltaTime;
                var posicao = jogador.transform.position;
                posicao.y = _alturaAntesDaQueda - RegraDeQueda.Descida(_tempoDesdeAQueda, definicao.GravidadeDaQueda, definicao.ProfundidadeDaQueda);
                jogador.transform.position = posicao;
                return;
            }

            if (!EmJogo)
            {
                return;
            }

            var noChao = new Vector2(jogador.transform.position.x, jogador.transform.position.z);
            var emInvestida = InvestidaDoJogador != null && InvestidaDoJogador.Investida.EmAndamento;
            if (RegraDeQueda.Caiu(noChao, emInvestida, definicao.Vaos))
            {
                Cair();
            }
        }

        private void Cair()
        {
            Caiu = true;
            _tempoDesdeAQueda = 0f;
            _alturaAntesDaQueda = jogador.transform.position.y;
            Debug.Log($"{name}: o jogador caiu no vão.", this);
            if (partida != null)
            {
                partida.DerrotarJogador();
            }
        }
    }
}
