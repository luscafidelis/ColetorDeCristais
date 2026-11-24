using System;
using ColetorDeCristais.Corpo;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Corpo
{
    /// <summary>Testes da <see cref="Invulnerabilidade"/>: duração e pisca-pisca.</summary>
    public class InvulnerabilidadeTestes
    {
        [Test]
        public void ComecaDesligada_EVisivel()
        {
            var invulnerabilidade = new Invulnerabilidade();
            Assert.IsFalse(invulnerabilidade.Ativa);
            Assert.IsTrue(invulnerabilidade.Visivel(10f));
        }

        [Test]
        public void DuraOTempoDado()
        {
            var invulnerabilidade = new Invulnerabilidade();
            invulnerabilidade.Iniciar(1f);

            invulnerabilidade.Atualizar(0.99f);
            Assert.IsTrue(invulnerabilidade.Ativa);
            invulnerabilidade.Atualizar(0.02f);
            Assert.IsFalse(invulnerabilidade.Ativa);
        }

        [Test]
        public void Pisca_AlternaACadaMeioCiclo()
        {
            var invulnerabilidade = new Invulnerabilidade();
            invulnerabilidade.Iniciar(1f);

            // 10 piscadas por segundo: aparece de 0 a 0,05 s, some de 0,05 a 0,1 s, aparece de novo...
            Assert.IsTrue(invulnerabilidade.Visivel(10f));
            invulnerabilidade.Atualizar(0.06f);
            Assert.IsFalse(invulnerabilidade.Visivel(10f));
            invulnerabilidade.Atualizar(0.05f);
            Assert.IsTrue(invulnerabilidade.Visivel(10f));
        }

        [Test]
        public void SemPiscadas_FicaVisivel()
        {
            var invulnerabilidade = new Invulnerabilidade();
            invulnerabilidade.Iniciar(1f);
            invulnerabilidade.Atualizar(0.06f);

            Assert.IsTrue(invulnerabilidade.Visivel(0f));
        }

        [Test]
        public void AoAcabar_VoltaAAparecer()
        {
            var invulnerabilidade = new Invulnerabilidade();
            invulnerabilidade.Iniciar(0.1f);
            invulnerabilidade.Atualizar(0.06f);
            Assert.IsFalse(invulnerabilidade.Visivel(10f));

            invulnerabilidade.Atualizar(0.06f);

            Assert.IsTrue(invulnerabilidade.Visivel(10f));
        }

        [Test]
        public void Encerrar_DesligaNaHora()
        {
            var invulnerabilidade = new Invulnerabilidade();
            invulnerabilidade.Iniciar(1f);

            invulnerabilidade.Encerrar();

            Assert.IsFalse(invulnerabilidade.Ativa);
        }

        [Test]
        public void ValoresInvalidos_Lancam()
        {
            var invulnerabilidade = new Invulnerabilidade();
            Assert.Throws<ArgumentException>(() => invulnerabilidade.Iniciar(-1f));
            Assert.Throws<ArgumentException>(() => invulnerabilidade.Iniciar(float.NaN));
            Assert.Throws<ArgumentException>(() => invulnerabilidade.Atualizar(-0.1f));
        }
    }
}
