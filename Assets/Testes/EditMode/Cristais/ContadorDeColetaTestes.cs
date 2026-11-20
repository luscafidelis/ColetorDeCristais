using System;
using ColetorDeCristais.Cristais;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Cristais
{
    /// <summary>Testes da contagem de cristais coletados (<see cref="ContadorDeColeta"/>).</summary>
    public class ContadorDeColetaTestes
    {
        [Test]
        public void NovoContador_EstaZerado()
        {
            var contador = new ContadorDeColeta();

            Assert.AreEqual(0, contador.Total);
            Assert.AreEqual(0, contador.Quantidade("qualquer"));
        }

        [Test]
        public void Registrar_SomaNoTotalENoTipo()
        {
            var contador = new ContadorDeColeta();

            contador.Registrar("a");
            contador.Registrar("a");
            contador.Registrar("b");

            Assert.AreEqual(3, contador.Total);
            Assert.AreEqual(2, contador.Quantidade("a"));
            Assert.AreEqual(1, contador.Quantidade("b"));
            Assert.AreEqual(0, contador.Quantidade("c"));
        }

        [Test]
        public void Identificadores_DiferenciamMaiusculas()
        {
            var contador = new ContadorDeColeta();

            contador.Registrar("Cristal");

            Assert.AreEqual(0, contador.Quantidade("cristal"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void IdentificadorVazio_LancaExcecaoENaoConta(string identificadorInvalido)
        {
            var contador = new ContadorDeColeta();

            Assert.Throws<ArgumentException>(() => contador.Registrar(identificadorInvalido));
            Assert.AreEqual(0, contador.Total);
        }

        [Test]
        public void QuantidadeDeNulo_RetornaZero()
        {
            Assert.AreEqual(0, new ContadorDeColeta().Quantidade(null));
        }

        [Test]
        public void Zerar_LimpaTotalETipos()
        {
            var contador = new ContadorDeColeta();
            contador.Registrar("a");
            contador.Registrar("b");

            contador.Zerar();

            Assert.AreEqual(0, contador.Total);
            Assert.AreEqual(0, contador.Quantidade("a"));
        }
    }
}
