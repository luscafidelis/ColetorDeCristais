using ColetorDeCristais.Cristais;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Cristais
{
    /// <summary>Testes da regra de coleta (<see cref="EstadoDeColeta"/>).</summary>
    public class EstadoDeColetaTestes
    {
        [Test]
        public void NovoCristal_NaoEstaColetado()
        {
            Assert.IsFalse(new EstadoDeColeta().Coletado);
        }

        [Test]
        public void ColetorValido_ColetaNaPrimeiraVez()
        {
            var estado = new EstadoDeColeta();

            Assert.IsTrue(estado.TentarColetar(coletorValido: true));
            Assert.IsTrue(estado.Coletado);
        }

        [Test]
        public void ColetorInvalido_NaoColeta()
        {
            var estado = new EstadoDeColeta();

            Assert.IsFalse(estado.TentarColetar(coletorValido: false));
            Assert.IsFalse(estado.Coletado);
        }

        [Test]
        public void ColetorInvalidoDepoisValido_ColetaNoValido()
        {
            var estado = new EstadoDeColeta();
            estado.TentarColetar(coletorValido: false);

            Assert.IsTrue(estado.TentarColetar(coletorValido: true));
        }

        [Test]
        public void SegundaTentativa_NaoColetaDeNovo()
        {
            var estado = new EstadoDeColeta();
            estado.TentarColetar(coletorValido: true);

            Assert.IsFalse(estado.TentarColetar(coletorValido: true));
            Assert.IsTrue(estado.Coletado);
        }
    }
}
