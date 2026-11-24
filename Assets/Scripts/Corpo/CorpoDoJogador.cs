using System;
using ColetorDeCristais.Atributos;
using ColetorDeCristais.Partida;
using UnityEngine;

namespace ColetorDeCristais.Corpo
{
    /// <summary>
    /// Corpo do jogador: guarda o tamanho (um <see cref="Atributo"/>), o aplica na escala do personagem mantendo os
    /// pés no chão, e recebe as batidas (dano, tranco e invulnerabilidade, pelo <see cref="DanoNoCorpo"/>).
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>O tamanho multiplica a escala do objeto inteiro: o desenho e o <c>CharacterController</c> crescem juntos,
    /// então a colisão acompanha o que se vê. A velocidade, a investida e o ímã não mudam com o tamanho.</item>
    /// <item>Itens mudam o tamanho com modificadores no <see cref="Tamanho"/> (ex.: a carne, pelo
    /// <see cref="ColetorDeCristais.Cristais.EfeitoDeTamanho"/>).</item>
    /// <item>A altura do centro só é ajustada quando o tamanho muda, para não brigar com quem mexe na altura
    /// (ex.: a descida da queda no vão).</item>
    /// <item>Uma batida (<see cref="ReceberBatida"/>) encolhe, joga o personagem para trás com
    /// <c>CharacterController.Move</c> (as paredes seguram) e o deixa piscando, sem novo dano, por um tempo.
    /// Abaixo do tamanho mínimo, chama <see cref="ControladorDaPartida.DerrotarJogador"/>.</item>
    /// <item>O <see cref="ControladorDaPartida"/> desliga este componente fora das ondas: o tranco para e o
    /// personagem volta a aparecer.</item>
    /// </list>
    /// </remarks>
    public class CorpoDoJogador : MonoBehaviour
    {
        // Menor escala aplicada: com tamanho zero (derrota), a escala zero quebraria o CharacterController.
        private const float EscalaMinima = 0.01f;

        [Tooltip("Valores do corpo (asset DefinicaoCorpo).")]
        [SerializeField] private DefinicaoCorpo definicao;

        [Tooltip("Partida da cena (opcional): recebe a derrota quando o tamanho fica abaixo do mínimo.")]
        [SerializeField] private ControladorDaPartida partida;

        private readonly DanoNoCorpo _dano = new DanoNoCorpo();
        private CharacterController _controlador;
        private Renderer[] _desenhos = Array.Empty<Renderer>();
        private bool _visivel = true;
        private float _alturaNaEscalaUm;
        private float _tamanhoAplicado = float.NaN;

        /// <summary>
        /// Tamanho do personagem, como multiplicador da escala do modelo (mínimo 0). A base vem da
        /// <see cref="DefinicaoCorpo"/>; aplique modificadores aqui.
        /// </summary>
        public Atributo Tamanho { get; } = new Atributo(1f, valorMinimo: 0f);

        /// <summary>Indica se o personagem está invulnerável (piscando) depois de uma batida.</summary>
        public bool Invulneravel => _dano.Invulnerabilidade.Ativa;

        /// <summary>Indica se o personagem está sendo jogado para trás por uma batida.</summary>
        public bool EmTranco => _dano.Tranco.EmAndamento;

        private void Awake()
        {
            _controlador = GetComponent<CharacterController>();
            // Só os desenhos que já aparecem: o pisca-pisca não pode ligar um que estava desligado de propósito.
            _desenhos = Array.FindAll(GetComponentsInChildren<Renderer>(true), desenho => desenho.enabled);
            if (definicao == null)
            {
                Debug.LogError($"{name}: o corpo não tem DefinicaoCorpo; o personagem fica no tamanho 1 e não recebe dano.", this);
            }
            else
            {
                Tamanho.ValorBase = definicao.TamanhoInicial;
            }

            var escala = transform.localScale.y;
            _alturaNaEscalaUm = escala > 0f ? transform.position.y / escala : transform.position.y;
            AplicarTamanho();
        }

        private void OnDisable()
        {
            _dano.Interromper();
            DefinirVisivel(true);
        }

        /// <summary>
        /// Recebe uma batida (ex.: de um carrinho). Invulnerável, não acontece nada. Senão, encolhe, é jogado na
        /// direção dada e fica invulnerável; abaixo do tamanho mínimo, é derrotado.
        /// </summary>
        /// <param name="direcao">Para onde o personagem é jogado, no chão (x = X, y = Z do mundo).</param>
        /// <returns>O que a batida fez.</returns>
        public ResultadoDaBatida ReceberBatida(Vector2 direcao)
        {
            if (!isActiveAndEnabled || definicao == null)
            {
                return ResultadoDaBatida.Ignorada;
            }

            ParametrosDeDano parametros;
            try
            {
                parametros = definicao.ParametrosDeDano;
            }
            catch (ArgumentException erro)
            {
                Debug.LogError($"{name}: valores inválidos em {definicao.name} ({erro.Message}); a batida não causou dano.", this);
                return ResultadoDaBatida.Ignorada;
            }

            var resultado = _dano.ReceberBatida(Tamanho, direcao, parametros);
            if (resultado == ResultadoDaBatida.Ignorada)
            {
                return resultado;
            }

            Debug.Log($"{name}: batida, tamanho {Tamanho.ValorFinal:0.###} ({resultado}).", this);
            if (resultado == ResultadoDaBatida.Derrota && partida != null)
            {
                partida.DerrotarJogador();
            }

            return resultado;
        }

        private void Update()
        {
            var deslocamento = _dano.Atualizar(Time.deltaTime);
            if (deslocamento != Vector2.zero && _controlador != null && _controlador.enabled)
            {
                _controlador.Move(new Vector3(deslocamento.x, 0f, deslocamento.y));
            }

            DefinirVisivel(_dano.Invulnerabilidade.Visivel(definicao != null ? definicao.PiscadasPorSegundo : 0f));
        }

        private void LateUpdate()
        {
            if (!Mathf.Approximately(Tamanho.ValorFinal, _tamanhoAplicado))
            {
                AplicarTamanho();
            }
        }

        private void AplicarTamanho()
        {
            var tamanho = Tamanho.ValorFinal;
            _tamanhoAplicado = tamanho;
            var escala = Mathf.Max(tamanho, EscalaMinima);
            transform.localScale = Vector3.one * escala;
            var posicao = transform.position;
            posicao.y = RegraDeTamanho.AlturaDoCentro(_alturaNaEscalaUm, escala);
            transform.position = posicao;
            // O CharacterController precisa enxergar a escala e a posição novas antes do próximo Move.
            Physics.SyncTransforms();
        }

        private void DefinirVisivel(bool visivel)
        {
            if (visivel == _visivel)
            {
                return;
            }

            _visivel = visivel;
            foreach (var desenho in _desenhos)
            {
                if (desenho != null)
                {
                    desenho.enabled = visivel;
                }
            }
        }
    }
}
