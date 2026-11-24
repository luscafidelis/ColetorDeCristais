using System;
using ColetorDeCristais.Corpo;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Corpo
{
    /// <summary>Testes do <see cref="Tranco"/>: distância exata em qualquer taxa de quadros, desaceleração e parada.</summary>
    public class TrancoTestes
    {
        private static Vector2 Somar(Tranco tranco, float passo, int quadros)
        {
            var total = Vector2.zero;
            for (var i = 0; i < quadros; i++)
            {
                total += tranco.Atualizar(passo);
            }

            return total;
        }

        [TestCase(30)]
        [TestCase(60)]
        [TestCase(144)]
        public void Total_EAExatamenteADistancia_EmQualquerTaxaDeQuadros(int quadrosPorSegundo)
        {
            var tranco = new Tranco();
            tranco.Iniciar(new Vector2(0f, 3f), 2f, 0.25f);

            var total = Somar(tranco, 1f / quadrosPorSegundo, quadrosPorSegundo); // 1 s, bem mais que a duração

            Assert.AreEqual(0f, total.x, 1e-4f);
            Assert.AreEqual(2f, total.y, 1e-4f);
            Assert.IsFalse(tranco.EmAndamento);
        }

        [Test]
        public void Direcao_ENormalizada()
        {
            var tranco = new Tranco();
            tranco.Iniciar(new Vector2(3f, 4f), 1f, 0.2f);

            var total = Somar(tranco, 0.05f, 10);

            Assert.AreEqual(0.6f, total.x, 1e-4f);
            Assert.AreEqual(0.8f, total.y, 1e-4f);
        }

        [Test]
        public void Desacelera_PrimeiroQuadroAndaMaisQueOUltimo()
        {
            var tranco = new Tranco();
            tranco.Iniciar(Vector2.right, 2f, 0.25f);

            var primeiro = tranco.Atualizar(0.05f).magnitude;
            tranco.Atualizar(0.05f);
            tranco.Atualizar(0.05f);
            tranco.Atualizar(0.05f);
            var ultimo = tranco.Atualizar(0.05f).magnitude;

            Assert.Greater(primeiro, ultimo * 3f);
            Assert.IsFalse(tranco.EmAndamento);
        }

        [Test]
        public void MetadeDoTempo_TresQuartosDaDistancia()
        {
            var tranco = new Tranco();
            tranco.Iniciar(Vector2.up, 2f, 0.25f);

            var metade = tranco.Atualizar(0.125f);

            Assert.AreEqual(1.5f, metade.y, 1e-5f);
            Assert.IsTrue(tranco.EmAndamento);
        }

        [Test]
        public void SemDirecaoOuSemDistancia_NaoComeca()
        {
            var tranco = new Tranco();
            tranco.Iniciar(Vector2.zero, 2f, 0.25f);
            Assert.IsFalse(tranco.EmAndamento);
            Assert.AreEqual(Vector2.zero, tranco.Atualizar(0.1f));

            tranco.Iniciar(Vector2.up, 0f, 0.25f);
            Assert.IsFalse(tranco.EmAndamento);
        }

        [Test]
        public void Cancelar_Para()
        {
            var tranco = new Tranco();
            tranco.Iniciar(Vector2.up, 2f, 0.25f);
            tranco.Atualizar(0.05f);

            tranco.Cancelar();

            Assert.IsFalse(tranco.EmAndamento);
            Assert.AreEqual(Vector2.zero, tranco.Atualizar(0.05f));
        }

        [Test]
        public void IniciarDeNovo_RecomecaDoZero()
        {
            var tranco = new Tranco();
            tranco.Iniciar(Vector2.up, 2f, 0.25f);
            tranco.Atualizar(0.2f);

            tranco.Iniciar(Vector2.right, 1f, 0.25f);
            var total = Somar(tranco, 0.05f, 10);

            Assert.AreEqual(1f, total.x, 1e-4f);
            Assert.AreEqual(0f, total.y, 1e-4f);
        }

        [Test]
        public void ValoresInvalidos_Lancam()
        {
            var tranco = new Tranco();
            Assert.Throws<ArgumentException>(() => tranco.Iniciar(Vector2.up, -1f, 0.25f));
            Assert.Throws<ArgumentException>(() => tranco.Iniciar(Vector2.up, 1f, 0f));
            Assert.Throws<ArgumentException>(() => tranco.Iniciar(Vector2.up, float.NaN, 0.25f));
            Assert.Throws<ArgumentException>(() => tranco.Atualizar(-0.1f));
        }
    }
}
