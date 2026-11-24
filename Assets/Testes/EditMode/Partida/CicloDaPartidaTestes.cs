using System;
using ColetorDeCristais.Partida;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Partida
{
    /// <summary>
    /// Testes do <see cref="CicloDaPartida"/> (cronômetro, ondas, chefes, vitória, derrota e recomeço),
    /// de <see cref="ParametrosDaPartida"/> e da formatação do cronômetro.
    /// O cenário "do jogo" usa os valores do asset: 3 ondas de 120 s.
    /// </summary>
    public class CicloDaPartidaTestes
    {
        private const float Tolerancia = 0.0001f;
        private static readonly ParametrosDaPartida DoJogo = new ParametrosDaPartida(120f, 3);

        private static void PassarOTempo(CicloDaPartida ciclo, float total, float passo)
        {
            for (var tempo = 0f; tempo < total - 1e-4f; tempo += passo)
            {
                ciclo.Atualizar(passo);
            }
        }

        // --- Início e cronômetro ---

        [Test]
        public void Comeca_NaOnda1_ComOTempoCheio()
        {
            var ciclo = new CicloDaPartida(DoJogo);

            Assert.AreEqual(1, ciclo.Onda);
            Assert.AreEqual(3, ciclo.QuantidadeDeOndas);
            Assert.AreEqual(120f, ciclo.TempoRestante);
            Assert.AreEqual(EstadoDaPartida.EmOnda, ciclo.Estado);
            Assert.IsTrue(ciclo.EmOnda);
            Assert.IsFalse(ciclo.EmChefe);
            Assert.IsFalse(ciclo.Encerrada);
        }

        [Test]
        public void Cronometro_DesceComOTempo()
        {
            var ciclo = new CicloDaPartida(DoJogo);

            Assert.IsFalse(ciclo.Atualizar(30f));

            Assert.AreEqual(90f, ciclo.TempoRestante, Tolerancia);
            Assert.IsTrue(ciclo.EmOnda);
        }

        [Test]
        public void FimDoTempo_NaoDependeDaTaxaDeQuadros()
        {
            var umQuadro = new CicloDaPartida(DoJogo);
            var aSessenta = new CicloDaPartida(DoJogo);
            var aTrinta = new CicloDaPartida(DoJogo);

            umQuadro.Atualizar(120f);
            PassarOTempo(aSessenta, 120f, 1f / 60f);
            PassarOTempo(aTrinta, 120f, 1f / 30f);

            foreach (var ciclo in new[] { umQuadro, aSessenta, aTrinta })
            {
                Assert.AreEqual(EstadoDaPartida.ChefeDaOnda, ciclo.Estado);
                Assert.AreEqual(0f, ciclo.TempoRestante);
            }
        }

        [Test]
        public void FimDoTempo_AvisaUmaVez_EPassaDoZeroSemFicarNegativo()
        {
            var ciclo = new CicloDaPartida(DoJogo);
            ciclo.Atualizar(119.5f);

            Assert.IsTrue(ciclo.Atualizar(2f), "o tempo acabou neste passo");
            Assert.AreEqual(0f, ciclo.TempoRestante);
            Assert.IsFalse(ciclo.Atualizar(5f), "na vez do chefe o cronômetro não corre");
            Assert.AreEqual(0f, ciclo.TempoRestante);
        }

        // --- Chefes e ondas ---

        [Test]
        public void ChefeDaOndaDerrotado_ComecaAProximaOnda_ComOTempoCheio()
        {
            var ciclo = new CicloDaPartida(DoJogo);
            ciclo.Atualizar(120f);

            Assert.IsTrue(ciclo.EmChefe);
            Assert.IsTrue(ciclo.DerrotarChefe());

            Assert.AreEqual(2, ciclo.Onda);
            Assert.AreEqual(120f, ciclo.TempoRestante);
            Assert.IsTrue(ciclo.EmOnda);
        }

        [Test]
        public void UltimaOnda_TrazOChefeDaArena_EVencerEhAVitoria()
        {
            var ciclo = new CicloDaPartida(DoJogo);
            for (var onda = 1; onda <= 2; onda++)
            {
                ciclo.Atualizar(120f);
                Assert.AreEqual(EstadoDaPartida.ChefeDaOnda, ciclo.Estado, $"onda {onda}");
                ciclo.DerrotarChefe();
            }

            ciclo.Atualizar(120f);
            Assert.AreEqual(3, ciclo.Onda);
            Assert.AreEqual(EstadoDaPartida.ChefeDaArena, ciclo.Estado, "o chefe da arena substitui o da 3ª onda");

            Assert.IsFalse(ciclo.DerrotarChefe(), "não começa outra onda");
            Assert.AreEqual(EstadoDaPartida.Vitoria, ciclo.Estado);
            Assert.IsTrue(ciclo.Encerrada);
        }

        [Test]
        public void UmaOndaSo_OChefeEhODaArena()
        {
            var ciclo = new CicloDaPartida(new ParametrosDaPartida(10f, 1));

            ciclo.Atualizar(10f);

            Assert.AreEqual(EstadoDaPartida.ChefeDaArena, ciclo.Estado);
        }

        [Test]
        public void DerrotarChefe_ForaDaVezDoChefe_NaoFazNada()
        {
            var ciclo = new CicloDaPartida(DoJogo);

            Assert.IsFalse(ciclo.DerrotarChefe());
            Assert.AreEqual(1, ciclo.Onda);
            Assert.IsTrue(ciclo.EmOnda);
        }

        // --- Derrota e recomeço ---

        [Test]
        public void DerrotarJogador_DuranteAOndaOuNoChefe_EncerraComDerrota()
        {
            var naOnda = new CicloDaPartida(DoJogo);
            var noChefe = new CicloDaPartida(DoJogo);
            noChefe.Atualizar(120f);

            Assert.IsTrue(naOnda.DerrotarJogador());
            Assert.IsTrue(noChefe.DerrotarJogador());

            Assert.AreEqual(EstadoDaPartida.Derrota, naOnda.Estado);
            Assert.AreEqual(EstadoDaPartida.Derrota, noChefe.Estado);
            Assert.IsTrue(naOnda.Encerrada);
            Assert.IsFalse(naOnda.Atualizar(10f), "o cronômetro para");
        }

        [Test]
        public void DepoisDoFim_NemDerrotaNemChefeMudamOEstado()
        {
            var ciclo = new CicloDaPartida(new ParametrosDaPartida(10f, 1));
            ciclo.Atualizar(10f);
            ciclo.DerrotarChefe(); // vitória

            Assert.IsFalse(ciclo.DerrotarJogador());
            Assert.IsFalse(ciclo.DerrotarChefe());
            Assert.AreEqual(EstadoDaPartida.Vitoria, ciclo.Estado);
        }

        [Test]
        public void Reiniciar_VoltaParaAOnda1_ComOTempoCheio()
        {
            var ciclo = new CicloDaPartida(DoJogo);
            ciclo.Atualizar(120f);
            ciclo.DerrotarChefe();
            ciclo.Atualizar(50f);
            ciclo.DerrotarJogador();

            ciclo.Reiniciar();

            Assert.AreEqual(1, ciclo.Onda);
            Assert.AreEqual(120f, ciclo.TempoRestante);
            Assert.IsTrue(ciclo.EmOnda);
        }

        // --- Validação ---

        [Test]
        public void ValoresInvalidos_LancamExcecao()
        {
            var ciclo = new CicloDaPartida(DoJogo);

            Assert.Throws<ArgumentException>(() => ciclo.Atualizar(-1f));
            Assert.Throws<ArgumentException>(() => ciclo.Atualizar(float.NaN));
            Assert.Throws<ArgumentException>(() => new ParametrosDaPartida(0f, 3), "duração zero");
            Assert.Throws<ArgumentException>(() => new ParametrosDaPartida(float.PositiveInfinity, 3), "duração infinita");
            Assert.Throws<ArgumentException>(() => new ParametrosDaPartida(120f, 0), "sem ondas");
        }

        // --- Formatação ---

        [TestCase(120f, "2:00")]
        [TestCase(119.01f, "2:00")]
        [TestCase(119f, "1:59")]
        [TestCase(61f, "1:01")]
        [TestCase(9.5f, "0:10")]
        [TestCase(0.01f, "0:01")]
        [TestCase(0f, "0:00")]
        [TestCase(-3f, "0:00")]
        [TestCase(float.NaN, "0:00")]
        public void FormatarTempo_ArredondaParaCima(float segundos, string esperado)
        {
            Assert.AreEqual(esperado, CicloDaPartida.FormatarTempo(segundos));
        }

        [TestCase(120f, 120)]
        [TestCase(119.2f, 120)]
        [TestCase(0.05f, 1)]
        [TestCase(0f, 0)]
        [TestCase(-1f, 0)]
        [TestCase(float.NaN, 0)]
        [TestCase(1e9f, 359999)]
        public void SegundosExibidos_ArredondaParaCimaENaFaixa(float segundos, int esperado)
        {
            Assert.AreEqual(esperado, CicloDaPartida.SegundosExibidos(segundos));
        }
    }
}
