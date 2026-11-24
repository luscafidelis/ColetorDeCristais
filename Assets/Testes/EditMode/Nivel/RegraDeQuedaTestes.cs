using System;
using System.Collections.Generic;
using ColetorDeCristais.Habilidades;
using ColetorDeCristais.Nivel;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Nivel
{
    /// <summary>
    /// Testes de <see cref="RegraDeQueda"/>, inclusive a travessia do vão do mercado (3 m) com a <see cref="Investida"/> do jogo.
    /// </summary>
    public class RegraDeQuedaTestes
    {
        private const float VelocidadeBase = 3.5f;
        private static readonly ParametrosDeInvestida InvestidaDoJogo = new ParametrosDeInvestida(5f, 0.25f, 5f);

        // O vão do mercado: a largura toda, de z 4 a 7.
        private static readonly List<Retangulo> Vaos = new List<Retangulo>
        {
            Retangulo.PelosCantos(new Vector2(-10f, 4f), new Vector2(10f, 7f)),
        };

        [Test]
        public void SobreOVao_SemInvestida_Cai()
        {
            Assert.IsTrue(RegraDeQueda.Caiu(new Vector2(0f, 5f), false, Vaos));
            Assert.IsTrue(RegraDeQueda.Caiu(new Vector2(-9.5f, 4f), false, Vaos), "na borda");
        }

        [Test]
        public void SobreOVao_EmInvestida_NaoCai()
        {
            Assert.IsFalse(RegraDeQueda.Caiu(new Vector2(0f, 5f), true, Vaos));
        }

        [Test]
        public void ForaDoVao_NaoCai()
        {
            Assert.IsFalse(RegraDeQueda.Caiu(new Vector2(0f, 3.9f), false, Vaos));
            Assert.IsFalse(RegraDeQueda.Caiu(new Vector2(0f, 7.1f), false, Vaos));
        }

        [Test]
        public void SemVaos_OuVaoInvalido_NaoCai()
        {
            Assert.IsFalse(RegraDeQueda.Caiu(Vector2.zero, false, null));
            Assert.IsFalse(RegraDeQueda.Caiu(Vector2.zero, false, new List<Retangulo> { default }));
        }

        // Simula a investida do jogo, para o norte, conferindo a queda a cada quadro, como o ControladorDoNivel.
        private static (bool caiu, float zFinal) Investir(float zInicial, float velocidade, float passo)
        {
            var investida = new Investida();
            investida.Liberar();
            investida.TentarIniciar(Vector3.forward, velocidade, InvestidaDoJogo);
            var z = zInicial;
            for (var quadro = 0; quadro < 200; quadro++)
            {
                z += investida.Atualizar(passo).z;
                if (RegraDeQueda.Caiu(new Vector2(0f, z), investida.EmAndamento, Vaos))
                {
                    return (true, z);
                }
            }

            return (false, z);
        }

        [TestCase(1f / 30f)]
        [TestCase(1f / 60f)]
        [TestCase(1f / 144f)]
        public void InvestidaBase_PertoDaBorda_AtravessaOVao(float passo)
        {
            // 4,375 m a partir de z = 3,9: termina em 8,275, depois do vão (7).
            var (caiu, zFinal) = Investir(3.9f, VelocidadeBase, passo);

            Assert.IsFalse(caiu);
            Assert.AreEqual(8.275f, zFinal, 0.001f);
        }

        [TestCase(1f / 30f)]
        [TestCase(1f / 60f)]
        public void InvestidaBase_LongeDaBorda_CaiNoVao(float passo)
        {
            // A partir de z = 2,5 termina em 6,875, ainda sobre o vão.
            var (caiu, zFinal) = Investir(2.5f, VelocidadeBase, passo);

            Assert.IsTrue(caiu);
            Assert.AreEqual(6.875f, zFinal, 0.001f);
        }

        [Test]
        public void InvestidaBase_AtravessaAPartirDeAte1375mDaBorda()
        {
            // Folga da investida base sobre o vão de 3 m: 4,375 - 3 = 1,375 m.
            Assert.IsFalse(Investir(4f - 1.37f, VelocidadeBase, 1f / 60f).caiu);
            Assert.IsTrue(Investir(4f - 1.38f, VelocidadeBase, 1f / 60f).caiu);
        }

        [Test]
        public void ComACoxa_AtravessaDeMaisLonge()
        {
            // Coxa ativa: 3,5 × 1,3 = 4,55 m/s, investida de 5,6875 m.
            Assert.IsFalse(Investir(2.5f, VelocidadeBase * 1.3f, 1f / 60f).caiu);
        }

        [Test]
        public void Descida_QuedaLivreComLimite()
        {
            Assert.AreEqual(0f, RegraDeQueda.Descida(0f, 20f, 5f));
            Assert.AreEqual(2.5f, RegraDeQueda.Descida(0.5f, 20f, 5f), 0.0001f);
            Assert.AreEqual(5f, RegraDeQueda.Descida(3f, 20f, 5f), "para na profundidade");
        }

        [Test]
        public void Descida_ValoresInvalidos_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => RegraDeQueda.Descida(-1f, 20f, 5f));
            Assert.Throws<ArgumentException>(() => RegraDeQueda.Descida(1f, 0f, 5f));
            Assert.Throws<ArgumentException>(() => RegraDeQueda.Descida(1f, 20f, -1f));
        }
    }
}
