using System;
using ColetorDeCristais.Habilidades;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Habilidades
{
    /// <summary>
    /// Testes da <see cref="Investida"/> (liberação, recarga, deslocamento) e de <see cref="ParametrosDeInvestida"/>.
    /// O cenário "do jogo" usa os valores do asset (5× a velocidade por 0,25 s, recarga de 5 s, base 3,5 m/s);
    /// os outros números são arbitrários.
    /// </summary>
    public class InvestidaTestes
    {
        private const float Tolerancia = 0.0001f;
        private static readonly ParametrosDeInvestida DoJogo = new ParametrosDeInvestida(5f, 0.25f, 5f);

        private static void AssertVetor(Vector3 esperado, Vector3 obtido)
        {
            Assert.AreEqual(esperado.x, obtido.x, Tolerancia, "x");
            Assert.AreEqual(esperado.y, obtido.y, Tolerancia, "y");
            Assert.AreEqual(esperado.z, obtido.z, Tolerancia, "z");
        }

        private static Investida Liberada()
        {
            var investida = new Investida();
            investida.Liberar();
            return investida;
        }

        private static Vector3 Percorrer(Investida investida, float tempoTotal, float passo)
        {
            var total = Vector3.zero;
            for (var tempo = 0f; tempo < tempoTotal - 1e-6f; tempo += passo)
            {
                total += investida.Atualizar(passo);
            }

            return total;
        }

        // --- Liberação ---

        [Test]
        public void ComecaBloqueada_ENaoInicia()
        {
            var investida = new Investida();

            Assert.IsFalse(investida.Liberada);
            Assert.IsFalse(investida.TentarIniciar(Vector3.forward, 3.5f, DoJogo));
            Assert.IsFalse(investida.EmAndamento);
            Assert.AreEqual(Vector3.zero, investida.Atualizar(0.1f));
        }

        [Test]
        public void Liberada_Inicia()
        {
            var investida = Liberada();

            Assert.IsTrue(investida.Pronta);
            Assert.IsTrue(investida.TentarIniciar(Vector3.forward, 3.5f, DoJogo));
            Assert.IsTrue(investida.EmAndamento);
        }

        // --- Deslocamento ---

        [Test]
        public void DoJogo_PercorreCincoVezesAVelocidadePorUmQuartoDeSegundo()
        {
            var investida = Liberada();
            investida.TentarIniciar(Vector3.forward, 3.5f, DoJogo);

            var total = Percorrer(investida, 0.5f, 1f / 60f);

            Assert.AreEqual(3.5f * 5f * 0.25f, total.z, 0.001f, "4,375 m para a frente");
            Assert.AreEqual(0f, total.x, Tolerancia);
            Assert.AreEqual(0f, total.y, Tolerancia);
            Assert.IsFalse(investida.EmAndamento);
            Assert.AreEqual(4.375f, DoJogo.Distancia(3.5f), Tolerancia);
        }

        [Test]
        public void UltimoQuadro_UsaSoOTempoQueFaltava()
        {
            var investida = Liberada();
            investida.TentarIniciar(Vector3.right, 2f, new ParametrosDeInvestida(5f, 0.25f, 1f));

            var primeiro = investida.Atualizar(0.2f);
            var segundo = investida.Atualizar(0.2f);

            Assert.AreEqual(2f, primeiro.x, Tolerancia, "10 m/s × 0,2 s");
            Assert.AreEqual(0.5f, segundo.x, Tolerancia, "só 0,05 s faltavam");
            Assert.AreEqual(Vector3.zero, investida.Atualizar(0.2f));
        }

        [Test]
        public void VelocidadeComBonus_InvestidaMaisLonga()
        {
            var investida = Liberada();
            investida.TentarIniciar(Vector3.forward, 4.55f, DoJogo);

            var total = Percorrer(investida, 0.25f, 0.05f);

            Assert.AreEqual(4.55f * 5f * 0.25f, total.z, 0.001f);
        }

        [Test]
        public void Direcao_EhNormalizadaEHorizontal()
        {
            var investida = Liberada();
            investida.TentarIniciar(new Vector3(3f, 7f, 4f), 2f, new ParametrosDeInvestida(1f, 1f, 0f));

            AssertVetor(new Vector3(0.6f, 0f, 0.8f) * 2f, investida.Velocidade);
        }

        [Test]
        public void SemDirecaoOuSemVelocidade_NaoInicia_ENaoGastaARecarga()
        {
            var investida = Liberada();

            Assert.IsFalse(investida.TentarIniciar(Vector3.up, 3.5f, DoJogo), "só vertical");
            Assert.IsFalse(investida.TentarIniciar(Vector3.forward, 0f, DoJogo), "parado por modificador");
            Assert.IsTrue(investida.Pronta);
        }

        // --- Recarga ---

        [Test]
        public void RecargaTotal_ZeroAntesDaPrimeira_EDepoisADaUltimaInvestida()
        {
            var investida = Liberada();
            Assert.AreEqual(0f, investida.RecargaTotal);

            investida.TentarIniciar(Vector3.forward, 3.5f, DoJogo);
            investida.Atualizar(1f);
            Assert.AreEqual(5f, investida.RecargaTotal, "não diminui com o tempo");

            investida.Atualizar(10f);
            investida.TentarIniciar(Vector3.forward, 3.5f, new ParametrosDeInvestida(5f, 0.25f, 2f));
            Assert.AreEqual(2f, investida.RecargaTotal);
        }

        [Test]
        public void Recarga_BloqueiaAteTerminar_EContaDesdeOInicio()
        {
            var investida = Liberada();
            investida.TentarIniciar(Vector3.forward, 3.5f, DoJogo);

            investida.Atualizar(0.25f); // fim do avanço
            Assert.AreEqual(4.75f, investida.RecargaRestante, Tolerancia);
            Assert.IsFalse(investida.TentarIniciar(Vector3.forward, 3.5f, DoJogo), "em recarga");

            investida.Atualizar(4.5f);
            Assert.IsFalse(investida.TentarIniciar(Vector3.forward, 3.5f, DoJogo), "faltam 0,25 s");

            investida.Atualizar(0.25f);
            Assert.AreEqual(0f, investida.RecargaRestante);
            Assert.IsTrue(investida.TentarIniciar(Vector3.forward, 3.5f, DoJogo));
        }

        [Test]
        public void Cancelar_DescartaORestoDoAvanco_EMantemARecarga()
        {
            var investida = Liberada();
            investida.TentarIniciar(Vector3.forward, 3.5f, DoJogo);
            investida.Atualizar(0.1f);

            investida.Cancelar();

            Assert.IsFalse(investida.EmAndamento);
            Assert.AreEqual(Vector3.zero, investida.Velocidade);
            Assert.AreEqual(Vector3.zero, investida.Atualizar(0.1f), "o resto do avanço não volta");
            Assert.AreEqual(4.8f, investida.RecargaRestante, Tolerancia, "a recarga segue contando");
            Assert.IsFalse(investida.TentarIniciar(Vector3.forward, 3.5f, DoJogo), "em recarga");
        }

        [Test]
        public void Cancelar_SemInvestida_NaoMudaNada()
        {
            var investida = Liberada();

            investida.Cancelar();

            Assert.IsTrue(investida.Pronta);
            Assert.AreEqual(0f, investida.RecargaRestante);
        }

        [Test]
        public void DuranteOAvanco_NaoReinicia()
        {
            var investida = Liberada();
            investida.TentarIniciar(Vector3.forward, 3.5f, new ParametrosDeInvestida(5f, 0.25f, 0f));
            investida.Atualizar(0.1f);

            Assert.IsFalse(investida.TentarIniciar(Vector3.right, 3.5f, new ParametrosDeInvestida(5f, 0.25f, 0f)));
            Assert.AreEqual(0f, investida.Velocidade.x, Tolerancia, "a direção não muda no meio");
        }

        [Test]
        public void SemRecarga_PodeEmendarLogoDepois()
        {
            var investida = Liberada();
            var semRecarga = new ParametrosDeInvestida(5f, 0.25f, 0f);
            investida.TentarIniciar(Vector3.forward, 3.5f, semRecarga);
            investida.Atualizar(0.25f);

            Assert.IsTrue(investida.TentarIniciar(Vector3.forward, 3.5f, semRecarga));
        }

        // --- Direção ---

        [Test]
        public void CalcularDirecao_ComEntrada_UsaAEntrada()
        {
            var direcao = Investida.CalcularDirecao(new Vector2(0.5f, 0.5f), Vector3.forward, 0.01f);

            AssertVetor(new Vector3(1f, 0f, 1f).normalized, direcao);
        }

        [Test]
        public void CalcularDirecao_SemEntrada_UsaAFrenteHorizontal()
        {
            var direcao = Investida.CalcularDirecao(new Vector2(0.001f, 0f), new Vector3(-2f, 5f, 0f), 0.01f);

            AssertVetor(Vector3.left, direcao);
        }

        [Test]
        public void CalcularDirecao_SemEntradaEFrenteVertical_UsaMaisZ()
        {
            AssertVetor(Vector3.forward, Investida.CalcularDirecao(Vector2.zero, Vector3.up, 0.01f));
        }

        // --- Validação ---

        [Test]
        public void ValoresInvalidos_LancamExcecao()
        {
            var investida = Liberada();

            Assert.Throws<ArgumentException>(() => investida.Atualizar(-0.1f));
            Assert.Throws<ArgumentException>(() => investida.Atualizar(float.NaN));
            Assert.Throws<ArgumentException>(() => investida.TentarIniciar(new Vector3(float.NaN, 0f, 1f), 3.5f, DoJogo));
            Assert.Throws<ArgumentException>(() => investida.TentarIniciar(Vector3.forward, float.PositiveInfinity, DoJogo));
        }

        [Test]
        public void Parametros_ForaDaFaixa_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => new ParametrosDeInvestida(0f, 0.25f, 5f), "multiplicador zero");
            Assert.Throws<ArgumentException>(() => new ParametrosDeInvestida(5f, 0f, 5f), "duração zero");
            Assert.Throws<ArgumentException>(() => new ParametrosDeInvestida(5f, 0.25f, -1f), "recarga negativa");
            Assert.Throws<ArgumentException>(() => new ParametrosDeInvestida(float.NaN, 0.25f, 5f), "NaN");
        }

        [Test]
        public void Distancia_VelocidadeNegativaContaComoZero()
        {
            Assert.AreEqual(0f, DoJogo.Distancia(-3f));
        }
    }
}
