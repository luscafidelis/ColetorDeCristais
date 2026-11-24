using System;
using System.Linq;
using ColetorDeCristais.Habilidades;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Habilidades
{
    /// <summary>
    /// Testes do indicador da investida: a malha do anel (<see cref="MalhaDeAnel"/>) e o pulso
    /// (<see cref="PulsoDoIndicador"/>), com os valores do jogo (pico 1,3 em 0,3 s).
    /// </summary>
    public class IndicadorDaInvestidaTestes
    {
        private const float Tolerancia = 0.0001f;

        [Test]
        public void Anel_ContagemDeVerticesETriangulos()
        {
            MalhaDeAnel.Gerar(1f, 1.2f, 48, out var vertices, out var triangulos);

            Assert.AreEqual(96, vertices.Length);
            Assert.AreEqual(48 * 6, triangulos.Length);
            Assert.IsTrue(triangulos.All(indice => indice >= 0 && indice < vertices.Length));
        }

        [Test]
        public void Anel_RaiosEAlturaZero()
        {
            MalhaDeAnel.Gerar(1f, 1.2f, 12, out var vertices, out _);

            for (var i = 0; i < 12; i++)
            {
                var interno = vertices[2 * i];
                var externo = vertices[2 * i + 1];
                Assert.AreEqual(1f, new Vector2(interno.x, interno.z).magnitude, Tolerancia, $"interno {i}");
                Assert.AreEqual(1.2f, new Vector2(externo.x, externo.z).magnitude, Tolerancia, $"externo {i}");
                Assert.AreEqual(0f, interno.y);
                Assert.AreEqual(0f, externo.y);
            }
        }

        [Test]
        public void Anel_TodosOsTriangulosViradosParaCima()
        {
            MalhaDeAnel.Gerar(0.5f, 2f, 7, out var vertices, out var triangulos);

            for (var t = 0; t < triangulos.Length; t += 3)
            {
                var a = vertices[triangulos[t]];
                var b = vertices[triangulos[t + 1]];
                var c = vertices[triangulos[t + 2]];
                // Mesma convenção da Unity (Mesh.RecalculateNormals): normal = (b - a) × (c - a).
                Assert.Greater(Vector3.Cross(b - a, c - a).y, 0f, $"triângulo {t / 3}");
            }
        }

        [Test]
        public void Anel_SemFuro_Funciona()
        {
            MalhaDeAnel.Gerar(0f, 1f, 3, out var vertices, out var triangulos);

            Assert.AreEqual(6, vertices.Length);
            Assert.AreEqual(18, triangulos.Length);
        }

        [TestCase(-0.1f, 1f, 48)]
        [TestCase(1f, 1f, 48)]
        [TestCase(1.2f, 1f, 48)]
        [TestCase(float.NaN, 1f, 48)]
        [TestCase(0f, float.PositiveInfinity, 48)]
        [TestCase(1f, 1.2f, 2)]
        public void Anel_ValoresInvalidos_Lanca(float interno, float externo, int segmentos)
        {
            Assert.Throws<ArgumentException>(() => MalhaDeAnel.Gerar(interno, externo, segmentos, out _, out _));
        }

        [Test]
        public void Pulso_ComecaEscondido()
        {
            var pulso = new PulsoDoIndicador(1.3f, 0.3f);

            Assert.IsFalse(pulso.Visivel);
            Assert.AreEqual(1f, pulso.Escala);
        }

        [Test]
        public void Pulso_NaoPronta_FicaEscondido()
        {
            var pulso = new PulsoDoIndicador(1.3f, 0.3f);
            pulso.Atualizar(false, 0.1f);

            Assert.IsFalse(pulso.Visivel);
            Assert.AreEqual(1f, pulso.Escala);
        }

        [Test]
        public void Pulso_AoFicarPronta_CresceAteOPicoEVolta()
        {
            var pulso = new PulsoDoIndicador(1.3f, 0.3f);

            pulso.Atualizar(true, 0.016f);
            Assert.IsTrue(pulso.Visivel);
            Assert.AreEqual(1f, pulso.Escala, Tolerancia, "começa na escala 1");
            Assert.IsTrue(pulso.EmPulso);

            pulso.Atualizar(true, 0.15f);
            Assert.AreEqual(1.3f, pulso.Escala, Tolerancia, "pico no meio");

            pulso.Atualizar(true, 0.15f);
            Assert.AreEqual(1f, pulso.Escala, Tolerancia, "volta a 1 no fim");
            Assert.IsFalse(pulso.EmPulso);
            Assert.IsTrue(pulso.Visivel, "continua visível depois do pulso");
        }

        [Test]
        public void Pulso_NuncaPassaDoPico()
        {
            var pulso = new PulsoDoIndicador(1.3f, 0.3f);
            pulso.Atualizar(true, 0f);
            for (var i = 0; i < 40; i++)
            {
                pulso.Atualizar(true, 0.01f);
                Assert.That(pulso.Escala, Is.InRange(1f, 1.3f + Tolerancia));
            }
        }

        [Test]
        public void Pulso_AoUsarAInvestida_SomeEPulsaDeNovoNaVolta()
        {
            var pulso = new PulsoDoIndicador(1.3f, 0.3f);
            pulso.Atualizar(true, 0f);
            pulso.Atualizar(true, 1f);
            Assert.IsFalse(pulso.EmPulso);

            pulso.Atualizar(false, 0.016f);
            Assert.IsFalse(pulso.Visivel);
            Assert.AreEqual(1f, pulso.Escala);

            pulso.Atualizar(true, 0.016f);
            Assert.IsTrue(pulso.Visivel);
            Assert.IsTrue(pulso.EmPulso);
            pulso.Atualizar(true, 0.15f);
            Assert.AreEqual(1.3f, pulso.Escala, Tolerancia);
        }

        [Test]
        public void Pulso_InterrompidoNoMeio_Cancela()
        {
            var pulso = new PulsoDoIndicador(1.3f, 0.3f);
            pulso.Atualizar(true, 0f);
            pulso.Atualizar(true, 0.1f);
            pulso.Atualizar(false, 0.016f);

            Assert.IsFalse(pulso.EmPulso);
            Assert.AreEqual(1f, pulso.Escala);
        }

        [Test]
        public void Pulso_DuracaoZero_SemPulso()
        {
            var pulso = new PulsoDoIndicador(1.3f, 0f);
            pulso.Atualizar(true, 0.016f);

            Assert.IsTrue(pulso.Visivel);
            Assert.AreEqual(1f, pulso.Escala);
            Assert.IsFalse(pulso.EmPulso);
        }

        [Test]
        public void Pulso_TempoNegativo_ContaComoZero()
        {
            var pulso = new PulsoDoIndicador(1.3f, 0.3f);
            pulso.Atualizar(true, 0f);
            pulso.Atualizar(true, 0.15f);
            pulso.Atualizar(true, -1f);

            Assert.AreEqual(1.3f, pulso.Escala, Tolerancia);
        }

        [TestCase(0.9f, 0.3f)]
        [TestCase(float.NaN, 0.3f)]
        [TestCase(1.3f, -0.1f)]
        [TestCase(1.3f, float.PositiveInfinity)]
        public void Pulso_ValoresInvalidos_Lanca(float pico, float duracao)
        {
            Assert.Throws<ArgumentException>(() => new PulsoDoIndicador(pico, duracao));
        }
    }
}
