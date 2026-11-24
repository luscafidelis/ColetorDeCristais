using System.Collections.Generic;
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

        private static readonly List<CristalColetavel> _ativos = new List<CristalColetavel>();

        private readonly EstadoDeColeta _estado = new EstadoDeColeta();

        /// <summary>Cristais ativos na cena agora (ex.: para o ímã do jogador procurar quem puxar).</summary>
        public static IReadOnlyList<CristalColetavel> Ativos => _ativos;

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

        private void OnEnable() => _ativos.Add(this);

        private void OnDisable() => _ativos.Remove(this);

        private void OnTriggerEnter(Collider outro)
        {
            var coletor = outro.GetComponentInParent<ColetorDoJogador>();
            // Um coletor desligado (ex.: partida fora de uma onda) não coleta.
            if (!_estado.TentarColetar(coletor != null && coletor.isActiveAndEnabled))
            {
                return;
            }

            coletor.Registrar(definicao);
            Destroy(gameObject);
        }
    }
}
