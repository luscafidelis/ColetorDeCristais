using System;
using ColetorDeCristais.Atributos;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Atributos
{
    /// <summary>Testes de criação e validação do <see cref="Modificador"/>.</summary>
    public class ModificadorTestes
    {
        [Test]
        public void Atalhos_CriamOTipoCertoComValorEOrigem()
        {
            var origem = new object();

            var plano = Modificador.Plano(1f, origem);
            var aditivo = Modificador.PercentualAditivo(0.2f, origem);
            var multiplicativo = Modificador.PercentualMultiplicativo(0.3f);

            Assert.AreEqual(TipoModificador.Plano, plano.Tipo);
            Assert.AreEqual(1f, plano.Valor);
            Assert.AreSame(origem, plano.Origem);
            Assert.AreEqual(TipoModificador.PercentualAditivo, aditivo.Tipo);
            Assert.AreEqual(0.2f, aditivo.Valor);
            Assert.AreEqual(TipoModificador.PercentualMultiplicativo, multiplicativo.Tipo);
            Assert.IsNull(multiplicativo.Origem);
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void ValorNaoFinito_LancaExcecao(float valorInvalido)
        {
            Assert.Throws<ArgumentException>(() => Modificador.Plano(valorInvalido));
        }

        [Test]
        public void SemDuracao_EhPermanente()
        {
            var modificador = Modificador.Plano(1f);

            Assert.IsFalse(modificador.EhTemporario);
            Assert.IsNull(modificador.Duracao);
            Assert.IsNull(modificador.Grupo);
        }

        [Test]
        public void ComDuracaoEGrupo_EhTemporario()
        {
            var modificador = Modificador.PercentualAditivo(0.5f, duracao: 2f, grupo: "g");

            Assert.IsTrue(modificador.EhTemporario);
            Assert.AreEqual(2f, modificador.Duracao);
            Assert.AreEqual("g", modificador.Grupo);
        }

        [TestCase(0f)]
        [TestCase(-1f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        public void DuracaoInvalida_LancaExcecao(float duracaoInvalida)
        {
            Assert.Throws<ArgumentException>(() => Modificador.Plano(1f, duracao: duracaoInvalida));
        }

        [Test]
        public void GrupoSemDuracao_LancaExcecao()
        {
            Assert.Throws<ArgumentException>(() => Modificador.Plano(1f, grupo: "g"));
        }

        [Test]
        public void GrupoVazio_EhTratadoComoSemGrupo()
        {
            var modificador = Modificador.Plano(1f, duracao: 1f, grupo: "");

            Assert.IsNull(modificador.Grupo);
        }

        [Test]
        public void TipoDesconhecido_LancaExcecao()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Modificador(1f, (TipoModificador)99));
        }
    }
}
