using ColetorDeCristais.Habilidades;
using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Efeito de coletável que libera a investida do jogador (<see cref="InvestidaDoJogador"/>).
    /// A liberação vale até o fim da partida; coletar de novo não muda nada.
    /// Crie pelo menu <i>Assets &gt; Create &gt; Coletor de Cristais &gt; Efeitos &gt; Liberar Investida</i>.
    /// </summary>
    [CreateAssetMenu(fileName = "LiberarInvestida", menuName = "Coletor de Cristais/Efeitos/Liberar Investida")]
    public class EfeitoLiberarInvestida : EfeitoDeCristal
    {
        /// <inheritdoc />
        public override void Aplicar(ColetorDoJogador coletor)
        {
            var investida = coletor != null ? coletor.GetComponent<InvestidaDoJogador>() : null;
            if (investida == null)
            {
                Debug.LogError($"{name}: o coletor não tem InvestidaDoJogador; a investida não foi liberada.", this);
                return;
            }

            investida.Liberar();
            Debug.Log($"{name}: investida liberada.", coletor);
        }
    }
}
