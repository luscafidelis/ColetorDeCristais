using System;
using System.Collections.Generic;
using ColetorDeCristais.Corpo;
using ColetorDeCristais.Cristais;
using ColetorDeCristais.Habilidades;
using ColetorDeCristais.Movimento;
using ColetorDeCristais.Nivel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

namespace ColetorDeCristais.Partida
{
    /// <summary>
    /// Ponte entre o <see cref="CicloDaPartida"/> e a cena: corre o cronômetro, trava o jogador fora das ondas,
    /// espera uma tecla para continuar, coloca os itens novos de cada onda e recomeça a partida.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Fora de uma onda (chefe, vitória, derrota), desliga o movimento, a investida, o ímã, a coleta e o corpo
    /// (tranco e pisca-pisca) do jogador.
    /// Os efeitos temporários também param, porque perdem tempo no <see cref="MovimentoDoJogador"/>.</item>
    /// <item>Com <see cref="DefinicaoPartida.EncerrarAoColetarTudo"/> (no tutorial), a onda acaba mais cedo quando não
    /// falta nenhum item que conta (<see cref="RegraDoFimAntecipado"/>; as áreas fora da contagem vêm do <i>Nivel</i>).</item>
    /// <item>Os chefes ainda não existem: na vez de um chefe, qualquer tecla conta como chefe derrotado.</item>
    /// <item>Depois do fim (vitória ou derrota), qualquer tecla recarrega a cena.</item>
    /// <item>A derrota acontece por <see cref="DerrotarJogador"/>, para o futuro sistema de vida chamar.</item>
    /// </list>
    /// </remarks>
    public class ControladorDaPartida : MonoBehaviour
    {
        [Tooltip("Valores da partida (asset DefinicaoPartida).")]
        [SerializeField] private DefinicaoPartida definicao;

        [Tooltip("O jogador (objeto com MovimentoDoJogador, InvestidaDoJogador, ImaDoJogador, ColetorDoJogador e CorpoDoJogador).")]
        [SerializeField] private GameObject jogador;

        [Tooltip("Pai dos itens novos de cada onda (o objeto Cristais da cena).")]
        [SerializeField] private Transform grupoDeItens;

        [Tooltip("Nível da cena (opcional): os itens novos não entram nos vãos nem nas áreas sem itens dele.")]
        [SerializeField] private DefinicaoNivel nivel;

        private readonly System.Random _sorteio = new System.Random();
        private Behaviour[] _controlesDoJogador = Array.Empty<Behaviour>();
        private IDisposable _escutaDeTecla;
        private float _esperaRestante;
        private readonly List<Vector2> _itensQueFaltam = new List<Vector2>();

        /// <summary>Estado da partida (onda, tempo, fase). Nulo se o componente não tiver definição.</summary>
        public CicloDaPartida Ciclo { get; private set; }

        /// <summary>Indica se o jogo já aceita uma tecla para continuar (fora de uma onda e depois da espera).</summary>
        public bool AguardandoTecla => Ciclo != null && !Ciclo.EmOnda && _esperaRestante <= 0f;

        private void Awake()
        {
            if (definicao == null || jogador == null)
            {
                Debug.LogError($"{name}: a partida precisa de DefinicaoPartida e do jogador; ela não vai funcionar.", this);
                enabled = false;
                return;
            }

            try
            {
                Ciclo = new CicloDaPartida(definicao.Parametros);
            }
            catch (ArgumentException erro)
            {
                Debug.LogError($"{name}: valores inválidos em {definicao.name} ({erro.Message}); a partida não vai funcionar.", this);
                enabled = false;
                return;
            }

            _controlesDoJogador = new Behaviour[]
            {
                jogador.GetComponent<MovimentoDoJogador>(),
                jogador.GetComponent<InvestidaDoJogador>(),
                jogador.GetComponent<ImaDoJogador>(),
                jogador.GetComponent<ColetorDoJogador>(),
                jogador.GetComponent<CorpoDoJogador>(),
            };
        }

        private void OnDisable() => PararDeEscutar();

        private void Update()
        {
            if (Ciclo.EmOnda)
            {
                if (Ciclo.Atualizar(Time.deltaTime) || (definicao.EncerrarAoColetarTudo && TudoColetado() && Ciclo.EncerrarOnda()))
                {
                    EntrarEmEspera();
                }

                return;
            }

            if (_esperaRestante > 0f)
            {
                _esperaRestante -= Time.deltaTime;
                if (_esperaRestante <= 0f)
                {
                    _escutaDeTecla = InputSystem.onAnyButtonPress.CallOnce(_ => Continuar());
                }
            }
        }

        /// <summary>
        /// O que "qualquer tecla" faz: na vez de um chefe, conta como derrotado (o chefe ainda não existe) e segue
        /// para a próxima onda ou para a vitória; depois do fim, recomeça a partida. Sem efeito durante uma onda.
        /// Público para testes pela CLI.
        /// </summary>
        public void Continuar()
        {
            PararDeEscutar();
            if (Ciclo == null || Ciclo.EmOnda)
            {
                return;
            }

            if (Ciclo.Encerrada)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                return;
            }

            if (Ciclo.DerrotarChefe())
            {
                ColocarItensDaOnda(Ciclo.Onda);
                DefinirControleDoJogador(true);
                Debug.Log($"Partida: onda {Ciclo.Onda} de {Ciclo.QuantidadeDeOndas}.", this);
            }
            else
            {
                EntrarEmEspera(); // vitória
            }
        }

        // Fim antecipado: nenhum item que conta (fora das áreas ignoradas do nível) ainda no chão ou caindo.
        private bool TudoColetado()
        {
            _itensQueFaltam.Clear();
            foreach (var cristal in CristalColetavel.Ativos)
            {
                if (!cristal.Coletado)
                {
                    var p = cristal.transform.position;
                    _itensQueFaltam.Add(new Vector2(p.x, p.z));
                }
            }

            return RegraDoFimAntecipado.TudoColetado(
                _itensQueFaltam, QuedaDoItem.EmQueda, nivel != null ? nivel.AreasForaDoFimAntecipado : null);
        }

        /// <summary>Registra a derrota do jogador (ex.: vida zerada). Sem efeito se a partida já acabou.</summary>
        public void DerrotarJogador()
        {
            if (Ciclo != null && Ciclo.DerrotarJogador())
            {
                EntrarEmEspera();
            }
        }

        private void EntrarEmEspera()
        {
            PararDeEscutar();
            DefinirControleDoJogador(false);
            _esperaRestante = Mathf.Max(definicao.EsperaAntesDeContinuar, 0.0001f);
            Debug.Log($"Partida: {Ciclo.Estado} (onda {Ciclo.Onda}).", this);
        }

        private void PararDeEscutar()
        {
            _escutaDeTecla?.Dispose();
            _escutaDeTecla = null;
        }

        private void DefinirControleDoJogador(bool ligado)
        {
            foreach (var controle in _controlesDoJogador)
            {
                if (controle != null)
                {
                    controle.enabled = ligado;
                }
            }
        }

        private void ColocarItensDaOnda(int onda)
        {
            if (!ComposicaoDaOnda.TrazItensNovos(onda))
            {
                return;
            }

            // Ordem de prioridade se a área encher: habilidade, comidas, experiência.
            var prefabs = new List<GameObject>();
            var habilidade = ComposicaoDaOnda.SortearItemDeHabilidade(definicao.ItensDeHabilidade.Count, _sorteio);
            if (habilidade >= 0)
            {
                prefabs.Add(definicao.ItensDeHabilidade[habilidade]);
            }

            prefabs.AddRange(definicao.ComidasPorOnda);
            var experiencia = ComposicaoDaOnda.CristaisDeExperiencia(onda, definicao.CristaisDeExperienciaNaPrimeiraOnda);
            for (var i = 0; i < experiencia; i++)
            {
                prefabs.Add(definicao.PrefabDeExperiencia);
            }

            var ocupadas = new List<Vector2>();
            foreach (var cristal in CristalColetavel.Ativos)
            {
                var p = cristal.transform.position;
                ocupadas.Add(new Vector2(p.x, p.z));
            }

            var posicaoDoJogador = jogador.transform.position;
            var posicoes = GeradorDePosicoes.Gerar(
                prefabs.Count, definicao.LimiteDaArea, new Vector2(posicaoDoJogador.x, posicaoDoJogador.z),
                ocupadas, definicao.DistanciaMinima, _sorteio,
                proibidas: nivel != null ? nivel.ZonasSemItensNovos : null);
            if (posicoes.Count < prefabs.Count)
            {
                Debug.LogWarning($"Partida: só coube {posicoes.Count} de {prefabs.Count} itens novos na onda {onda}.", this);
            }

            for (var i = 0; i < posicoes.Count; i++)
            {
                if (prefabs[i] == null)
                {
                    continue;
                }

                var posicao = new Vector3(posicoes[i].x, definicao.AlturaDosItens, posicoes[i].y);
                Instantiate(prefabs[i], posicao, Quaternion.identity, grupoDeItens);
            }
        }
    }
}
