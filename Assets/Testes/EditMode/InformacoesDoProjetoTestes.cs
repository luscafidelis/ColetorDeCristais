using NUnit.Framework;

namespace ColetorDeCristais.Testes
{
    /// <summary>
    /// Testes EditMode que confirmam que a estrutura de testes do projeto funciona:
    /// o assembly de testes enxerga o assembly do jogo e o Test Framework roda pela CLI.
    /// </summary>
    public class InformacoesDoProjetoTestes
    {
        [Test]
        public void Nome_EhColetorDeCristais()
        {
            Assert.AreEqual("Coletor de Cristais", InformacoesDoProjeto.Nome);
        }

        [Test]
        public void Nome_NaoTemEspacosNasPontas()
        {
            Assert.AreEqual(InformacoesDoProjeto.Nome.Trim(), InformacoesDoProjeto.Nome);
        }
    }
}
