using ColetorDeCristais.Cristais;
using UnityEngine;

namespace ColetorDeCristais.Habilidades
{
    /// <summary>
    /// Ímã do jogador (habilidade passiva): depois de liberado, puxa a cada quadro os cristais
    /// dentro do raio na direção do jogador. A coleta continua sendo a normal (trigger do cristal).
    /// Começa bloqueado; um item coletável o libera (<see cref="Liberar"/>).
    /// </summary>
    /// <remarks>
    /// Os cálculos ficam no <see cref="Ima"/>. O raio e a velocidade dependem da
    /// <see cref="DefinicaoCristal.AtracaoDoIma"/> de cada cristal e dos valores da <see cref="DefinicaoIma"/>.
    /// Um cristal só é puxado se nada sólido estiver entre ele e o jogador (ex.: uma parede), então ele
    /// nunca atravessa obstáculos.
    /// </remarks>
    public class ImaDoJogador : MonoBehaviour
    {
        [Tooltip("Valores do ímã (asset DefinicaoIma).")]
        [SerializeField] private DefinicaoIma definicao;

        /// <summary>Estado da habilidade (liberada ou não).</summary>
        public Ima Ima { get; } = new Ima();

        /// <summary>Libera o ímã (chamado pelo efeito do item coletável).</summary>
        public void Liberar() => Ima.Liberar();

        private void Awake()
        {
            if (definicao == null)
            {
                Debug.LogError($"{name}: o ímã não tem DefinicaoIma; ele não vai funcionar.", this);
            }
        }

        private void Update()
        {
            if (!Ima.Liberada || definicao == null)
            {
                return;
            }

            var jogador = transform.position;
            var deltaTempo = Time.deltaTime;
            var completa = definicao.ParametrosCompletos;
            var reduzida = definicao.ParametrosReduzidos;
            var cristais = CristalColetavel.Ativos;
            for (var i = 0; i < cristais.Count; i++)
            {
                var cristal = cristais[i];
                var atracao = cristal.Definicao != null ? cristal.Definicao.AtracaoDoIma : (AtracaoDoIma?)null;
                var posicao = cristal.transform.position;
                if (Ima.TentarCalcularPuxao(cristal.Coletado, atracao, completa, reduzida, posicao, jogador, deltaTempo, out var novaPosicao)
                    && CaminhoLivre(posicao, jogador))
                {
                    cristal.transform.position = novaPosicao;
                }
            }
        }

        // Verifica, na altura do cristal, se algum collider sólido (não trigger) que não seja o próprio
        // jogador fica entre o cristal e o jogador.
        private bool CaminhoLivre(Vector3 cristal, Vector3 jogador)
        {
            var alvo = new Vector3(jogador.x, cristal.y, jogador.z);
            if (!Physics.Linecast(cristal, alvo, out var acerto, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                return true;
            }

            return acerto.collider.transform.IsChildOf(transform);
        }
    }
}
