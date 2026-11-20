using System;
using ColetorDeCristais.Cristais;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Cristais
{
    /// <summary>
    /// Testes da animação de giro e flutuação (<see cref="CalculadoraDeFlutuacao"/>).
    /// Os números são arbitrários, não são valores de jogo.
    /// </summary>
    public class CalculadoraDeFlutuacaoTestes
    {
        private const float Tolerancia = 0.0001f;

        [Test]
        public void Flutuacao_ComecaNoRepouso()
        {
            Assert.AreEqual(0f, CalculadoraDeFlutuacao.DeslocamentoVertical(0f, 0.2f, 2f), Tolerancia);
        }

        [Test]
        public void Flutuacao_AtingeAmplitudeNoQuartoDoPeriodo()
        {
            Assert.AreEqual(0.2f, CalculadoraDeFlutuacao.DeslocamentoVertical(0.5f, 0.2f, 2f), Tolerancia);
            Assert.AreEqual(-0.2f, CalculadoraDeFlutuacao.DeslocamentoVertical(1.5f, 0.2f, 2f), Tolerancia);
        }

        [Test]
        public void Flutuacao_SeRepeteACadaPeriodo()
        {
            var primeiro = CalculadoraDeFlutuacao.DeslocamentoVertical(0.3f, 0.2f, 2f);
            var depoisDeUmPeriodo = CalculadoraDeFlutuacao.DeslocamentoVertical(2.3f, 0.2f, 2f);

            Assert.AreEqual(primeiro, depoisDeUmPeriodo, Tolerancia);
        }

        [Test]
        public void Flutuacao_NuncaPassaDaAmplitude()
        {
            for (var tempo = 0f; tempo < 4f; tempo += 0.05f)
            {
                Assert.LessOrEqual(Math.Abs(CalculadoraDeFlutuacao.DeslocamentoVertical(tempo, 0.2f, 2f)), 0.2f + Tolerancia);
            }
        }

        [Test]
        public void Flutuacao_AmplitudeNegativaOuZero_NaoMexe()
        {
            Assert.AreEqual(0f, CalculadoraDeFlutuacao.DeslocamentoVertical(0.5f, -1f, 2f), Tolerancia);
            Assert.AreEqual(0f, CalculadoraDeFlutuacao.DeslocamentoVertical(0.5f, 0f, 2f), Tolerancia);
        }

        [Test]
        public void Flutuacao_ValoresInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => CalculadoraDeFlutuacao.DeslocamentoVertical(1f, 0.2f, 0f));
            Assert.Throws<ArgumentException>(() => CalculadoraDeFlutuacao.DeslocamentoVertical(1f, 0.2f, -2f));
            Assert.Throws<ArgumentException>(() => CalculadoraDeFlutuacao.DeslocamentoVertical(float.NaN, 0.2f, 2f));
            Assert.Throws<ArgumentException>(() => CalculadoraDeFlutuacao.DeslocamentoVertical(1f, float.PositiveInfinity, 2f));
        }

        [Test]
        public void Giro_AvancaComOTempo()
        {
            Assert.AreEqual(0f, CalculadoraDeFlutuacao.AnguloDeGiro(0f, 90f), Tolerancia);
            Assert.AreEqual(45f, CalculadoraDeFlutuacao.AnguloDeGiro(0.5f, 90f), Tolerancia);
        }

        [Test]
        public void Giro_FicaEntreZeroE360()
        {
            Assert.AreEqual(90f, CalculadoraDeFlutuacao.AnguloDeGiro(5f, 90f), Tolerancia);
            Assert.AreEqual(270f, CalculadoraDeFlutuacao.AnguloDeGiro(1f, -90f), Tolerancia);
        }

        [Test]
        public void Giro_ValoresInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => CalculadoraDeFlutuacao.AnguloDeGiro(float.NaN, 90f));
            Assert.Throws<ArgumentException>(() => CalculadoraDeFlutuacao.AnguloDeGiro(1f, float.NegativeInfinity));
        }
    }
}
