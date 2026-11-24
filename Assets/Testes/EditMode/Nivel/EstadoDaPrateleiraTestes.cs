using System;
using System.Collections.Generic;
using ColetorDeCristais.Nivel;
using NUnit.Framework;
using UnityEngine;
using Random = System.Random;

namespace ColetorDeCristais.Testes.Nivel
{
    /// <summary>Testes de <see cref="EstadoDaPrateleira"/> (o segredo das prateleiras).</summary>
    public class EstadoDaPrateleiraTestes
    {
        private const float Tolerancia = 0.0001f;

        // Prateleira de 1 m de largura (x) e 2 m de comprimento (z), no lado oeste de um corredor.
        private static readonly Retangulo Prateleira = new Retangulo(new Vector2(-3f, 0f), new Vector2(1f, 2f));

        [Test]
        public void SoAInvestidaDerruba()
        {
            var estado = new EstadoDaPrateleira();

            Assert.IsFalse(estado.TentarDerrubar(false), "andando");
            Assert.IsFalse(estado.Derrubou);
            Assert.IsTrue(estado.TentarDerrubar(true), "na investida");
            Assert.IsTrue(estado.Derrubou);
        }

        [Test]
        public void DerrubaUmaVezSo()
        {
            var estado = new EstadoDaPrateleira();
            estado.TentarDerrubar(true);

            Assert.IsFalse(estado.TentarDerrubar(true));
        }

        [Test]
        public void SortearProduto_FicaNaFaixa_EUsaTodas()
        {
            var sorteio = new Random(5);
            var vistos = new HashSet<int>();
            for (var i = 0; i < 200; i++)
            {
                var indice = EstadoDaPrateleira.SortearProduto(4, sorteio);
                Assert.That(indice, Is.InRange(0, 3));
                vistos.Add(indice);
            }

            Assert.AreEqual(4, vistos.Count);
            Assert.AreEqual(-1, EstadoDaPrateleira.SortearProduto(0, sorteio));
            Assert.Throws<ArgumentNullException>(() => EstadoDaPrateleira.SortearProduto(2, null));
        }

        [Test]
        public void Produto_CaiNaFrenteDaFaceBatida_ParaOLado()
        {
            // Personagem no corredor, a leste da prateleira (face em x = -2,5), encostado: centro em x = -1,92.
            var personagem = new Vector2(-1.92f, 0.3f);

            var produto = EstadoDaPrateleira.PosicaoDoProduto(Prateleira, personagem, 0.8f, 1.5f);

            Assert.AreEqual(-1.7f, produto.x, Tolerancia, "0,8 m para fora da face");
            Assert.AreEqual(1.8f, produto.y, Tolerancia, "1,5 m ao longo da face");
            Assert.That(Vector2.Distance(produto, personagem), Is.GreaterThan(1.2f), "longe o bastante para não ser coletado na hora");
        }

        [Test]
        public void Produto_LadoNegativo_VaiParaOOutroLado()
        {
            var produto = EstadoDaPrateleira.PosicaoDoProduto(Prateleira, new Vector2(-1.92f, 0.3f), 0.8f, -1.5f);

            Assert.AreEqual(-1.2f, produto.y, Tolerancia);
        }

        [TestCase(-1f, 0f, 1f, 0f)] // leste
        [TestCase(-5f, 0.5f, -1f, 0f)] // oeste
        [TestCase(-3f, 2f, 0f, 1f)] // norte (na ponta da prateleira)
        [TestCase(-3.2f, -1.6f, 0f, -1f)] // sul
        public void NormalDaFace_VoltadaParaOPersonagem(float x, float z, float nx, float nz)
        {
            Assert.AreEqual(new Vector2(nx, nz), EstadoDaPrateleira.NormalDaFace(Prateleira, new Vector2(x, z)));
        }

        [Test]
        public void ValoresInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => EstadoDaPrateleira.PosicaoDoProduto(Prateleira, Vector2.zero, -1f, 0f));
            Assert.Throws<ArgumentException>(() => EstadoDaPrateleira.PosicaoDoProduto(Prateleira, Vector2.zero, 1f, float.NaN));
        }
    }
}
