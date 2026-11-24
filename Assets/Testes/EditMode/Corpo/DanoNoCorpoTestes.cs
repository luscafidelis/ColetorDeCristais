using System;
using ColetorDeCristais.Atributos;
using ColetorDeCristais.Corpo;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Corpo
{
    /// <summary>
    /// Testes do <see cref="DanoNoCorpo"/> com os valores do jogo: tamanho inicial 1/3, dano 1/6, derrota abaixo de 1/16,
    /// tranco de 3 m em 0,25 s e 1 s de invulnerabilidade.
    /// </summary>
    public class DanoNoCorpoTestes
    {
        private static readonly ParametrosDeDano Jogo = new ParametrosDeDano(1f / 6f, 1f / 16f, 3f, 0.25f, 1f);

        private static Atributo TamanhoInicial() => new Atributo(1f / 3f, valorMinimo: 0f);

        private static void Esperar(DanoNoCorpo dano, float segundos)
        {
            for (var t = 0f; t < segundos; t += 0.02f)
            {
                dano.Atualizar(0.02f);
            }
        }

        [Test]
        public void PrimeiraBatida_EncolheEDaTranco()
        {
            var dano = new DanoNoCorpo();
            var tamanho = TamanhoInicial();

            var resultado = dano.ReceberBatida(tamanho, Vector2.up, Jogo);

            Assert.AreEqual(ResultadoDaBatida.Dano, resultado);
            Assert.AreEqual(1f / 6f, tamanho.ValorFinal, 1e-5f);
            Assert.IsTrue(dano.Tranco.EmAndamento);
            Assert.IsTrue(dano.Invulnerabilidade.Ativa);
        }

        [Test]
        public void SegundaBatidaLogoDepois_EIgnorada()
        {
            var dano = new DanoNoCorpo();
            var tamanho = TamanhoInicial();
            dano.ReceberBatida(tamanho, Vector2.up, Jogo);
            dano.Atualizar(0.5f);

            var resultado = dano.ReceberBatida(tamanho, Vector2.up, Jogo);

            Assert.AreEqual(ResultadoDaBatida.Ignorada, resultado);
            Assert.AreEqual(1f / 6f, tamanho.ValorFinal, 1e-5f);
        }

        [Test]
        public void SemCarne_SegundaBatidaDepoisDaInvulnerabilidade_Derrota()
        {
            var dano = new DanoNoCorpo();
            var tamanho = TamanhoInicial();
            dano.ReceberBatida(tamanho, Vector2.up, Jogo);
            Esperar(dano, 1.05f);

            var resultado = dano.ReceberBatida(tamanho, Vector2.up, Jogo);

            Assert.AreEqual(ResultadoDaBatida.Derrota, resultado);
            Assert.AreEqual(0f, tamanho.ValorFinal, 1e-5f);
        }

        [Test]
        public void ComUmaCarne_AguentaDuasBatidas_ATerceiraDerrota()
        {
            var dano = new DanoNoCorpo();
            var tamanho = TamanhoInicial();
            tamanho.AdicionarModificador(new Modificador(1f / 6f, TipoModificador.Plano)); // a carne do início

            Assert.AreEqual(ResultadoDaBatida.Dano, dano.ReceberBatida(tamanho, Vector2.up, Jogo));
            Esperar(dano, 1.05f);
            Assert.AreEqual(ResultadoDaBatida.Dano, dano.ReceberBatida(tamanho, Vector2.up, Jogo));
            Esperar(dano, 1.05f);
            Assert.AreEqual(ResultadoDaBatida.Derrota, dano.ReceberBatida(tamanho, Vector2.up, Jogo));
        }

        [Test]
        public void IgualAoMinimo_NaoDerrota()
        {
            var dano = new DanoNoCorpo();
            var tamanho = new Atributo(0.5f, valorMinimo: 0f);
            var parametros = new ParametrosDeDano(0.25f, 0.25f, 2f, 0.25f, 1f);

            Assert.AreEqual(ResultadoDaBatida.Dano, dano.ReceberBatida(tamanho, Vector2.up, parametros));
            Assert.AreEqual(0.25f, tamanho.ValorFinal, 1e-6f);
        }

        [Test]
        public void Tranco_PercorreADistanciaNaDirecaoDaBatida()
        {
            var dano = new DanoNoCorpo();
            var total = Vector2.zero;
            dano.ReceberBatida(TamanhoInicial(), new Vector2(0f, -5f), Jogo);

            for (var i = 0; i < 60; i++)
            {
                total += dano.Atualizar(1f / 60f);
            }

            Assert.AreEqual(0f, total.x, 1e-4f);
            Assert.AreEqual(-3f, total.y, 1e-4f);
            Assert.IsFalse(dano.Tranco.EmAndamento);
            Assert.IsFalse(dano.Invulnerabilidade.Ativa);
        }

        [Test]
        public void Interromper_ParaOTrancoEAInvulnerabilidade()
        {
            var dano = new DanoNoCorpo();
            dano.ReceberBatida(TamanhoInicial(), Vector2.up, Jogo);

            dano.Interromper();

            Assert.IsFalse(dano.Tranco.EmAndamento);
            Assert.IsFalse(dano.Invulnerabilidade.Ativa);
        }

        [Test]
        public void TamanhoNulo_Lanca()
        {
            Assert.Throws<ArgumentNullException>(() => new DanoNoCorpo().ReceberBatida(null, Vector2.up, Jogo));
        }

        [Test]
        public void Parametros_ValoresInvalidos_Lancam()
        {
            Assert.Throws<ArgumentException>(() => new ParametrosDeDano(-0.1f, 0f, 2f, 0.25f, 1f));
            Assert.Throws<ArgumentException>(() => new ParametrosDeDano(0.1f, -1f, 2f, 0.25f, 1f));
            Assert.Throws<ArgumentException>(() => new ParametrosDeDano(0.1f, 0f, -2f, 0.25f, 1f));
            Assert.Throws<ArgumentException>(() => new ParametrosDeDano(0.1f, 0f, 2f, 0f, 1f));
            Assert.Throws<ArgumentException>(() => new ParametrosDeDano(0.1f, 0f, 2f, 0.25f, float.NaN));
        }

        [TestCase(0.05f, 1f / 16f, true)]
        [TestCase(1f / 16f, 1f / 16f, false)]
        [TestCase(1f / 6f, 1f / 16f, false)]
        public void RegraDeTamanho_Derrotado_SoAbaixoDoMinimo(float tamanho, float minimo, bool esperado)
        {
            Assert.AreEqual(esperado, RegraDeTamanho.Derrotado(tamanho, minimo));
        }
    }
}
