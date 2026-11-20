using System;
using ColetorDeCristais.Ferramentas;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Ferramentas
{
    /// <summary>Testes do <see cref="RoteiroDeEntrada"/> usado pelo gravador de quadros.</summary>
    public class RoteiroDeEntradaTestes
    {
        private static RoteiroDeEntrada RoteiroDeExemplo()
        {
            return new RoteiroDeEntrada(new[]
            {
                new EtapaDoRoteiro(Vector2.zero, 1f),
                new EtapaDoRoteiro(Vector2.right, 2f),
                new EtapaDoRoteiro(Vector2.up, 0.5f),
            });
        }

        [Test]
        public void DuracaoTotal_SomaAsEtapas()
        {
            Assert.AreEqual(3.5f, RoteiroDeExemplo().DuracaoTotal, 0.0001f);
        }

        [Test]
        public void EntradaNoTempo_SegueAEtapaAtual()
        {
            var roteiro = RoteiroDeExemplo();

            Assert.AreEqual(Vector2.zero, roteiro.EntradaNoTempo(0.5f));
            Assert.AreEqual(Vector2.right, roteiro.EntradaNoTempo(1f));
            Assert.AreEqual(Vector2.right, roteiro.EntradaNoTempo(2.9f));
            Assert.AreEqual(Vector2.up, roteiro.EntradaNoTempo(3.2f));
        }

        [Test]
        public void ForaDoRoteiro_EntradaEhZero()
        {
            var roteiro = RoteiroDeExemplo();

            Assert.AreEqual(Vector2.zero, roteiro.EntradaNoTempo(-1f));
            Assert.AreEqual(Vector2.zero, roteiro.EntradaNoTempo(3.5f));
            Assert.AreEqual(Vector2.zero, roteiro.EntradaNoTempo(100f));
        }

        [Test]
        public void EtapaDeDuracaoZero_EhPulada()
        {
            var roteiro = new RoteiroDeEntrada(new[]
            {
                new EtapaDoRoteiro(Vector2.left, 0f),
                new EtapaDoRoteiro(Vector2.down, 1f),
            });

            Assert.AreEqual(Vector2.down, roteiro.EntradaNoTempo(0f));
        }

        [Test]
        public void RoteiroVazio_TemDuracaoZero()
        {
            var roteiro = new RoteiroDeEntrada(Array.Empty<EtapaDoRoteiro>());

            Assert.AreEqual(0f, roteiro.DuracaoTotal);
            Assert.AreEqual(Vector2.zero, roteiro.EntradaNoTempo(0f));
        }

        [TestCase(-1f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        public void DuracaoInvalida_LancaExcecao(float duracaoInvalida)
        {
            Assert.Throws<ArgumentException>(() => new RoteiroDeEntrada(new[] { new EtapaDoRoteiro(Vector2.up, duracaoInvalida) }));
        }

        [Test]
        public void ListaNula_LancaExcecao()
        {
            Assert.Throws<ArgumentNullException>(() => new RoteiroDeEntrada(null));
        }
    }
}
