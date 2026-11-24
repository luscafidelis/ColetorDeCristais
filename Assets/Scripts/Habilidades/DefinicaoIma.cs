using ColetorDeCristais.Cristais;
using UnityEngine;

namespace ColetorDeCristais.Habilidades
{
    /// <summary>
    /// Valores do ímã, guardados como asset (ScriptableObject) e ajustáveis no Inspector.
    /// Há dois níveis de atração (completa e reduzida); cada <see cref="DefinicaoCristal"/> diz qual usa.
    /// Crie pelo menu <i>Assets &gt; Create &gt; Coletor de Cristais &gt; Definição de Ímã</i>.
    /// </summary>
    [CreateAssetMenu(fileName = "Ima", menuName = "Coletor de Cristais/Definição de Ímã")]
    public class DefinicaoIma : ScriptableObject
    {
        [Header("Atração completa")]
        [Tooltip("Raio, em metros, para itens com atração completa.")]
        [Min(0.01f)]
        [SerializeField] private float raioCompleto = 3f;

        [Tooltip("Velocidade, em m/s, junto do jogador, para itens com atração completa.")]
        [Min(0.01f)]
        [SerializeField] private float velocidadeCompleta = 6f;

        [Header("Atração reduzida")]
        [Tooltip("Raio, em metros, para itens com atração reduzida.")]
        [Min(0.01f)]
        [SerializeField] private float raioReduzido = 1f;

        [Tooltip("Velocidade, em m/s, junto do jogador, para itens com atração reduzida.")]
        [Min(0.01f)]
        [SerializeField] private float velocidadeReduzida = 2f;

        [Header("Aceleração")]
        [Tooltip("Fração da velocidade na borda do raio; ela cresce até a velocidade cheia junto do jogador. 1 = constante.")]
        [Range(0f, 1f)]
        [SerializeField] private float fracaoNaBorda = 0.25f;

        /// <summary>Valores do nível completo.</summary>
        /// <exception cref="System.ArgumentException">Se algum valor do asset for inválido.</exception>
        public ParametrosDeIma ParametrosCompletos => new ParametrosDeIma(raioCompleto, velocidadeCompleta, fracaoNaBorda);

        /// <summary>Valores do nível reduzido.</summary>
        /// <exception cref="System.ArgumentException">Se algum valor do asset for inválido.</exception>
        public ParametrosDeIma ParametrosReduzidos => new ParametrosDeIma(raioReduzido, velocidadeReduzida, fracaoNaBorda);

        /// <summary>Os valores do nível de atração pedido (a escolha é a de <see cref="Ima.TentarObterParametros"/>).</summary>
        /// <param name="atracao">Nível de atração do item.</param>
        /// <param name="parametros">Raio e velocidade do nível; padrão se o retorno for <c>false</c>.</param>
        /// <returns><c>false</c> para <see cref="AtracaoDoIma.Nenhuma"/> (o ímã não puxa).</returns>
        /// <exception cref="System.ArgumentException">Se algum valor do asset for inválido.</exception>
        public bool TentarObterParametros(AtracaoDoIma atracao, out ParametrosDeIma parametros) =>
            Ima.TentarObterParametros(atracao, ParametrosCompletos, ParametrosReduzidos, out parametros);
    }
}
