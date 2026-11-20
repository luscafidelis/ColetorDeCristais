#if UNITY_EDITOR
using System.IO;
using ColetorDeCristais.Movimento;
using UnityEngine;

namespace ColetorDeCristais.Ferramentas
{
    /// <summary>
    /// Ferramenta só da Editor (não entra no build): grava quadros PNG da câmera do jogo enquanto
    /// move o personagem por um <see cref="RoteiroDeEntrada"/>. Os quadros servem para montar as
    /// animações do registro de atualizações (ver <c>ATUALIZACOES.md</c>).
    /// </summary>
    /// <remarks>
    /// Fixa <see cref="Time.captureFramerate"/>, então cada quadro avança o mesmo tempo de jogo,
    /// mesmo que a gravação em disco seja lenta. O personagem é movido com a mesma
    /// <see cref="CalculadoraDeMovimento"/> e a mesma velocidade (<see cref="MovimentoDoJogador.Velocidade"/>)
    /// do jogo; só a entrada é simulada. Use em Play Mode, com a cena Arena aberta:
    /// <c>ColetorDeCristais.Ferramentas.GravadorDeQuadros.Iniciar();</c>
    /// </remarks>
    public class GravadorDeQuadros : MonoBehaviour
    {
        /// <summary>Fica verdadeiro quando o roteiro termina e o último quadro é gravado.</summary>
        public static bool Concluido;

        /// <summary>Quantidade de quadros gravados na última gravação.</summary>
        public static int QuadrosGravados;

        [SerializeField] private int largura = 480;
        [SerializeField] private int altura = 270;
        [SerializeField] private int quadrosPorSegundo = 20;
        [Tooltip("Pasta de saída, relativa à raiz do projeto. É apagada e recriada no início.")]
        [SerializeField] private string pastaDeSaida = "Temp/quadros";
        [Tooltip("Velocidade de giro do personagem durante a gravação, em graus por segundo.")]
        [SerializeField] private float velocidadeDeGiro = 720f;

        [SerializeField] private EtapaDoRoteiro[] etapas =
        {
            new EtapaDoRoteiro(Vector2.zero, 0.8f),
            new EtapaDoRoteiro(new Vector2(1f, 1f), 2.6f),
            new EtapaDoRoteiro(Vector2.down, 3.6f),
            new EtapaDoRoteiro(Vector2.zero, 0.6f),
        };

        private RoteiroDeEntrada _roteiro;
        private Transform _jogador;
        private CharacterController _controlador;
        private MovimentoDoJogador _movimento;
        private Camera _camera;
        private RenderTexture _alvo;
        private Texture2D _textura;
        private string _pasta;
        private float _tempo;

        /// <summary>Cria o gravador na cena atual com o roteiro padrão.</summary>
        public static GravadorDeQuadros Iniciar()
        {
            return new GameObject("GravadorDeQuadros").AddComponent<GravadorDeQuadros>();
        }

        private void Start()
        {
            Concluido = false;
            QuadrosGravados = 0;
            _roteiro = new RoteiroDeEntrada(etapas);
            Time.captureFramerate = quadrosPorSegundo;

            var jogador = GameObject.Find("Jogador");
            _jogador = jogador.transform;
            _controlador = jogador.GetComponent<CharacterController>();
            _movimento = jogador.GetComponent<MovimentoDoJogador>();
            _camera = Camera.main;
            _alvo = new RenderTexture(largura, altura, 24);
            _textura = new Texture2D(largura, altura, TextureFormat.RGB24, false);

            _pasta = Path.GetFullPath(Path.Combine(Application.dataPath, "..", pastaDeSaida));
            if (Directory.Exists(_pasta))
            {
                Directory.Delete(_pasta, true);
            }

            Directory.CreateDirectory(_pasta);
        }

        private void Update()
        {
            if (Concluido)
            {
                return;
            }

            var deltaTempo = Time.deltaTime;
            var entrada = _roteiro.EntradaNoTempo(_tempo);
            _tempo += deltaTempo;

            var velocidade = _movimento != null ? _movimento.Velocidade.ValorFinal : 0f;
            _controlador.Move(CalculadoraDeMovimento.CalcularDeslocamento(entrada, velocidade, deltaTempo));
            if (CalculadoraDeMovimento.TentarCalcularAnguloAlvo(entrada, out var anguloAlvo))
            {
                var angulo = CalculadoraDeMovimento.GirarEmDirecao(_jogador.eulerAngles.y, anguloAlvo, velocidadeDeGiro, deltaTempo);
                _jogador.rotation = Quaternion.Euler(0f, angulo, 0f);
            }
        }

        private void LateUpdate()
        {
            if (Concluido)
            {
                return;
            }

            _camera.targetTexture = _alvo;
            _camera.Render();
            _camera.targetTexture = null;

            var ativoAnterior = RenderTexture.active;
            RenderTexture.active = _alvo;
            _textura.ReadPixels(new Rect(0, 0, largura, altura), 0, 0);
            _textura.Apply();
            RenderTexture.active = ativoAnterior;

            File.WriteAllBytes(Path.Combine(_pasta, $"q_{QuadrosGravados:D4}.png"), _textura.EncodeToPNG());
            QuadrosGravados++;

            if (_tempo >= _roteiro.DuracaoTotal)
            {
                Concluido = true;
                Time.captureFramerate = 0;
                Debug.Log($"GravadorDeQuadros: {QuadrosGravados} quadros em {_pasta}.", this);
            }
        }

        private void OnDestroy()
        {
            Time.captureFramerate = 0;
            if (_alvo != null)
            {
                _alvo.Release();
                Destroy(_alvo);
            }

            if (_textura != null)
            {
                Destroy(_textura);
            }
        }
    }
}
#endif
