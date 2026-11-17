using System;
using ColetorDeCristais.Atributos;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Atributos
{
    /// <summary>
    /// Testes do <see cref="Atributo"/>: valor base, adição e remoção de modificadores,
    /// ordem de aplicação dos tipos, limites e casos de borda.
    /// Os números usados são arbitrários; não representam valores de jogo.
    /// </summary>
    public class AtributoTestes
    {
        private const float Tolerancia = 0.0001f;

        // --- Valor base ---

        [Test]
        public void SemModificadores_ValorFinalEhOValorBase()
        {
            var atributo = new Atributo(10f);

            Assert.AreEqual(10f, atributo.ValorFinal, Tolerancia);
            Assert.IsEmpty(atributo.Modificadores);
        }

        [Test]
        public void AlterarValorBase_AtualizaValorFinal()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(2f));
            Assert.AreEqual(12f, atributo.ValorFinal, Tolerancia);

            atributo.ValorBase = 20f;

            Assert.AreEqual(22f, atributo.ValorFinal, Tolerancia);
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void ValorBaseNaoFinito_LancaExcecao(float valorInvalido)
        {
            Assert.Throws<ArgumentException>(() => new Atributo(valorInvalido));

            var atributo = new Atributo(1f);
            Assert.Throws<ArgumentException>(() => atributo.ValorBase = valorInvalido);
            Assert.AreEqual(1f, atributo.ValorBase, Tolerancia);
        }

        // --- Adicionar e remover ---

        [Test]
        public void AdicionarModificadorPlano_SomaAoBase()
        {
            var atributo = new Atributo(10f);

            atributo.AdicionarModificador(Modificador.Plano(5f));

            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);
            Assert.AreEqual(1, atributo.Modificadores.Count);
        }

        [Test]
        public void AdicionarModificadorPercentualAditivo_AplicaFracao()
        {
            var atributo = new Atributo(10f);

            atributo.AdicionarModificador(Modificador.PercentualAditivo(0.5f));

            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void AdicionarModificadorPercentualMultiplicativo_AplicaFracao()
        {
            var atributo = new Atributo(10f);

            atributo.AdicionarModificador(Modificador.PercentualMultiplicativo(0.5f));

            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void RemoverModificador_VoltaAoValorAnterior()
        {
            var atributo = new Atributo(10f);
            var modificador = Modificador.Plano(5f);
            atributo.AdicionarModificador(modificador);

            var removeu = atributo.RemoverModificador(modificador);

            Assert.IsTrue(removeu);
            Assert.AreEqual(10f, atributo.ValorFinal, Tolerancia);
            Assert.IsEmpty(atributo.Modificadores);
        }

        [Test]
        public void RemoverModificadorQueNaoEstaNoAtributo_RetornaFalsoENaoMudaNada()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(5f));

            Assert.IsFalse(atributo.RemoverModificador(Modificador.Plano(5f)));
            Assert.IsFalse(atributo.RemoverModificador(null));
            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void RemoverUmDeDoisModificadoresIguais_RemoveSoAInstanciaInformada()
        {
            var atributo = new Atributo(10f);
            var primeiro = Modificador.Plano(5f);
            var segundo = Modificador.Plano(5f);
            atributo.AdicionarModificador(primeiro);
            atributo.AdicionarModificador(segundo);

            atributo.RemoverModificador(primeiro);

            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);
            Assert.AreSame(segundo, atributo.Modificadores[0]);
        }

        [Test]
        public void AdicionarAMesmaInstanciaDuasVezes_LancaExcecao()
        {
            var atributo = new Atributo(10f);
            var modificador = Modificador.Plano(5f);
            atributo.AdicionarModificador(modificador);

            Assert.Throws<InvalidOperationException>(() => atributo.AdicionarModificador(modificador));
            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void AdicionarNulo_LancaExcecao()
        {
            var atributo = new Atributo(10f);

            Assert.Throws<ArgumentNullException>(() => atributo.AdicionarModificador(null));
        }

        [Test]
        public void RemoverModificadoresDaOrigem_RemoveSoOsDaquelaOrigem()
        {
            var atributo = new Atributo(10f);
            var origemA = new object();
            var origemB = new object();
            atributo.AdicionarModificador(Modificador.Plano(1f, origemA));
            atributo.AdicionarModificador(Modificador.PercentualAditivo(1f, origemA));
            atributo.AdicionarModificador(Modificador.Plano(2f, origemB));

            var removidos = atributo.RemoverModificadoresDaOrigem(origemA);

            Assert.AreEqual(2, removidos);
            Assert.AreEqual(12f, atributo.ValorFinal, Tolerancia);
            Assert.AreEqual(0, atributo.RemoverModificadoresDaOrigem(null));
            Assert.AreEqual(0, atributo.RemoverModificadoresDaOrigem(new object()));
        }

        [Test]
        public void RemoverTodosModificadores_VoltaAoValorBase()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(1f));
            atributo.AdicionarModificador(Modificador.PercentualMultiplicativo(1f));

            atributo.RemoverTodosModificadores();

            Assert.AreEqual(10f, atributo.ValorFinal, Tolerancia);
            Assert.IsEmpty(atributo.Modificadores);
        }

        // --- Combinação de tipos e ordem de aplicação ---

        [Test]
        public void PercentuaisAditivos_SeSomamAntesDeMultiplicar()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.PercentualAditivo(0.1f));
            atributo.AdicionarModificador(Modificador.PercentualAditivo(0.1f));

            // 10 × (1 + 0,1 + 0,1) = 12
            Assert.AreEqual(12f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void PercentuaisMultiplicativos_SeMultiplicamEntreSi()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.PercentualMultiplicativo(0.1f));
            atributo.AdicionarModificador(Modificador.PercentualMultiplicativo(0.1f));

            // 10 × 1,1 × 1,1 = 12,1
            Assert.AreEqual(12.1f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void TodosOsTipos_AplicamPlanoDepoisAditivoDepoisMultiplicativo()
        {
            var atributo = new Atributo(10f);
            // Adicionados fora de ordem, para provar que a ordem não depende da inserção.
            atributo.AdicionarModificador(Modificador.PercentualMultiplicativo(1f));
            atributo.AdicionarModificador(Modificador.PercentualAditivo(0.5f));
            atributo.AdicionarModificador(Modificador.Plano(2f));

            // (10 + 2) × (1 + 0,5) × (1 + 1) = 36
            Assert.AreEqual(36f, atributo.ValorFinal, Tolerancia);
        }

        // --- Casos de borda: negativos e limites ---

        [Test]
        public void ModificadorPlanoNegativo_ReduzOValor()
        {
            var atributo = new Atributo(10f);

            atributo.AdicionarModificador(Modificador.Plano(-4f));

            Assert.AreEqual(6f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void SemLimites_ValorFinalPodeFicarNegativo()
        {
            var atributo = new Atributo(10f);

            atributo.AdicionarModificador(Modificador.Plano(-15f));

            Assert.AreEqual(-5f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void ValorBaseNegativo_EhAceitoEModificado()
        {
            var atributo = new Atributo(-10f);

            atributo.AdicionarModificador(Modificador.PercentualAditivo(0.5f));

            Assert.AreEqual(-15f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void PercentualMenosCemPorCento_ZeraOValor()
        {
            var atributo = new Atributo(10f);

            atributo.AdicionarModificador(Modificador.PercentualMultiplicativo(-1f));

            Assert.AreEqual(0f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void ValorMinimo_LimitaOValorFinal()
        {
            var atributo = new Atributo(10f, valorMinimo: 0f);

            atributo.AdicionarModificador(Modificador.PercentualAditivo(-2f));

            Assert.AreEqual(0f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void ValorMaximo_LimitaOValorFinal()
        {
            var atributo = new Atributo(10f, valorMaximo: 12f);

            atributo.AdicionarModificador(Modificador.Plano(5f));

            Assert.AreEqual(12f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void Limites_NaoAlteramOValorBase()
        {
            var atributo = new Atributo(20f, valorMinimo: 0f, valorMaximo: 5f);

            Assert.AreEqual(20f, atributo.ValorBase, Tolerancia);
            Assert.AreEqual(5f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void MinimoMaiorQueMaximo_LancaExcecao()
        {
            Assert.Throws<ArgumentException>(() => new Atributo(1f, valorMinimo: 5f, valorMaximo: 1f));
        }

        [Test]
        public void LimiteNaN_LancaExcecao()
        {
            Assert.Throws<ArgumentException>(() => new Atributo(1f, valorMinimo: float.NaN));
            Assert.Throws<ArgumentException>(() => new Atributo(1f, valorMaximo: float.NaN));
        }
    }
}
