using System;
using System.Collections.Generic;
using ColetorDeCristais.Nivel;
using ColetorDeCristais.Partida;
using NUnit.Framework;
using UnityEngine;
using Random = System.Random;

namespace ColetorDeCristais.Testes.Partida
{
    /// <summary>
    /// Testes de <see cref="ComposicaoDaOnda"/> (quantos itens novos cada onda traz),
    /// <see cref="GeradorDePosicoes"/> (posições sorteadas) e dos textos de <see cref="MensagensDaPartida"/>.
    /// </summary>
    public class OndasEPosicoesTestes
    {
        // --- ComposicaoDaOnda ---

        [TestCase(1, 4)]
        [TestCase(2, 8)]
        [TestCase(3, 16)]
        public void CristaisDeExperiencia_DobramACadaOnda(int onda, int esperado)
        {
            Assert.AreEqual(esperado, ComposicaoDaOnda.CristaisDeExperiencia(onda, 4));
        }

        [Test]
        public void CristaisDeExperiencia_ValoresInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => ComposicaoDaOnda.CristaisDeExperiencia(0, 4));
            Assert.Throws<ArgumentException>(() => ComposicaoDaOnda.CristaisDeExperiencia(1, -1));
            Assert.Throws<ArgumentException>(() => ComposicaoDaOnda.CristaisDeExperiencia(40, 4));
            Assert.Throws<ArgumentException>(() => ComposicaoDaOnda.CristaisDeExperiencia(32, 1), "2^31 não cabe");
            Assert.Throws<ArgumentException>(() => ComposicaoDaOnda.CristaisDeExperiencia(31, 2), "2 × 2^30 não cabe");
        }

        [Test]
        public void CristaisDeExperiencia_NoLimite_AindaDobra()
        {
            Assert.AreEqual(1 << 30, ComposicaoDaOnda.CristaisDeExperiencia(31, 1));
            Assert.AreEqual(0, ComposicaoDaOnda.CristaisDeExperiencia(40, 0), "sem cristais na primeira onda, nenhuma onda tem");
        }

        [Test]
        public void SoAPartirDaSegundaOnda_TrazItensNovos()
        {
            Assert.IsFalse(ComposicaoDaOnda.TrazItensNovos(1), "a primeira onda usa os itens da cena");
            Assert.IsTrue(ComposicaoDaOnda.TrazItensNovos(2));
            Assert.IsTrue(ComposicaoDaOnda.TrazItensNovos(3));
        }

        [Test]
        public void SortearItemDeHabilidade_FicaNaFaixa_EUsaAsDuasOpcoes()
        {
            var sorteio = new Random(1);
            var vistos = new HashSet<int>();
            for (var i = 0; i < 50; i++)
            {
                var indice = ComposicaoDaOnda.SortearItemDeHabilidade(2, sorteio);
                Assert.That(indice, Is.InRange(0, 1));
                vistos.Add(indice);
            }

            Assert.AreEqual(2, vistos.Count, "café e rosquinha aparecem");
            Assert.AreEqual(-1, ComposicaoDaOnda.SortearItemDeHabilidade(0, sorteio), "sem opções");
        }

        // --- GeradorDePosicoes ---

        [Test]
        public void Posicoes_RespeitamAAreaEAsDistancias()
        {
            var jogador = new Vector2(1f, -2f);
            var ocupadas = new List<Vector2> { new Vector2(3f, 3f), new Vector2(-5f, 0f) };

            var posicoes = GeradorDePosicoes.Gerar(22, 8f, jogador, ocupadas, 2f, new Random(42));

            Assert.AreEqual(22, posicoes.Count, "cabe uma onda 3 inteira (1 + 5 + 16)");
            for (var i = 0; i < posicoes.Count; i++)
            {
                var p = posicoes[i];
                Assert.That(Mathf.Abs(p.x), Is.LessThanOrEqualTo(8f), "dentro da área em x");
                Assert.That(Mathf.Abs(p.y), Is.LessThanOrEqualTo(8f), "dentro da área em z");
                Assert.That(Vector2.Distance(p, jogador), Is.GreaterThanOrEqualTo(2f), "longe do jogador");
                foreach (var ocupada in ocupadas)
                {
                    Assert.That(Vector2.Distance(p, ocupada), Is.GreaterThanOrEqualTo(2f), "longe dos itens que sobraram");
                }

                for (var j = i + 1; j < posicoes.Count; j++)
                {
                    Assert.That(Vector2.Distance(p, posicoes[j]), Is.GreaterThanOrEqualTo(2f), "longe das outras novas");
                }
            }
        }

        [Test]
        public void Posicoes_ForaDasAreasProibidas()
        {
            // O vão do mercado (z de 4 a 7, a largura toda) e uma prateleira; um retângulo inválido é ignorado.
            var vao = Retangulo.PelosCantos(new Vector2(-10f, 4f), new Vector2(10f, 7f));
            var prateleira = Retangulo.PelosCantos(new Vector2(-3.5f, -5f), new Vector2(-2.5f, 2f));
            var proibidas = new List<Retangulo> { vao, prateleira, default };

            var posicoes = GeradorDePosicoes.Gerar(30, 8f, Vector2.zero, null, 1.5f, new Random(11), proibidas: proibidas);

            Assert.AreEqual(30, posicoes.Count);
            foreach (var p in posicoes)
            {
                Assert.IsFalse(vao.Contem(p), $"{p} no vão");
                Assert.IsFalse(prateleira.Contem(p), $"{p} na prateleira");
            }
        }

        [Test]
        public void Posicoes_TudoProibido_NaoDevolveNenhuma()
        {
            var tudo = new List<Retangulo> { new Retangulo(Vector2.zero, new Vector2(20f, 20f)) };

            Assert.AreEqual(0, GeradorDePosicoes.Gerar(5, 8f, new Vector2(50f, 50f), null, 1.5f, new Random(2), 20, tudo).Count);
        }

        [Test]
        public void Posicoes_MesmaSemente_MesmoResultado_SementeDiferente_OutroResultado()
        {
            var a = GeradorDePosicoes.Gerar(5, 8f, Vector2.zero, null, 2f, new Random(7));
            var b = GeradorDePosicoes.Gerar(5, 8f, Vector2.zero, null, 2f, new Random(7));
            var c = GeradorDePosicoes.Gerar(5, 8f, Vector2.zero, null, 2f, new Random(8));

            CollectionAssert.AreEqual(a, b);
            CollectionAssert.AreNotEqual(a, c);
        }

        [Test]
        public void Posicoes_AreaCheia_DevolveMenosQueOPedido()
        {
            // Área de 2 × 2 m com distância mínima de 2 m: cabem poucas posições.
            var posicoes = GeradorDePosicoes.Gerar(20, 1f, new Vector2(50f, 50f), null, 2f, new Random(3), 50);

            Assert.That(posicoes.Count, Is.GreaterThan(0).And.LessThan(20));
        }

        [Test]
        public void Posicoes_ValoresInvalidos_LancamExcecao()
        {
            var sorteio = new Random(1);

            Assert.Throws<ArgumentException>(() => GeradorDePosicoes.Gerar(-1, 8f, Vector2.zero, null, 2f, sorteio));
            Assert.Throws<ArgumentException>(() => GeradorDePosicoes.Gerar(1, 0f, Vector2.zero, null, 2f, sorteio));
            Assert.Throws<ArgumentException>(() => GeradorDePosicoes.Gerar(1, 8f, Vector2.zero, null, -1f, sorteio));
            Assert.Throws<ArgumentException>(() => GeradorDePosicoes.Gerar(1, 8f, Vector2.zero, null, 2f, sorteio, 0));
            Assert.Throws<ArgumentNullException>(() => GeradorDePosicoes.Gerar(1, 8f, Vector2.zero, null, 2f, null));
            Assert.AreEqual(0, GeradorDePosicoes.Gerar(0, 8f, Vector2.zero, null, 2f, sorteio).Count);
        }

        // --- Textos da interface mínima ---

        [Test]
        public void Mensagens_CadaFaseTemSeuTexto()
        {
            Assert.AreEqual(string.Empty, MensagensDaPartida.TextoDaMensagem(EstadoDaPartida.EmOnda, 1, false));
            StringAssert.StartsWith("Chefe da onda 2!", MensagensDaPartida.TextoDaMensagem(EstadoDaPartida.ChefeDaOnda, 2, false));
            StringAssert.StartsWith("Chefe da arena!", MensagensDaPartida.TextoDaMensagem(EstadoDaPartida.ChefeDaArena, 3, false));
            StringAssert.StartsWith("Arena concluída!", MensagensDaPartida.TextoDaMensagem(EstadoDaPartida.Vitoria, 3, false));
            StringAssert.StartsWith("Derrota", MensagensDaPartida.TextoDaMensagem(EstadoDaPartida.Derrota, 2, false));
        }

        [Test]
        public void Mensagens_SoMostramATeclaDepoisDaEspera()
        {
            StringAssert.DoesNotContain("qualquer tecla", MensagensDaPartida.TextoDaMensagem(EstadoDaPartida.Vitoria, 3, false));
            StringAssert.Contains("qualquer tecla para recomeçar", MensagensDaPartida.TextoDaMensagem(EstadoDaPartida.Vitoria, 3, true));
            StringAssert.Contains("qualquer tecla para vencê-lo", MensagensDaPartida.TextoDaMensagem(EstadoDaPartida.ChefeDaOnda, 1, true));
        }
    }
}
