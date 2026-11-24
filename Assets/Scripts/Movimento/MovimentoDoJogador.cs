using ColetorDeCristais.Atributos;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColetorDeCristais.Movimento
{
    /// <summary>
    /// Move o personagem no chão da arena a partir da ação "Move" do Input System.
    /// É só a ponte com a Unity: lê a entrada, pede os cálculos à <see cref="CalculadoraDeMovimento"/>
    /// e aplica o resultado no <see cref="CharacterController"/> e na rotação.
    /// </summary>
    /// <remarks>
    /// A velocidade é um <see cref="Atributo"/> (valor base <see cref="velocidadeBase"/>, mínimo 0),
    /// para que itens possam aplicar modificadores sobre ela. Os modificadores temporários
    /// perdem tempo aqui, uma vez por quadro.
    /// O personagem não pula nem cai: o deslocamento é sempre horizontal (chão plano).
    /// </remarks>
    [RequireComponent(typeof(CharacterController))]
    public class MovimentoDoJogador : MonoBehaviour
    {
        [Tooltip("Ação de movimento (Vector2). No projeto: InputSystem_Actions > Player/Move.")]
        [SerializeField] private InputActionReference acaoMover;

        [Tooltip("Velocidade base em metros por segundo, antes dos modificadores.")]
        [Min(0f)]
        [SerializeField] private float velocidadeBase = 3.5f;

        [Tooltip("Velocidade do giro em direção ao movimento, em graus por segundo.")]
        [Min(0f)]
        [SerializeField] private float velocidadeDeGiro = 720f;

        private CharacterController _controlador;

        /// <summary>Velocidade do personagem em m/s. Aplique modificadores aqui (ex.: itens de velocidade).</summary>
        public Atributo Velocidade { get; private set; }

        /// <summary>Última entrada de movimento lida (x → X do mundo, y → Z), mesmo quando suspenso.</summary>
        public Vector2 EntradaAtual { get; private set; }

        /// <summary>
        /// Quando <c>true</c>, o componente não move nem gira o personagem (ex.: durante a investida, que
        /// faz o próprio deslocamento). A entrada continua sendo lida e os modificadores continuam perdendo tempo.
        /// </summary>
        public bool Suspenso { get; set; }

        private void Awake()
        {
            _controlador = GetComponent<CharacterController>();
            Velocidade = new Atributo(velocidadeBase, valorMinimo: 0f);
        }

        private void OnEnable()
        {
            if (acaoMover != null)
            {
                acaoMover.action.Enable();
            }
            else
            {
                Debug.LogWarning($"{name}: a ação de movimento não foi definida no Inspector.", this);
            }
        }

        private void OnDisable()
        {
            if (acaoMover != null)
            {
                acaoMover.action.Disable();
            }
        }

        private void Update()
        {
            var deltaTempo = Time.deltaTime;
            Velocidade.Atualizar(deltaTempo);

            var entrada = acaoMover != null ? acaoMover.action.ReadValue<Vector2>() : Vector2.zero;
            EntradaAtual = entrada;
            if (Suspenso)
            {
                return;
            }

            var deslocamento = CalculadoraDeMovimento.CalcularDeslocamento(entrada, Velocidade.ValorFinal, deltaTempo);
            _controlador.Move(deslocamento);

            if (CalculadoraDeMovimento.TentarCalcularAnguloAlvo(entrada, out var anguloAlvo))
            {
                var anguloAtual = transform.eulerAngles.y;
                var novoAngulo = CalculadoraDeMovimento.GirarEmDirecao(anguloAtual, anguloAlvo, velocidadeDeGiro, deltaTempo);
                transform.rotation = Quaternion.Euler(0f, novoAngulo, 0f);
            }
        }
    }
}
