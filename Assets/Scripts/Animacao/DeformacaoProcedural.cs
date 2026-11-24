using ColetorDeCristais.Habilidades;
using UnityEngine;

namespace ColetorDeCristais.Animacao
{
    /// <summary>
    /// Aplica a animação da <see cref="CalculadoraDeDeformacao"/> num objeto visual:
    /// pulsa parado e, ao andar, estica, achata e se inclina na direção do movimento.
    /// </summary>
    /// <remarks>
    /// Use num filho só visual, com o pivô na base do corpo (nos "pés"), para a deformação
    /// não afundar o modelo no chão. A velocidade é medida pela mudança de posição do objeto
    /// a cada quadro, no referencial do pai (o personagem), então funciona com qualquer
    /// forma de movimento e com modificadores de velocidade. Não mexe em colisão nem em lógica.
    /// Os campos podem ser ajustados no Inspector durante o Play Mode.
    /// </remarks>
    public class DeformacaoProcedural : MonoBehaviour
    {
        [Header("Parado")]
        [Tooltip("Quanto o corpo cresce e encolhe parado (0,04 = ±4%).")]
        [Range(0f, 0.5f)]
        [SerializeField] private float amplitudeDoPulso = 0.04f;

        [Tooltip("Duração de um pulso completo, em segundos.")]
        [Min(0.05f)]
        [SerializeField] private float periodoDoPulso = 1f;

        [Header("Andando")]
        [Tooltip("Quanto o corpo estica e achata na velocidade de referência (0,1 = 10%).")]
        [Range(0f, 0.5f)]
        [SerializeField] private float estiramentoMaximo = 0.1f;

        [Tooltip("Duração de um ciclo de esticar e achatar na velocidade de referência, em segundos.")]
        [Min(0.05f)]
        [SerializeField] private float periodoDoPasso = 0.4f;

        [Tooltip("Inclinação máxima na direção do movimento, em graus.")]
        [Range(0f, 45f)]
        [SerializeField] private float inclinacaoMaxima = 12f;

        [Tooltip("Velocidade (m/s) em que o efeito de andar fica completo. A velocidade base do jogador é 3,5 m/s.")]
        [Min(0.1f)]
        [SerializeField] private float velocidadeDeReferencia = 3.5f;

        [Tooltip("Tempo (s) para a animação acompanhar uma mudança de velocidade.")]
        [Min(0f)]
        [SerializeField] private float tempoDeResposta = 0.1f;

        [Header("Investida")]
        [Tooltip("Quanto o corpo estica para a frente durante a investida (0,3 = 30%), afinando para preservar o volume.")]
        [Range(0f, 0.9f)]
        [SerializeField] private float estiramentoDaInvestida = 0.3f;

        private readonly CalculadoraDeDeformacao _calculadora = new CalculadoraDeDeformacao();
        private Vector3 _escalaOriginal;
        private Quaternion _rotacaoOriginal;
        private Vector3 _posicaoAnterior;
        private InvestidaDoJogador _investida;

        private void Awake()
        {
            _escalaOriginal = transform.localScale;
            _rotacaoOriginal = transform.localRotation;
            _investida = GetComponentInParent<InvestidaDoJogador>(); // opcional: sem ela, não há estiramento de investida
        }

        private void OnEnable()
        {
            _posicaoAnterior = transform.position;
            _calculadora.Reiniciar();
        }

        private void OnDisable()
        {
            transform.localScale = _escalaOriginal;
            transform.localRotation = _rotacaoOriginal;
        }

        private void LateUpdate()
        {
            var deltaTempo = Time.deltaTime;
            var posicao = transform.position;
            var velocidadeMundo = deltaTempo > 0f ? (posicao - _posicaoAnterior) / deltaTempo : Vector3.zero;
            _posicaoAnterior = posicao;

            var referencial = transform.parent != null ? transform.parent : transform;
            var velocidadeLocal3 = Quaternion.Inverse(referencial.rotation) * velocidadeMundo;
            var velocidadeLocal = new Vector2(velocidadeLocal3.x, velocidadeLocal3.z);

            var parametros = new ParametrosDeDeformacao(
                amplitudeDoPulso, periodoDoPulso, estiramentoMaximo, periodoDoPasso,
                inclinacaoMaxima, velocidadeDeReferencia, tempoDeResposta, estiramentoDaInvestida);
            var emInvestida = _investida != null && _investida.EmInvestida;
            var pose = _calculadora.Atualizar(velocidadeLocal, deltaTempo, parametros, emInvestida);

            transform.localScale = Vector3.Scale(_escalaOriginal, pose.Escala);
            transform.localRotation = pose.Rotacao * _rotacaoOriginal;
        }
    }
}
