using ColetorDeCristais.Habilidades;
using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Efeito de coletável que libera o ímã do jogador (<see cref="ImaDoJogador"/>).
    /// A liberação vale até o fim da partida; coletar de novo não muda nada.
    /// Crie pelo menu <i>Assets &gt; Create &gt; Coletor de Cristais &gt; Efeitos &gt; Liberar Ímã</i>.
    /// </summary>
    [CreateAssetMenu(fileName = "LiberarIma", menuName = "Coletor de Cristais/Efeitos/Liberar Ímã")]
    public class EfeitoLiberarIma : EfeitoDeCristal
    {
        /// <inheritdoc />
        public override void Aplicar(ColetorDoJogador coletor)
        {
            var ima = coletor != null ? coletor.GetComponent<ImaDoJogador>() : null;
            if (ima == null)
            {
                Debug.LogError($"{name}: o coletor não tem ImaDoJogador; o ímã não foi liberado.", this);
                return;
            }

            ima.Liberar();
            Debug.Log($"{name}: ímã liberado.", coletor);
        }
    }
}
