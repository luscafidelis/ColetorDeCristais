using System;
using ColetorDeCristais.Corpo;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Corpo
{
    /// <summary>Testes da <see cref="RegraDeTamanho"/>: altura do centro para manter os pés no chão.</summary>
    public class RegraDeTamanhoTestes
    {
        [TestCase(1f, 1f, 1f)]
        [TestCase(1f, 1f / 3f, 1f / 3f)]
        [TestCase(1f, 0.5f, 0.5f)]
        [TestCase(2f, 1.5f, 3f)]
        [TestCase(1f, 0f, 0f)]
        public void AlturaDoCentro_ProporcionalAoTamanho(float alturaNaEscalaUm, float tamanho, float esperado)
        {
            Assert.AreEqual(esperado, RegraDeTamanho.AlturaDoCentro(alturaNaEscalaUm, tamanho), 1e-6f);
        }

        [Test]
        public void AlturaDoCentro_TamanhoInicialDoJogo_PesNoChao()
        {
            // O CharacterController tem 2 m de altura com o centro no meio: com tamanho t, os pés ficam
            // em (altura do centro) − t. Com 1/3, o centro fica a 1/3 m e os pés em zero.
            var tamanho = 1f / 3f;
            var centro = RegraDeTamanho.AlturaDoCentro(1f, tamanho);
            Assert.AreEqual(0f, centro - 1f * tamanho, 1e-6f);
        }

        [TestCase(-0.1f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        public void AlturaDoCentro_TamanhoInvalido_Lanca(float tamanho)
        {
            Assert.Throws<ArgumentException>(() => RegraDeTamanho.AlturaDoCentro(1f, tamanho));
        }

        [TestCase(float.NaN)]
        [TestCase(float.NegativeInfinity)]
        public void AlturaDoCentro_AlturaInvalida_Lanca(float altura)
        {
            Assert.Throws<ArgumentException>(() => RegraDeTamanho.AlturaDoCentro(altura, 1f));
        }
    }
}
