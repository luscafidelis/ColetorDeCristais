using System;
using ColetorDeCristais.Nivel;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Nivel
{
    /// <summary>Testes de <see cref="Retangulo"/> (áreas do nível no chão).</summary>
    public class RetanguloTestes
    {
        private const float Tolerancia = 0.0001f;

        // O vão do mercado: de x -10 a 10 e de z 4 a 7.
        private static readonly Retangulo Vao = Retangulo.PelosCantos(new Vector2(-10f, 4f), new Vector2(10f, 7f));

        [Test]
        public void PelosCantos_CalculaCentroETamanho_EmQualquerOrdem()
        {
            var invertido = Retangulo.PelosCantos(new Vector2(10f, 7f), new Vector2(-10f, 4f));

            Assert.AreEqual(new Vector2(0f, 5.5f), Vao.Centro);
            Assert.AreEqual(new Vector2(20f, 3f), Vao.Tamanho);
            Assert.AreEqual(new Vector2(-10f, 4f), Vao.Minimo);
            Assert.AreEqual(new Vector2(10f, 7f), Vao.Maximo);
            Assert.AreEqual(Vao.Centro, invertido.Centro);
            Assert.AreEqual(Vao.Tamanho, invertido.Tamanho);
        }

        [TestCase(0f, 5.5f, true)]
        [TestCase(0f, 4f, true)] // a borda conta como dentro
        [TestCase(0f, 7f, true)]
        [TestCase(0f, 3.99f, false)]
        [TestCase(0f, 7.01f, false)]
        [TestCase(10.5f, 5f, false)]
        public void Contem(float x, float z, bool esperado)
        {
            Assert.AreEqual(esperado, Vao.Contem(new Vector2(x, z)));
        }

        [Test]
        public void DistanciaEPontoMaisProximo()
        {
            Assert.AreEqual(0f, Vao.DistanciaAte(new Vector2(3f, 5f)), "dentro");
            Assert.AreEqual(1f, Vao.DistanciaAte(new Vector2(3f, 3f)), Tolerancia, "embaixo");
            Assert.AreEqual(5f, Vao.DistanciaAte(new Vector2(13f, 11f)), Tolerancia, "na diagonal do canto (3, 4, 5)");
            Assert.AreEqual(new Vector2(10f, 7f), Vao.PontoMaisProximo(new Vector2(13f, 11f)));
        }

        [Test]
        public void ComCentro_MantemOTamanho()
        {
            var movido = Vao.ComCentro(new Vector2(1f, 2f));

            Assert.AreEqual(new Vector2(1f, 2f), movido.Centro);
            Assert.AreEqual(Vao.Tamanho, movido.Tamanho);
        }

        [Test]
        public void Valido_FalsoNoPadraoDoStruct()
        {
            Assert.IsTrue(Vao.Valido);
            Assert.IsFalse(default(Retangulo).Valido, "um item novo numa lista do Inspector começa com tamanho zero");
        }

        [Test]
        public void ValoresInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => new Retangulo(Vector2.zero, new Vector2(0f, 1f)));
            Assert.Throws<ArgumentException>(() => new Retangulo(Vector2.zero, new Vector2(1f, -1f)));
            Assert.Throws<ArgumentException>(() => new Retangulo(new Vector2(float.NaN, 0f), Vector2.one));
            Assert.Throws<ArgumentException>(() => Retangulo.PelosCantos(Vector2.zero, new Vector2(0f, 3f)));
        }
    }
}
