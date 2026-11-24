using System;
using ColetorDeCristais.Nivel;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Nivel
{
    /// <summary>Testes de <see cref="VaiEVem"/> (o movimento dos carrinhos).</summary>
    public class VaiEVemTestes
    {
        private const float Tolerancia = 0.0001f;
        private static readonly Vector3 Inicio = new Vector3(0f, 0f, -4f);
        private static readonly Vector3 Fim = new Vector3(0f, 0f, 0.5f);

        // Carrinho do jogo: 4,5 m de percurso a 2 m/s (2,25 s para ir, 4,5 s o ciclo).
        [TestCase(0f, -4f)]
        [TestCase(1f, -2f)]
        [TestCase(2.25f, 0.5f)]
        [TestCase(3.25f, -1.5f)]
        [TestCase(4.5f, -4f)]
        [TestCase(5.5f, -2f)]
        public void Posicao_VaiEVoltaEmVelocidadeConstante(float tempo, float zEsperado)
        {
            var posicao = VaiEVem.Posicao(Inicio, Fim, 2f, tempo);

            Assert.AreEqual(zEsperado, posicao.z, Tolerancia);
            Assert.AreEqual(0f, posicao.x, Tolerancia);
        }

        [Test]
        public void Fase_AdiantaOCiclo()
        {
            Assert.AreEqual(0.5f, VaiEVem.Posicao(Inicio, Fim, 2f, 0f, 0.5f).z, Tolerancia, "fase 0,5 começa na volta");
            Assert.AreEqual(VaiEVem.Posicao(Inicio, Fim, 2f, 1.5f).z, VaiEVem.Posicao(Inicio, Fim, 2f, 0f, 1.5f / 4.5f).z, Tolerancia);
            Assert.AreEqual(-4f, VaiEVem.Posicao(Inicio, Fim, 2f, 0f, 1f).z, Tolerancia, "fase 1 é o ciclo inteiro");
        }

        [TestCase(1f / 30f)]
        [TestCase(1f / 60f)]
        [TestCase(1f / 144f)]
        public void TempoAcumulado_MesmaPosicaoEmQualquerTaxaDeQuadros(float passo)
        {
            // O componente soma Time.deltaTime e pede a posição pelo tempo total.
            var tempo = 0f;
            var quadros = Mathf.RoundToInt(7.3f / passo);
            for (var i = 0; i < quadros; i++)
            {
                tempo += passo;
            }

            var esperado = VaiEVem.Posicao(Inicio, Fim, 2f, (float)(quadros * (double)passo)).z;
            Assert.AreEqual(esperado, VaiEVem.Posicao(Inicio, Fim, 2f, tempo).z, 0.002f);
        }

        [Test]
        public void TempoLongo_ContinuaNaFaixa()
        {
            var percorrido = VaiEVem.Percorrido(4.5f, 2f, 100000.3f);

            Assert.That(percorrido, Is.InRange(0f, 4.5f));
        }

        [Test]
        public void PontosIguais_FicaParado()
        {
            Assert.AreEqual(Inicio, VaiEVem.Posicao(Inicio, Inicio, 2f, 3f));
        }

        [Test]
        public void ValoresInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => VaiEVem.Percorrido(-1f, 2f, 0f));
            Assert.Throws<ArgumentException>(() => VaiEVem.Percorrido(4f, 0f, 0f));
            Assert.Throws<ArgumentException>(() => VaiEVem.Percorrido(4f, 2f, -1f));
            Assert.Throws<ArgumentException>(() => VaiEVem.Percorrido(4f, 2f, float.NaN));
            Assert.Throws<ArgumentException>(() => VaiEVem.Percorrido(4f, 2f, 0f, 1.5f));
        }
    }
}
