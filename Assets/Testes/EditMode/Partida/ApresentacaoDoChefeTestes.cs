using ColetorDeCristais.Partida;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Partida
{
    /// <summary>
    /// Testes de quando o chefe aparece (<see cref="ApresentacaoDoChefe.DeveAparecer"/>), com a partida do mercado
    /// (1 onda de 120 s) e a de 3 ondas.
    /// </summary>
    public class ApresentacaoDoChefeTestes
    {
        private static readonly ParametrosDaPartida DoMercado = new ParametrosDaPartida(120f, 1);
        private static readonly ParametrosDaPartida DeTresOndas = new ParametrosDaPartida(120f, 3);

        [Test]
        public void SemPartida_NaoAparece()
        {
            Assert.IsFalse(ApresentacaoDoChefe.DeveAparecer(null));
        }

        [Test]
        public void DuranteAOnda_NaoAparece()
        {
            var ciclo = new CicloDaPartida(DoMercado);
            ciclo.Atualizar(60f);

            Assert.IsFalse(ApresentacaoDoChefe.DeveAparecer(ciclo));
        }

        [Test]
        public void FimDoTempoNoMercado_ApareceOChefeDaArena()
        {
            var ciclo = new CicloDaPartida(DoMercado);
            ciclo.Atualizar(120f);

            Assert.AreEqual(EstadoDaPartida.ChefeDaArena, ciclo.Estado);
            Assert.IsTrue(ApresentacaoDoChefe.DeveAparecer(ciclo));
        }

        [Test]
        public void FimAntecipado_Aparece()
        {
            var ciclo = new CicloDaPartida(DoMercado);
            ciclo.Atualizar(30f);
            Assert.IsTrue(ciclo.EncerrarOnda());

            Assert.IsTrue(ApresentacaoDoChefe.DeveAparecer(ciclo));
        }

        [Test]
        public void ChefeDerrotado_Some()
        {
            var ciclo = new CicloDaPartida(DoMercado);
            ciclo.Atualizar(120f);
            ciclo.DerrotarChefe();

            Assert.AreEqual(EstadoDaPartida.Vitoria, ciclo.Estado);
            Assert.IsFalse(ApresentacaoDoChefe.DeveAparecer(ciclo));
        }

        [Test]
        public void Derrota_NaoAparece()
        {
            var ciclo = new CicloDaPartida(DoMercado);
            ciclo.DerrotarJogador();

            Assert.IsFalse(ApresentacaoDoChefe.DeveAparecer(ciclo));
        }

        [Test]
        public void ChefeDaOnda_ApareceESomeNaProximaOnda()
        {
            var ciclo = new CicloDaPartida(DeTresOndas);
            ciclo.Atualizar(120f);
            Assert.AreEqual(EstadoDaPartida.ChefeDaOnda, ciclo.Estado);
            Assert.IsTrue(ApresentacaoDoChefe.DeveAparecer(ciclo));

            ciclo.DerrotarChefe();
            Assert.IsFalse(ApresentacaoDoChefe.DeveAparecer(ciclo));
        }
    }
}
