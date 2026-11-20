using System;
using ColetorDeCristais.Movimento;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Movimento
{
    /// <summary>
    /// Testes da <see cref="CalculadoraDeMovimento"/>: deslocamento a partir da entrada,
    /// ângulo alvo e giro suave. Os números são arbitrários, não são valores de jogo.
    /// </summary>
    public class CalculadoraDeMovimentoTestes
    {
        private const float Tolerancia = 0.0001f;

        private static void AssertVetor(Vector3 esperado, Vector3 obtido)
        {
            Assert.AreEqual(esperado.x, obtido.x, Tolerancia, "x");
            Assert.AreEqual(esperado.y, obtido.y, Tolerancia, "y");
            Assert.AreEqual(esperado.z, obtido.z, Tolerancia, "z");
        }

        // --- Deslocamento ---

        [Test]
        public void SemEntrada_NaoDesloca()
        {
            AssertVetor(Vector3.zero, CalculadoraDeMovimento.CalcularDeslocamento(Vector2.zero, 4f, 0.5f));
        }

        [Test]
        public void EntradaParaCima_MoveParaMaisZ()
        {
            var deslocamento = CalculadoraDeMovimento.CalcularDeslocamento(Vector2.up, 4f, 0.5f);

            AssertVetor(new Vector3(0f, 0f, 2f), deslocamento);
        }

        [Test]
        public void EntradaParaDireita_MoveParaMaisX()
        {
            var deslocamento = CalculadoraDeMovimento.CalcularDeslocamento(Vector2.right, 4f, 0.5f);

            AssertVetor(new Vector3(2f, 0f, 0f), deslocamento);
        }

        [Test]
        public void EntradaParaBaixoEEsquerda_MoveNosEixosNegativos()
        {
            AssertVetor(new Vector3(0f, 0f, -1f), CalculadoraDeMovimento.CalcularDeslocamento(Vector2.down, 1f, 1f));
            AssertVetor(new Vector3(-1f, 0f, 0f), CalculadoraDeMovimento.CalcularDeslocamento(Vector2.left, 1f, 1f));
        }

        [Test]
        public void Diagonal_NaoEhMaisRapidaQueReta()
        {
            var deslocamento = CalculadoraDeMovimento.CalcularDeslocamento(new Vector2(1f, 1f), 4f, 1f);

            Assert.AreEqual(4f, deslocamento.magnitude, Tolerancia);
        }

        [Test]
        public void EntradaParcial_DeslocaProporcionalmente()
        {
            // Analógico empurrado até a metade.
            var deslocamento = CalculadoraDeMovimento.CalcularDeslocamento(new Vector2(0f, 0.5f), 4f, 1f);

            AssertVetor(new Vector3(0f, 0f, 2f), deslocamento);
        }

        [Test]
        public void Deslocamento_NuncaTemComponenteVertical()
        {
            var deslocamento = CalculadoraDeMovimento.CalcularDeslocamento(new Vector2(0.3f, -0.8f), 7f, 0.1f);

            Assert.AreEqual(0f, deslocamento.y);
        }

        [Test]
        public void VelocidadeNegativa_EhTratadaComoZero()
        {
            AssertVetor(Vector3.zero, CalculadoraDeMovimento.CalcularDeslocamento(Vector2.up, -3f, 1f));
        }

        [Test]
        public void TempoZero_NaoDesloca()
        {
            AssertVetor(Vector3.zero, CalculadoraDeMovimento.CalcularDeslocamento(Vector2.up, 4f, 0f));
        }

        [Test]
        public void EntradaVelocidadeOuTempoInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => CalculadoraDeMovimento.CalcularDeslocamento(new Vector2(float.NaN, 0f), 1f, 1f));
            Assert.Throws<ArgumentException>(() => CalculadoraDeMovimento.CalcularDeslocamento(Vector2.up, float.PositiveInfinity, 1f));
            Assert.Throws<ArgumentException>(() => CalculadoraDeMovimento.CalcularDeslocamento(Vector2.up, 1f, -0.1f));
            Assert.Throws<ArgumentException>(() => CalculadoraDeMovimento.CalcularDeslocamento(Vector2.up, 1f, float.NaN));
        }

        // --- Ângulo alvo ---

        [TestCase(0f, 1f, 0f)]
        [TestCase(1f, 0f, 90f)]
        [TestCase(0f, -1f, 180f)]
        [TestCase(-1f, 0f, 270f)]
        [TestCase(1f, 1f, 45f)]
        public void AnguloAlvo_SegueAConvencaoDaUnity(float x, float y, float anguloEsperado)
        {
            var calculou = CalculadoraDeMovimento.TentarCalcularAnguloAlvo(new Vector2(x, y), out var angulo);

            Assert.IsTrue(calculou);
            Assert.AreEqual(anguloEsperado, angulo, Tolerancia);
        }

        [Test]
        public void AnguloAlvo_SemEntradaOuRuido_NaoCalcula()
        {
            Assert.IsFalse(CalculadoraDeMovimento.TentarCalcularAnguloAlvo(Vector2.zero, out _));
            Assert.IsFalse(CalculadoraDeMovimento.TentarCalcularAnguloAlvo(new Vector2(0.001f, 0.001f), out _));
            Assert.IsFalse(CalculadoraDeMovimento.TentarCalcularAnguloAlvo(new Vector2(float.NaN, 1f), out _));
        }

        // --- Giro ---

        [Test]
        public void Giro_AvancaNoMaximoOPassoDoQuadro()
        {
            var angulo = CalculadoraDeMovimento.GirarEmDirecao(0f, 90f, 100f, 0.5f);

            Assert.AreEqual(50f, angulo, Tolerancia);
        }

        [Test]
        public void Giro_NaoPassaDoAlvo()
        {
            var angulo = CalculadoraDeMovimento.GirarEmDirecao(0f, 30f, 100f, 1f);

            Assert.AreEqual(30f, angulo, Tolerancia);
        }

        [Test]
        public void Giro_UsaOCaminhoMaisCurtoPassandoPorZero()
        {
            // De 350° para 10°: o caminho curto é +20°, passando por 0°, e não -340°.
            var angulo = CalculadoraDeMovimento.GirarEmDirecao(350f, 10f, 10f, 1f);

            Assert.AreEqual(0f, Mathf.DeltaAngle(0f, angulo), Tolerancia);
        }

        [Test]
        public void Giro_ResultadoFicaEntreZeroE360()
        {
            var angulo = CalculadoraDeMovimento.GirarEmDirecao(10f, 340f, 15f, 1f);

            Assert.AreEqual(355f, angulo, Tolerancia);
        }

        [Test]
        public void Giro_VelocidadeAngularNegativaOuTempoZero_NaoGira()
        {
            Assert.AreEqual(20f, CalculadoraDeMovimento.GirarEmDirecao(20f, 90f, -50f, 1f), Tolerancia);
            Assert.AreEqual(20f, CalculadoraDeMovimento.GirarEmDirecao(20f, 90f, 50f, 0f), Tolerancia);
        }

        [Test]
        public void Giro_ValoresInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => CalculadoraDeMovimento.GirarEmDirecao(float.NaN, 0f, 1f, 1f));
            Assert.Throws<ArgumentException>(() => CalculadoraDeMovimento.GirarEmDirecao(0f, 0f, 1f, -1f));
        }
    }
}
