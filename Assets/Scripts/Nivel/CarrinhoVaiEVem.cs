using UnityEngine;

namespace ColetorDeCristais.Nivel
{
    /// <summary>
    /// Carrinho (ou outro obstáculo) que vai e volta entre a posição inicial e um ponto, e empurra o personagem.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>A posição vem de <see cref="VaiEVem"/> pelo tempo acumulado, então não depende da taxa de quadros.
    /// A velocidade vem do <see cref="DefinicaoNivel"/> do <see cref="ControladorDoNivel"/> no pai.</item>
    /// <item>Para fora de uma onda (<see cref="ControladorDoNivel.EmJogo"/>).</item>
    /// <item>A cada quadro, calcula o <see cref="Empurrao"/> com a base do <c>BoxCollider</c> na posição nova e o raio
    /// (mais a pele) do <c>CharacterController</c> do jogador, move o jogador com <c>CharacterController.Move</c>
    /// (as paredes e prateleiras seguram o personagem) e só depois anda.</item>
    /// <item>A base é um retângulo alinhado aos eixos: o carrinho não gira.</item>
    /// </list>
    /// </remarks>
    [RequireComponent(typeof(BoxCollider))]
    public class CarrinhoVaiEVem : MonoBehaviour
    {
        [Tooltip("Ponto da volta, em relação à posição inicial (em metros, no mundo).")]
        [SerializeField] private Vector3 pontoDeVolta = new Vector3(0f, 0f, 5f);

        [Tooltip("Adianto do ciclo, de 0 a 1 (0,5 começa no ponto da volta), para os carrinhos não andarem juntos.")]
        [Range(0f, 1f)]
        [SerializeField] private float fase;

        private ControladorDoNivel _nivel;
        private Vector3 _inicio;
        private Vector3 _fim;
        private Vector2 _tamanhoDaBase;
        private Vector2 _deslocamentoDaBase;
        private float _tempo;

        private void Start()
        {
            _nivel = GetComponentInParent<ControladorDoNivel>();
            if (_nivel == null || _nivel.Definicao == null)
            {
                Debug.LogError($"{name}: o carrinho precisa estar dentro de um ControladorDoNivel com definição; ele não vai andar.", this);
                enabled = false;
                return;
            }

            _inicio = transform.position;
            _fim = _inicio + pontoDeVolta;
            var limites = GetComponent<BoxCollider>().bounds;
            _tamanhoDaBase = new Vector2(limites.size.x, limites.size.z);
            _deslocamentoDaBase = new Vector2(limites.center.x - _inicio.x, limites.center.z - _inicio.z);
            transform.position = VaiEVem.Posicao(_inicio, _fim, _nivel.Definicao.VelocidadeDosCarrinhos, 0f, fase);
        }

        private void Update()
        {
            if (!_nivel.EmJogo)
            {
                return;
            }

            _tempo += Time.deltaTime;
            var anterior = transform.position;
            var nova = VaiEVem.Posicao(_inicio, _fim, _nivel.Definicao.VelocidadeDosCarrinhos, _tempo, fase);
            // Empurra antes de andar: se o carrinho entrasse no personagem primeiro, o CharacterController
            // resolveria a sobreposição subindo em cima dele (e, sem gravidade, ficaria lá em cima).
            Empurrar(new Vector2(nova.x - anterior.x, nova.z - anterior.z), new Vector2(nova.x, nova.z));
            transform.position = nova;
        }

        private void Empurrar(Vector2 movimento, Vector2 posicao)
        {
            var controle = _nivel.ControleDoJogador;
            if (controle == null || !controle.enabled)
            {
                return;
            }

            var baseDoCarrinho = new Retangulo(posicao + _deslocamentoDaBase, _tamanhoDaBase);
            var jogador = controle.transform.position;
            // A folga inclui a "pele" do CharacterController, para ele não começar o Move já encostado no carrinho.
            var empurrao = Empurrao.Calcular(baseDoCarrinho, movimento, new Vector2(jogador.x, jogador.z), controle.radius, controle.skinWidth + 0.01f);
            if (empurrao != Vector2.zero)
            {
                controle.Move(new Vector3(empurrao.x, 0f, empurrao.y));
            }
        }
    }
}
