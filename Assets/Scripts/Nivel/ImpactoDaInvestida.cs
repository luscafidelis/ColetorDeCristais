using ColetorDeCristais.Habilidades;
using UnityEngine;

namespace ColetorDeCristais.Nivel
{
    /// <summary>
    /// No jogador: avisa a <see cref="PrateleiraSecreta"/> que o personagem bateu nela durante uma investida.
    /// </summary>
    /// <remarks>
    /// Usa o <c>OnControllerColliderHit</c> do <c>CharacterController</c> e só conta as batidas do deslocamento
    /// da investida (<see cref="InvestidaDoJogador.AplicandoInvestida"/>), não as do andar.
    /// </remarks>
    [RequireComponent(typeof(CharacterController), typeof(InvestidaDoJogador))]
    public class ImpactoDaInvestida : MonoBehaviour
    {
        private InvestidaDoJogador _investida;

        private void Awake() => _investida = GetComponent<InvestidaDoJogador>();

        private void OnControllerColliderHit(ControllerColliderHit batida)
        {
            if (!_investida.AplicandoInvestida)
            {
                return;
            }

            var prateleira = batida.collider.GetComponent<PrateleiraSecreta>();
            if (prateleira != null)
            {
                prateleira.ReceberBatida(transform.position, true);
            }
        }
    }
}
