using ColetorDeCristais.Movimento;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColetorDeCristais.Habilidades
{
    /// <summary>
    /// Investida do jogador: lê o botão, pede os cálculos à <see cref="Investida"/> e aplica o
    /// deslocamento no <see cref="CharacterController"/>, que trata as colisões (a parede para o avanço).
    /// Começa bloqueada; um item coletável a libera (<see cref="Liberar"/>).
    /// </summary>
    /// <remarks>
    /// Roda antes do <see cref="MovimentoDoJogador"/> (<see cref="DefaultExecutionOrderAttribute"/>) e o
    /// suspende durante o avanço, para os dois não moverem o personagem no mesmo quadro.
    /// A direção é a da entrada de movimento; sem entrada, para onde o personagem está virado.
    /// A velocidade é a de movimento no início (com os bônus) × o multiplicador da <see cref="DefinicaoInvestida"/>.
    /// </remarks>
    [DefaultExecutionOrder(-10)]
    [RequireComponent(typeof(CharacterController), typeof(MovimentoDoJogador))]
    public class InvestidaDoJogador : MonoBehaviour
    {
        [Tooltip("Ação da investida (botão). No projeto: InputSystem_Actions > Player/Investida.")]
        [SerializeField] private InputActionReference acaoInvestida;

        [Tooltip("Valores da investida (asset DefinicaoInvestida).")]
        [SerializeField] private DefinicaoInvestida definicao;

        private CharacterController _controlador;
        private MovimentoDoJogador _movimento;
        private bool _suspendeuMovimento;

        /// <summary>Estado da habilidade (liberada, em andamento, recarga).</summary>
        public Investida Investida { get; } = new Investida();

        /// <summary>Indica se o personagem está no meio de uma investida.</summary>
        public bool EmInvestida => Investida.EmAndamento;

        /// <summary>
        /// Verdadeiro só durante o <c>CharacterController.Move</c> do avanço da investida: as batidas
        /// (<c>OnControllerColliderHit</c>) que acontecem nesse momento são da investida. Inclui o último quadro,
        /// quando <see cref="EmInvestida"/> já ficou falso.
        /// </summary>
        public bool AplicandoInvestida { get; private set; }

        /// <summary>Libera a investida (chamado pelo efeito do item coletável).</summary>
        public void Liberar() => Investida.Liberar();

        private void Awake()
        {
            _controlador = GetComponent<CharacterController>();
            _movimento = GetComponent<MovimentoDoJogador>();
            if (definicao == null)
            {
                Debug.LogError($"{name}: a investida não tem DefinicaoInvestida; ela não vai funcionar.", this);
            }
        }

        private void OnEnable()
        {
            if (acaoInvestida != null)
            {
                acaoInvestida.action.Enable();
            }
            else
            {
                Debug.LogWarning($"{name}: a ação da investida não foi definida no Inspector.", this);
            }
        }

        private void OnDisable()
        {
            if (acaoInvestida != null)
            {
                acaoInvestida.action.Disable();
            }

            // Desligada no meio de uma investida (ex.: jogador travado no fim da onda), ela para ali:
            // sem isso, o resto do avanço aconteceria sozinho quando o componente voltasse.
            Investida.Cancelar();

            if (_movimento != null)
            {
                AjustarSuspensao(false);
            }
        }

        private void Update()
        {
            if (acaoInvestida != null && definicao != null && acaoInvestida.action.WasPressedThisFrame())
            {
                TentarIniciar();
            }

            var deslocamento = Investida.Atualizar(Time.deltaTime);
            if (deslocamento != Vector3.zero)
            {
                AplicandoInvestida = true;
                _controlador.Move(deslocamento);
                AplicandoInvestida = false;
            }

            AjustarSuspensao(Investida.EmAndamento);
        }

        /// <summary>Tenta começar uma investida agora (botão ou chamada direta, ex.: testes pela CLI).</summary>
        /// <returns><c>true</c> se a investida começou.</returns>
        public bool TentarIniciar()
        {
            if (definicao == null || _movimento == null || _movimento.Velocidade == null)
            {
                return false;
            }

            var direcao = Investida.CalcularDirecao(_movimento.LerEntrada(), transform.forward, CalculadoraDeMovimento.EntradaMinima);
            if (!Investida.TentarIniciar(direcao, _movimento.Velocidade.ValorFinal, definicao.Parametros))
            {
                return false;
            }

            transform.rotation = Quaternion.LookRotation(direcao, Vector3.up);
            AjustarSuspensao(true);
            return true;
        }

        // Só escreve em MovimentoDoJogador.Suspenso no início e no fim da investida, para não
        // apagar a cada quadro uma suspensão pedida por outro sistema.
        private void AjustarSuspensao(bool suspender)
        {
            if (suspender == _suspendeuMovimento)
            {
                return;
            }

            _suspendeuMovimento = suspender;
            _movimento.Suspenso = suspender;
        }
    }
}
