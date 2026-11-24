using UnityEngine;
using UnityEngine.UI;

namespace ColetorDeCristais.Partida
{
    /// <summary>
    /// Interface mínima da partida (provisória, até o HUD completo): o cronômetro no topo e a mensagem
    /// de cada fase no centro. Cria um Canvas de uGUI em <c>Screen Space - Camera</c> na câmera principal,
    /// então os textos aparecem também nas capturas da câmera.
    /// </summary>
    [RequireComponent(typeof(ControladorDaPartida))]
    public class MensagensDaPartida : MonoBehaviour
    {
        [Tooltip("Tamanho da fonte do cronômetro, em pixels de referência (tela de 1080 de altura).")]
        [SerializeField] private int tamanhoDoCronometro = 48;

        [Tooltip("Tamanho da fonte da mensagem central, em pixels de referência.")]
        [SerializeField] private int tamanhoDaMensagem = 64;

        private ControladorDaPartida _controlador;
        private Text _cronometro;
        private Text _mensagem;

        // O que está na tela agora (-1 / nulo: nada mostrado ainda).
        private int _segundosExibidos = -1;
        private int _ondaDoCronometro = -1;
        private EstadoDaPartida? _estadoDaMensagem;
        private int _ondaDaMensagem = -1;
        private bool _aguardandoNaMensagem;

        private void Awake()
        {
            _controlador = GetComponent<ControladorDaPartida>();
            var fonte = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var objetoDoCanvas = new GameObject("InterfaceDaPartida");
            objetoDoCanvas.transform.SetParent(transform, false);
            var canvas = objetoDoCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1f;
            var escalador = objetoDoCanvas.AddComponent<CanvasScaler>();
            escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            escalador.referenceResolution = new Vector2(1920f, 1080f);
            escalador.matchWidthOrHeight = 1f;

            _cronometro = CriarTexto(objetoDoCanvas.transform, "Cronometro", fonte, tamanhoDoCronometro,
                new Vector2(0.5f, 1f), new Vector2(0f, -60f), TextAnchor.UpperCenter);
            _mensagem = CriarTexto(objetoDoCanvas.transform, "Mensagem", fonte, tamanhoDaMensagem,
                new Vector2(0.5f, 0.5f), Vector2.zero, TextAnchor.MiddleCenter);
        }

        private void LateUpdate()
        {
            var ciclo = _controlador.Ciclo;
            if (ciclo == null)
            {
                return;
            }

            // Os textos só são refeitos quando o que mostram muda (o cronômetro, uma vez por segundo),
            // para não criar strings novas a cada quadro.
            var segundos = CicloDaPartida.SegundosExibidos(ciclo.TempoRestante);
            if (segundos != _segundosExibidos || ciclo.Onda != _ondaDoCronometro)
            {
                _segundosExibidos = segundos;
                _ondaDoCronometro = ciclo.Onda;
                _cronometro.text = $"Onda {ciclo.Onda}/{ciclo.QuantidadeDeOndas}   {CicloDaPartida.FormatarTempo(ciclo.TempoRestante)}";
            }

            var aguardando = _controlador.AguardandoTecla;
            if (ciclo.Estado != _estadoDaMensagem || ciclo.Onda != _ondaDaMensagem || aguardando != _aguardandoNaMensagem)
            {
                _estadoDaMensagem = ciclo.Estado;
                _ondaDaMensagem = ciclo.Onda;
                _aguardandoNaMensagem = aguardando;
                _mensagem.text = TextoDaMensagem(ciclo.Estado, ciclo.Onda, aguardando);
            }
        }

        /// <summary>Texto central de cada fase (vazio durante a onda).</summary>
        public static string TextoDaMensagem(EstadoDaPartida estado, int onda, bool aguardandoTecla)
        {
            string titulo;
            string acao;
            switch (estado)
            {
                case EstadoDaPartida.ChefeDaOnda:
                    titulo = $"Chefe da onda {onda}!";
                    acao = "(em breve) Aperte qualquer tecla para vencê-lo";
                    break;
                case EstadoDaPartida.ChefeDaArena:
                    titulo = "Chefe da arena!";
                    acao = "(em breve) Aperte qualquer tecla para vencê-lo";
                    break;
                case EstadoDaPartida.Vitoria:
                    titulo = "Arena concluída!";
                    acao = "Aperte qualquer tecla para recomeçar";
                    break;
                case EstadoDaPartida.Derrota:
                    titulo = "Derrota";
                    acao = "Aperte qualquer tecla para recomeçar";
                    break;
                default:
                    return string.Empty;
            }

            return aguardandoTecla ? $"{titulo}\n<size=40>{acao}</size>" : titulo;
        }

        private static Text CriarTexto(Transform pai, string nome, Font fonte, int tamanho, Vector2 ancora, Vector2 posicao, TextAnchor alinhamento)
        {
            var objeto = new GameObject(nome);
            objeto.transform.SetParent(pai, false);
            var retangulo = objeto.AddComponent<RectTransform>();
            retangulo.anchorMin = ancora;
            retangulo.anchorMax = ancora;
            retangulo.pivot = ancora;
            retangulo.anchoredPosition = posicao;
            retangulo.sizeDelta = new Vector2(1600f, 400f);

            var texto = objeto.AddComponent<Text>();
            texto.font = fonte;
            texto.fontSize = tamanho;
            texto.fontStyle = FontStyle.Bold;
            texto.alignment = alinhamento;
            texto.color = Color.white;
            texto.supportRichText = true;
            texto.raycastTarget = false;

            var contorno = objeto.AddComponent<Outline>();
            contorno.effectColor = new Color(0f, 0f, 0f, 0.8f);
            contorno.effectDistance = new Vector2(2f, -2f);
            return texto;
        }
    }
}
