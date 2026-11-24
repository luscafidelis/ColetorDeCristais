using UnityEngine;

namespace ColetorDeCristais.Habilidades
{
    /// <summary>
    /// Valores da investida, guardados como asset (ScriptableObject) e ajustáveis no Inspector.
    /// Crie pelo menu <i>Assets &gt; Create &gt; Coletor de Cristais &gt; Definição de Investida</i>.
    /// </summary>
    [CreateAssetMenu(fileName = "Investida", menuName = "Coletor de Cristais/Definição de Investida")]
    public class DefinicaoInvestida : ScriptableObject
    {
        [Tooltip("Velocidade da investida = velocidade de movimento atual × este valor. Bônus de velocidade aumentam a investida.")]
        [Min(0.01f)]
        [SerializeField] private float multiplicadorDeVelocidade = 5f;

        [Tooltip("Duração da investida, em segundos.")]
        [Min(0.01f)]
        [SerializeField] private float duracao = 0.25f;

        [Tooltip("Tempo até poder usar de novo, em segundos, contado a partir do início da investida.")]
        [Min(0f)]
        [SerializeField] private float recarga = 5f;

        /// <summary>Os valores do asset.</summary>
        /// <exception cref="System.ArgumentException">Se algum valor for inválido.</exception>
        public ParametrosDeInvestida Parametros => new ParametrosDeInvestida(multiplicadorDeVelocidade, duracao, recarga);
    }
}
