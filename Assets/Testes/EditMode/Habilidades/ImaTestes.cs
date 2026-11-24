using System;
using ColetorDeCristais.Cristais;
using ColetorDeCristais.Habilidades;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ColetorDeCristais.Testes.Habilidades
{
    /// <summary>
    /// Testes do <see cref="Ima"/> (liberação, alcance, velocidade e deslocamento), de
    /// <see cref="ParametrosDeIma"/> e da escolha de valores na <see cref="DefinicaoIma"/>.
    /// Os cenários "do jogo" usam os valores do asset: atração completa com raio 3 m e 6 m/s,
    /// reduzida com 1 m e 2 m/s, e 25% da velocidade na borda.
    /// </summary>
    public class ImaTestes
    {
        private const float Tolerancia = 0.0001f;
        private static readonly ParametrosDeIma Completa = new ParametrosDeIma(3f, 6f, 0.25f);
        private static readonly ParametrosDeIma Reduzida = new ParametrosDeIma(1f, 2f, 0.25f);
        private static readonly ParametrosDeIma Constante = new ParametrosDeIma(3f, 2f, 1f);

        // --- Liberação ---

        [Test]
        public void ComecaBloqueado_ELiberarValeParaSempre()
        {
            var ima = new Ima();
            Assert.IsFalse(ima.Liberada);

            ima.Liberar();
            ima.Liberar();

            Assert.IsTrue(ima.Liberada);
        }

        // --- Alcance ---

        [Test]
        public void Alcance_UsaSoADistanciaNoChao()
        {
            var jogador = new Vector3(0f, 1.08f, 0f);

            Assert.IsTrue(Ima.EstaNoAlcance(new Vector3(3f, 0.8f, 0f), jogador, Completa), "na borda conta");
            Assert.IsTrue(Ima.EstaNoAlcance(new Vector3(1.8f, 50f, 2.4f), jogador, Completa), "altura ignorada (3 m no chão)");
            Assert.IsFalse(Ima.EstaNoAlcance(new Vector3(3.01f, 0.8f, 0f), jogador, Completa));
            Assert.IsFalse(Ima.EstaNoAlcance(new Vector3(1.01f, 0.8f, 0f), jogador, Reduzida), "raio reduzido de 1 m");
        }

        // --- Velocidade ---

        [Test]
        public void Velocidade_CresceDaBordaAteOJogador()
        {
            Assert.AreEqual(1.5f, Ima.Velocidade(3f, Completa), Tolerancia, "25% de 6 m/s na borda");
            Assert.AreEqual(3.75f, Ima.Velocidade(1.5f, Completa), Tolerancia, "no meio do raio");
            Assert.AreEqual(6f, Ima.Velocidade(0f, Completa), Tolerancia, "junto do jogador");
            Assert.AreEqual(0f, Ima.Velocidade(3.5f, Completa), "fora do raio");
            Assert.AreEqual(0.5f, Ima.Velocidade(1f, Reduzida), Tolerancia, "25% de 2 m/s na borda reduzida");
        }

        [Test]
        public void Velocidade_ComFracaoUm_EhConstante()
        {
            Assert.AreEqual(2f, Ima.Velocidade(3f, Constante), Tolerancia);
            Assert.AreEqual(2f, Ima.Velocidade(0.5f, Constante), Tolerancia);
        }

        // --- Distância depois de um tempo ---

        [Test]
        public void DistanciaApos_PassoPequeno_SegueAVelocidadeDaquelaDistancia()
        {
            var depois = Ima.DistanciaApos(3f, 0.001f, Completa);

            Assert.AreEqual(3f - 1.5f * 0.001f, depois, 0.00001f);
        }

        [Test]
        public void DistanciaApos_NaoDependeDaTaxaDeQuadros()
        {
            var umPasso = Ima.DistanciaApos(3f, 0.6f, Completa);

            var muitosPassos = 3f;
            for (var i = 0; i < 60; i++)
            {
                muitosPassos = Ima.DistanciaApos(muitosPassos, 0.01f, Completa);
            }

            Assert.AreEqual(umPasso, muitosPassos, 0.001f);
            Assert.Less(umPasso, 3f);
        }

        [Test]
        public void DistanciaApos_DaBorda_ChegaAoJogadorEmMenosDeUmSegundo()
        {
            // Solução exata: t = ln(4) / 1,5 ≈ 0,924 s.
            Assert.Greater(Ima.DistanciaApos(3f, 0.92f, Completa), 0f);
            Assert.AreEqual(0f, Ima.DistanciaApos(3f, 0.93f, Completa));
            Assert.AreEqual(0f, Ima.DistanciaApos(3f, 100f, Completa), "tempo grande não fica negativo");
        }

        [Test]
        public void DistanciaApos_Constante_AndaVelocidadeVezesTempo()
        {
            Assert.AreEqual(1f, Ima.DistanciaApos(2f, 0.5f, Constante), Tolerancia);
            Assert.AreEqual(0f, Ima.DistanciaApos(2f, 5f, Constante));
        }

        // --- Nova posição ---

        [Test]
        public void NovaPosicao_ForaDoRaio_NaoMexe()
        {
            var item = new Vector3(5f, 0.8f, 0f);

            Assert.AreEqual(item, Ima.CalcularNovaPosicao(item, Vector3.zero, Completa, 0.1f));
        }

        [Test]
        public void NovaPosicao_VaiEmLinhaRetaNoChao_EMantemAAltura()
        {
            var jogador = new Vector3(1f, 1.08f, 1f);
            var item = new Vector3(1f + 1.8f, 0.8f, 1f + 2.4f); // 3 m, direção (0,6; 0,8)

            var nova = Ima.CalcularNovaPosicao(item, jogador, Completa, 0.1f);

            var esperada = Ima.DistanciaApos(3f, 0.1f, Completa);
            Assert.AreEqual(esperada, Ima.DistanciaHorizontal(nova, jogador), Tolerancia);
            Assert.AreEqual(0.8f, nova.y, Tolerancia, "mantém a altura do item");
            Assert.AreEqual(0.6f / 0.8f, (nova.x - jogador.x) / (nova.z - jogador.z), Tolerancia, "mesma direção");
        }

        [Test]
        public void NovaPosicao_ComTempoGrande_ParaNoJogadorSemPassar()
        {
            var nova = Ima.CalcularNovaPosicao(new Vector3(2f, 0.8f, 0f), Vector3.zero, Completa, 10f);

            Assert.AreEqual(0f, nova.x, Tolerancia);
            Assert.AreEqual(0f, nova.z, Tolerancia);
            Assert.AreEqual(0.8f, nova.y, Tolerancia);
        }

        [Test]
        public void NovaPosicao_ItemEmCimaDoJogador_NaoMexe()
        {
            var item = new Vector3(0f, 0.8f, 0f);

            Assert.AreEqual(item, Ima.CalcularNovaPosicao(item, new Vector3(0f, 1.08f, 0f), Completa, 0.1f));
        }

        // --- Validação ---

        [Test]
        public void ValoresInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => Ima.DistanciaApos(1f, -0.1f, Completa));
            Assert.Throws<ArgumentException>(() => Ima.DistanciaApos(float.NaN, 0.1f, Completa));
            Assert.Throws<ArgumentException>(() => Ima.CalcularNovaPosicao(new Vector3(float.NaN, 0f, 0f), Vector3.zero, Completa, 0.1f));
            Assert.Throws<ArgumentException>(() => Ima.CalcularNovaPosicao(Vector3.one, Vector3.zero, Completa, float.PositiveInfinity));
        }

        [Test]
        public void Parametros_ForaDaFaixa_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => new ParametrosDeIma(0f, 6f, 0.25f), "raio zero");
            Assert.Throws<ArgumentException>(() => new ParametrosDeIma(3f, 0f, 0.25f), "velocidade zero");
            Assert.Throws<ArgumentException>(() => new ParametrosDeIma(3f, 6f, 1.1f), "fração acima de 1");
            Assert.Throws<ArgumentException>(() => new ParametrosDeIma(3f, 6f, -0.1f), "fração negativa");
            Assert.Throws<ArgumentException>(() => new ParametrosDeIma(float.NaN, 6f, 0.25f), "NaN");
        }

        // --- DefinicaoIma e DefinicaoCristal ---

        [Test]
        public void DefinicaoIma_EscolheOsValoresPeloNivelDeAtracao()
        {
            var definicao = ScriptableObject.CreateInstance<DefinicaoIma>();
            try
            {
                Assert.IsTrue(definicao.TentarObterParametros(AtracaoDoIma.Completa, out var completa));
                Assert.AreEqual(3f, completa.Raio);
                Assert.AreEqual(6f, completa.VelocidadeMaxima);
                Assert.AreEqual(0.25f, completa.FracaoNaBorda);

                Assert.IsTrue(definicao.TentarObterParametros(AtracaoDoIma.Reduzida, out var reduzida));
                Assert.AreEqual(1f, reduzida.Raio);
                Assert.AreEqual(2f, reduzida.VelocidadeMaxima);

                Assert.IsFalse(definicao.TentarObterParametros(AtracaoDoIma.Nenhuma, out _));
            }
            finally
            {
                Object.DestroyImmediate(definicao);
            }
        }

        [Test]
        public void DefinicaoCristal_PorPadrao_TemAtracaoCompleta()
        {
            var definicao = ScriptableObject.CreateInstance<DefinicaoCristal>();
            try
            {
                Assert.AreEqual(AtracaoDoIma.Completa, definicao.AtracaoDoIma);
            }
            finally
            {
                Object.DestroyImmediate(definicao);
            }
        }
    }
}
