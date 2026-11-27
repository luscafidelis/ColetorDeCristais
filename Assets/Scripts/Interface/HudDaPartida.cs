using ColetorDeCristais.Habilidades;
using ColetorDeCristais.Partida;
using ColetorDeCristais.Tema;
using UnityEngine;

namespace ColetorDeCristais.Interface
{
    /// <summary>
    /// HUD da partida: onda e tempo no topo, itens que faltam no canto superior esquerdo (só com fim antecipado),
    /// as habilidades no canto inferior esquerdo e a mensagem de cada fase no centro. Cria um Canvas de uGUI em
    /// <c>Screen Space - Camera</c> na câmera principal, então a HUD aparece também nas capturas da câmera.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Os ícones das habilidades (Bote, a investida, e Faro, o ímã) ficam apagados até a habilidade ser obtida; o da investida
    /// escurece e vai enchendo de baixo para cima durante a recarga (<see cref="IndicadorDeHabilidade"/>).</item>
    /// <item>Os textos só são refeitos quando o que mostram muda, para não criar strings novas a cada quadro.</item>
    /// <item>Sem jogador na <see cref="ControladorDaPartida"/>, ou sem os componentes das habilidades, os ícones não
    /// aparecem.</item>
    /// </list>
    /// </remarks>
    [RequireComponent(typeof(ControladorDaPartida))]
    public class HudDaPartida : MonoBehaviour
    {
        private static readonly Color CorBloqueada = new Color(0.45f, 0.45f, 0.45f, 0.5f);
        private static readonly Color CorDoFundo = new Color(0f, 0f, 0f, 0.55f);
        private static readonly Color CorDaRecarga = new Color(0f, 0f, 0f, 0.6f);

        [Tooltip("Tamanho da fonte do cronômetro, em pixels de referência (tela de 1080 de altura).")]
        [SerializeField] private int tamanhoDoCronometro = 48;

        [Tooltip("Tamanho da fonte da mensagem central, em pixels de referência.")]
        [SerializeField] private int tamanhoDaMensagem = 64;

        [Tooltip("Tamanho da fonte do contador de itens, em pixels de referência.")]
        [SerializeField] private int tamanhoDosItens = 48;

        [Tooltip("Lado de cada ícone de habilidade, em pixels de referência.")]
        [SerializeField] private float tamanhoDoIcone = 120f;

        [Tooltip("Cor do ícone do Bote (a investida). Padrão: a carne da paleta do tema.")]
        [SerializeField] private Color corDaInvestida = PaletaDoTema.Carne;

        [Tooltip("Cor do ícone do Faro (o ímã). Padrão: a gosma da paleta do tema.")]
        [SerializeField] private Color corDoIma = PaletaDoTema.Gosma;

        private ControladorDaPartida _controlador;
        private UnityEngine.UI.Text _cronometro;
        private UnityEngine.UI.Text _mensagem;
        private UnityEngine.UI.Text _itens;
        private InvestidaDoJogador _investida;
        private ImaDoJogador _ima;
        private IconeDeHabilidade _iconeDaInvestida;
        private IconeDeHabilidade _iconeDoIma;

        // O que está na tela agora (-1 / nulo: nada mostrado ainda).
        private int _segundosExibidos = -1;
        private int _ondaDoCronometro = -1;
        private EstadoDaPartida? _estadoDaMensagem;
        private int _ondaDaMensagem = -1;
        private bool _aguardandoNaMensagem;
        private int _itensExibidos = int.MinValue;

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
            var escalador = objetoDoCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            escalador.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            escalador.referenceResolution = new Vector2(1920f, 1080f);
            escalador.matchWidthOrHeight = 1f;
            var raiz = objetoDoCanvas.transform;

            _cronometro = CriarTexto(raiz, "Cronometro", fonte, tamanhoDoCronometro,
                new Vector2(0.5f, 1f), new Vector2(0f, -60f), new Vector2(1600f, 400f), TextAnchor.UpperCenter);
            _mensagem = CriarTexto(raiz, "Mensagem", fonte, tamanhoDaMensagem,
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1600f, 400f), TextAnchor.MiddleCenter);
            _itens = CriarTexto(raiz, "Itens", fonte, tamanhoDosItens,
                new Vector2(0f, 1f), new Vector2(40f, -60f), new Vector2(600f, 100f), TextAnchor.UpperLeft);
            _itens.gameObject.SetActive(false);

            var jogador = _controlador.Jogador;
            if (jogador != null)
            {
                _investida = jogador.GetComponent<InvestidaDoJogador>();
                _ima = jogador.GetComponent<ImaDoJogador>();
            }

            var espaco = tamanhoDoIcone + 20f;
            if (_investida != null)
            {
                _iconeDaInvestida = new IconeDeHabilidade(raiz, "Icone_Investida", fonte, "»", "Bote", corDaInvestida,
                    tamanhoDoIcone, new Vector2(40f, 40f));
            }

            if (_ima != null)
            {
                _iconeDoIma = new IconeDeHabilidade(raiz, "Icone_Ima", fonte, "U", "Faro", corDoIma,
                    tamanhoDoIcone, new Vector2(40f + espaco, 40f));
            }
        }

        private void LateUpdate()
        {
            var ciclo = _controlador.Ciclo;
            if (ciclo == null)
            {
                return;
            }

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

            var itens = _controlador.ItensQueFaltam;
            if (itens != _itensExibidos)
            {
                _itensExibidos = itens;
                _itens.gameObject.SetActive(itens >= 0);
                _itens.text = TextoDosItens(itens);
            }

            if (_iconeDaInvestida != null)
            {
                var investida = _investida.Investida;
                _iconeDaInvestida.Mostrar(IndicadorDeHabilidade.Calcular(investida.Liberada, investida.RecargaRestante, investida.RecargaTotal));
            }

            if (_iconeDoIma != null)
            {
                _iconeDoIma.Mostrar(IndicadorDeHabilidade.Calcular(_ima.Ima.Liberada, 0f, 0f));
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

        /// <summary>Texto do contador de itens que faltam (vazio se a partida não tem fim antecipado, valor negativo).</summary>
        public static string TextoDosItens(int itensQueFaltam) => itensQueFaltam < 0 ? string.Empty : $"Comida: {itensQueFaltam}";

        private static UnityEngine.UI.Text CriarTexto(Transform pai, string nome, Font fonte, int tamanho, Vector2 ancora,
            Vector2 posicao, Vector2 dimensao, TextAnchor alinhamento)
        {
            var retangulo = CriarRetangulo(pai, nome);
            retangulo.anchorMin = ancora;
            retangulo.anchorMax = ancora;
            retangulo.pivot = ancora;
            retangulo.anchoredPosition = posicao;
            retangulo.sizeDelta = dimensao;
            return ConfigurarTexto(retangulo.gameObject, fonte, tamanho, alinhamento);
        }

        private static UnityEngine.UI.Text ConfigurarTexto(GameObject objeto, Font fonte, int tamanho, TextAnchor alinhamento)
        {
            var texto = objeto.AddComponent<UnityEngine.UI.Text>();
            texto.font = fonte;
            texto.fontSize = tamanho;
            texto.fontStyle = FontStyle.Bold;
            texto.alignment = alinhamento;
            texto.color = Color.white;
            texto.supportRichText = true;
            texto.raycastTarget = false;
            texto.horizontalOverflow = HorizontalWrapMode.Overflow;

            var contorno = objeto.AddComponent<UnityEngine.UI.Outline>();
            contorno.effectColor = new Color(0f, 0f, 0f, 0.8f);
            contorno.effectDistance = new Vector2(2f, -2f);
            return texto;
        }

        private static RectTransform CriarRetangulo(Transform pai, string nome)
        {
            var objeto = new GameObject(nome);
            objeto.transform.SetParent(pai, false);
            return objeto.AddComponent<RectTransform>();
        }

        private static RectTransform Esticar(RectTransform retangulo, float margem)
        {
            retangulo.anchorMin = Vector2.zero;
            retangulo.anchorMax = Vector2.one;
            retangulo.offsetMin = new Vector2(margem, margem);
            retangulo.offsetMax = new Vector2(-margem, -margem);
            return retangulo;
        }

        private static UnityEngine.UI.Image CriarImagem(Transform pai, string nome, Color cor, float margem)
        {
            var retangulo = Esticar(CriarRetangulo(pai, nome), margem);
            var imagem = retangulo.gameObject.AddComponent<UnityEngine.UI.Image>();
            imagem.color = cor;
            imagem.raycastTarget = false;
            return imagem;
        }

        // Um ícone de habilidade: fundo escuro, quadrado colorido, véu escuro da recarga, símbolo e nome.
        private sealed class IconeDeHabilidade
        {
            private readonly UnityEngine.UI.Image _cor;
            private readonly RectTransform _veu;
            private readonly UnityEngine.UI.Text _simbolo;
            private readonly UnityEngine.UI.Text _nome;
            private readonly Color _corObtida;
            private EstadoDaHabilidade? _estadoExibido;
            private float _preenchimentoExibido = -1f;

            public IconeDeHabilidade(Transform pai, string nome, Font fonte, string simbolo, string rotulo, Color cor,
                float lado, Vector2 posicao)
            {
                _corObtida = cor;
                var raiz = CriarRetangulo(pai, nome);
                raiz.anchorMin = Vector2.zero;
                raiz.anchorMax = Vector2.zero;
                raiz.pivot = Vector2.zero;
                raiz.anchoredPosition = posicao;
                raiz.sizeDelta = new Vector2(lado, lado);

                CriarImagem(raiz, "Fundo", CorDoFundo, 0f);
                _cor = CriarImagem(raiz, "Cor", cor, 6f);
                _veu = CriarImagem(_cor.transform, "Recarga", CorDaRecarga, 0f).rectTransform;
                _veu.gameObject.SetActive(false);

                var areaDoSimbolo = Esticar(CriarRetangulo(raiz, "Simbolo"), 0f);
                areaDoSimbolo.offsetMin = new Vector2(0f, lado * 0.25f);
                _simbolo = ConfigurarTexto(areaDoSimbolo.gameObject, fonte, Mathf.RoundToInt(lado * 0.5f), TextAnchor.MiddleCenter);
                _simbolo.text = simbolo;

                var areaDoNome = Esticar(CriarRetangulo(raiz, "Nome"), 0f);
                areaDoNome.anchorMax = new Vector2(1f, 0.3f);
                _nome = ConfigurarTexto(areaDoNome.gameObject, fonte, Mathf.RoundToInt(lado * 0.18f), TextAnchor.MiddleCenter);
                _nome.text = rotulo;
            }

            public void Mostrar(IndicadorDeHabilidade indicador)
            {
                if (indicador.Estado != _estadoExibido)
                {
                    _estadoExibido = indicador.Estado;
                    var obtida = indicador.Estado != EstadoDaHabilidade.Bloqueada;
                    _cor.color = obtida ? _corObtida : CorBloqueada;
                    var corDoTexto = obtida ? Color.white : new Color(1f, 1f, 1f, 0.4f);
                    _simbolo.color = corDoTexto;
                    _nome.color = corDoTexto;
                    _veu.gameObject.SetActive(indicador.Estado == EstadoDaHabilidade.EmRecarga);
                }

                if (indicador.Estado == EstadoDaHabilidade.EmRecarga && !Mathf.Approximately(indicador.Preenchimento, _preenchimentoExibido))
                {
                    // O véu cobre a parte que ainda falta recarregar, de cima para baixo: o ícone enche de baixo para cima.
                    _preenchimentoExibido = indicador.Preenchimento;
                    _veu.anchorMin = new Vector2(0f, indicador.Preenchimento);
                }
            }
        }
    }
}
