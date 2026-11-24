using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Anima a queda de um item coletável recém-criado (um produto que cai da prateleira, a carne estragada) pela
    /// <see cref="TrajetoriaDeQueda"/>. Durante a queda o item não é coletado nem puxado pelo ímã; no fim, o
    /// componente se remove.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Derrubar"/> logo depois do <c>Instantiate</c>. O componente desliga o <c>Collider</c> e o
    /// <see cref="CristalColetavel"/> do item (que sai da lista <see cref="CristalColetavel.Ativos"/>) e os religa ao
    /// chegar (só ao chegar: se o item for destruído no meio da queda, eles ficam desligados). O giro e a flutuação do visual (<see cref="GiroEFlutuacao"/>, num filho) continuam durante a queda.
    /// <see cref="EmQueda"/> conta os itens caindo agora.
    /// </remarks>
    public class QuedaDoItem : MonoBehaviour
    {
        private Vector3 _inicio;
        private Vector3 _fim;
        private float _alturaDoArco;
        private float _duracao = 0.0001f;
        private float _tempo;
        private Collider _colisor;
        private CristalColetavel _coletavel;
        private bool _contado;
        private bool _terminou;

        /// <summary>Quantos itens estão caindo agora (ainda não podem ser coletados).</summary>
        public static int EmQueda { get; private set; }

        /// <summary>
        /// Começa a queda de um item: ele vai de <paramref name="inicio"/> até <paramref name="fim"/> em arco.
        /// </summary>
        /// <param name="item">O item recém-criado (com <see cref="CristalColetavel"/>).</param>
        /// <param name="inicio">De onde sai (ex.: o alto da prateleira).</param>
        /// <param name="fim">Onde fica depois de cair.</param>
        /// <param name="duracao">Duração da queda, em segundos (zero ou menos: o item vai direto para o fim).</param>
        /// <param name="alturaDoArco">Quanto o arco sobe acima da linha reta no meio da queda.</param>
        /// <returns>O componente da queda, ou nulo se o item foi direto para o fim.</returns>
        public static QuedaDoItem Derrubar(GameObject item, Vector3 inicio, Vector3 fim, float duracao, float alturaDoArco)
        {
            if (item == null)
            {
                return null;
            }

            if (duracao <= 0f)
            {
                item.transform.position = fim;
                return null;
            }

            var queda = item.AddComponent<QuedaDoItem>();
            queda._inicio = inicio;
            queda._fim = fim;
            queda._duracao = duracao;
            queda._alturaDoArco = Mathf.Max(0f, alturaDoArco);
            queda.Comecar();
            return queda;
        }

        private void Comecar()
        {
            _colisor = GetComponent<Collider>();
            _coletavel = GetComponent<CristalColetavel>();
            DefinirColeta(false);
            EmQueda++;
            _contado = true;
            transform.position = TrajetoriaDeQueda.Posicao(_inicio, _fim, _alturaDoArco, _duracao, 0f);
        }

        private void Update()
        {
            _tempo += Time.deltaTime;
            transform.position = TrajetoriaDeQueda.Posicao(_inicio, _fim, _alturaDoArco, _duracao, _tempo);
            if (TrajetoriaDeQueda.Terminou(_duracao, _tempo))
            {
                _terminou = true;
                Destroy(this);
            }
        }

        // Ao terminar, o item volta a poder ser coletado. Se o item inteiro sumir no meio da queda (ex.: a cena
        // recarrega), a coleta não é religada: religar o CristalColetavel de um objeto sendo destruído o colocaria de
        // volta na lista Ativos sem OnDisable para tirá-lo. Nos dois casos, sai da contagem.
        private void OnDestroy()
        {
            if (_terminou)
            {
                DefinirColeta(true);
            }

            if (_contado)
            {
                EmQueda--;
                _contado = false;
            }
        }

        private void DefinirColeta(bool ligada)
        {
            if (_colisor != null)
            {
                _colisor.enabled = ligada;
            }

            if (_coletavel != null)
            {
                _coletavel.enabled = ligada;
            }
        }

        // Zera a contagem ao entrar no Play Mode sem recarregar o domínio (Enter Play Mode Options).
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ZerarContagem() => EmQueda = 0;
    }
}
