using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Cristal na cena. Quando um <see cref="ColetorDoJogador"/> entra no trigger, registra a
    /// coleta no jogador e o cristal some na hora (é destruído).
    /// A regra de "só o jogador, só uma vez" fica no <see cref="EstadoDeColeta"/>.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CristalColetavel : MonoBehaviour
    {
        [Tooltip("Tipo deste cristal (asset DefinicaoCristal).")]
        [SerializeField] private DefinicaoCristal definicao;

        private readonly EstadoDeColeta _estado = new EstadoDeColeta();

        /// <summary>Tipo deste cristal.</summary>
        public DefinicaoCristal Definicao => definicao;

        /// <summary>Indica se o cristal já foi coletado.</summary>
        public bool Coletado => _estado.Coletado;

        private void Awake()
        {
            var colisor = GetComponent<Collider>();
            if (!colisor.isTrigger)
            {
                Debug.LogWarning($"{name}: o collider do cristal precisa estar com Is Trigger marcado; ajustado em tempo de execução.", this);
                colisor.isTrigger = true;
            }

            if (definicao == null)
            {
                Debug.LogError($"{name}: o cristal não tem DefinicaoCristal.", this);
            }
        }

        private void OnTriggerEnter(Collider outro)
        {
            var coletor = outro.GetComponentInParent<ColetorDoJogador>();
            if (!_estado.TentarColetar(coletor != null))
            {
                return;
            }

            coletor.Registrar(definicao);
            Destroy(gameObject);
        }
    }
}
