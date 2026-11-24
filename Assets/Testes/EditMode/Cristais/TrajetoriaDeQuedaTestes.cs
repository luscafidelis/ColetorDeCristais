using System;
using ColetorDeCristais.Cristais;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Cristais
{
    /// <summary>Testes da <see cref="TrajetoriaDeQueda"/>: pontas, arco no meio, linha reta no chão e valores inválidos.</summary>
    public class TrajetoriaDeQuedaTestes
    {
        private static readonly Vector3 Inicio = new Vector3(2.84f, 1.42f, -1f);
        private static readonly Vector3 Fim = new Vector3(2f, 0.8f, 0.5f);

        private static void Igual(Vector3 esperado, Vector3 obtido)
        {
            Assert.AreEqual(esperado.x, obtido.x, 1e-5f);
            Assert.AreEqual(esperado.y, obtido.y, 1e-5f);
            Assert.AreEqual(esperado.z, obtido.z, 1e-5f);
        }

        [Test]
        public void ComecaNoInicio_TerminaNoFim()
        {
            Igual(Inicio, TrajetoriaDeQueda.Posicao(Inicio, Fim, 0.3f, 0.4f, 0f));
            Igual(Fim, TrajetoriaDeQueda.Posicao(Inicio, Fim, 0.3f, 0.4f, 0.4f));
        }

        [Test]
        public void DepoisDaDuracao_FicaNoFim_AntesDoInicio_FicaNoInicio()
        {
            Igual(Fim, TrajetoriaDeQueda.Posicao(Inicio, Fim, 0.3f, 0.4f, 5f));
            Igual(Inicio, TrajetoriaDeQueda.Posicao(Inicio, Fim, 0.3f, 0.4f, -1f));
        }

        [Test]
        public void NoMeio_SobeAAlturaDoArcoAcimaDaLinhaReta()
        {
            var meio = TrajetoriaDeQueda.Posicao(Inicio, Fim, 0.3f, 0.4f, 0.2f);

            Assert.AreEqual((Inicio.y + Fim.y) / 2f + 0.3f, meio.y, 1e-5f);
            Assert.AreEqual((Inicio.x + Fim.x) / 2f, meio.x, 1e-5f);
            Assert.AreEqual((Inicio.z + Fim.z) / 2f, meio.z, 1e-5f);
        }

        [Test]
        public void SemArco_LinhaReta()
        {
            var umQuarto = TrajetoriaDeQueda.Posicao(Inicio, Fim, 0f, 0.4f, 0.1f);

            Igual(Vector3.Lerp(Inicio, Fim, 0.25f), umQuarto);
        }

        [Test]
        public void NoChao_AndaEmLinhaRetaComVelocidadeConstante()
        {
            var a = TrajetoriaDeQueda.Posicao(Inicio, Fim, 0.3f, 0.4f, 0.1f);
            var b = TrajetoriaDeQueda.Posicao(Inicio, Fim, 0.3f, 0.4f, 0.2f);
            var c = TrajetoriaDeQueda.Posicao(Inicio, Fim, 0.3f, 0.4f, 0.3f);

            Assert.AreEqual(b.x - a.x, c.x - b.x, 1e-5f);
            Assert.AreEqual(b.z - a.z, c.z - b.z, 1e-5f);
        }

        [TestCase(0.39f, false)]
        [TestCase(0.4f, true)]
        [TestCase(1f, true)]
        public void Terminou_SoDepoisDaDuracao(float tempo, bool esperado)
        {
            Assert.AreEqual(esperado, TrajetoriaDeQueda.Terminou(0.4f, tempo));
        }

        [Test]
        public void ValoresInvalidos_Lancam()
        {
            Assert.Throws<ArgumentException>(() => TrajetoriaDeQueda.Posicao(Inicio, Fim, -0.1f, 0.4f, 0f));
            Assert.Throws<ArgumentException>(() => TrajetoriaDeQueda.Posicao(Inicio, Fim, 0.3f, 0f, 0f));
            Assert.Throws<ArgumentException>(() => TrajetoriaDeQueda.Posicao(Inicio, Fim, 0.3f, 0.4f, float.NaN));
            Assert.Throws<ArgumentException>(() => TrajetoriaDeQueda.Posicao(Inicio, Fim, float.PositiveInfinity, 0.4f, 0f));
        }
    }
}
