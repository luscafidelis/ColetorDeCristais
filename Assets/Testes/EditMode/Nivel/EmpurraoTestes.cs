using System;
using ColetorDeCristais.Nivel;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Nivel
{
    /// <summary>Testes de <see cref="Empurrao"/> (o carrinho empurrando o personagem).</summary>
    public class EmpurraoTestes
    {
        private const float Tolerancia = 0.001f;
        private const float Raio = 0.5f;

        // Carrinho de 1 × 1,4 m centrado na origem, depois de andar.
        private static readonly Retangulo Carrinho = new Retangulo(Vector2.zero, new Vector2(1f, 1.4f));

        [Test]
        public void NaFrente_EmpurraNaDirecaoDoMovimento_ComAFolga()
        {
            // A frente do carrinho está em z = 0,7; o personagem ficou com o centro em 1,0 (sobrepondo 0,2 m).
            var empurrao = Empurrao.Calcular(Carrinho, new Vector2(0f, 0.05f), new Vector2(0f, 1f), Raio, 0.01f);

            Assert.AreEqual(0f, empurrao.x, Tolerancia);
            Assert.AreEqual(0.21f, empurrao.y, Tolerancia, "sai até 0,7 + 0,5 + 0,01");
        }

        [Test]
        public void NaQuinaDaFrente_SaiPelaQuinaArredondada()
        {
            // Centro a 0,3 m para fora da lateral (x = 0,8): sai quando a distância até a quina (0,5; 0,7) chega a 0,51.
            var empurrao = Empurrao.Calcular(Carrinho, new Vector2(0f, 0.05f), new Vector2(0.8f, 0.9f), Raio, 0.01f);

            var zFinal = 0.9f + empurrao.y;
            Assert.AreEqual(0f, empurrao.x, Tolerancia);
            Assert.AreEqual(0.51f, Vector2.Distance(new Vector2(0.8f, zFinal), new Vector2(0.5f, 0.7f)), Tolerancia);
        }

        [Test]
        public void SoEncostadoNoLado_NaoEmpurra()
        {
            // Encostado na lateral (x = 0,5 + 0,5): distância igual ao raio, sem sobrepor.
            Assert.AreEqual(Vector2.zero, Empurrao.Calcular(Carrinho, new Vector2(0f, 0.05f), new Vector2(1f, 0f), Raio));
        }

        [Test]
        public void Longe_OuCarrinhoParado_NaoEmpurra()
        {
            Assert.AreEqual(Vector2.zero, Empurrao.Calcular(Carrinho, new Vector2(0f, 0.05f), new Vector2(0f, 3f), Raio), "longe");
            Assert.AreEqual(Vector2.zero, Empurrao.Calcular(Carrinho, Vector2.zero, new Vector2(0f, 1f), Raio), "parado");
        }

        [Test]
        public void CarrinhoVoltando_EmpurraParaTras()
        {
            var empurrao = Empurrao.Calcular(Carrinho, new Vector2(0f, -0.05f), new Vector2(0f, -1f), Raio, 0f);

            Assert.AreEqual(-0.2f, empurrao.y, Tolerancia);
        }

        [Test]
        public void ValoresInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => Empurrao.Calcular(Carrinho, Vector2.up, Vector2.zero, 0f));
            Assert.Throws<ArgumentException>(() => Empurrao.Calcular(Carrinho, Vector2.up, Vector2.zero, Raio, -1f));
            Assert.Throws<ArgumentException>(() => Empurrao.Calcular(Carrinho, new Vector2(float.NaN, 0f), Vector2.zero, Raio));
        }
    }
}
