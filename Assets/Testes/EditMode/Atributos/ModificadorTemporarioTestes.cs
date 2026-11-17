using System;
using ColetorDeCristais.Atributos;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Atributos
{
    /// <summary>
    /// Testes de modificadores temporários no <see cref="Atributo"/>: expiração por tempo,
    /// convivência com permanentes e renovação por grupo (o tempo volta ao início e os efeitos se somam).
    /// Os números usados são arbitrários; não representam valores de jogo.
    /// </summary>
    public class ModificadorTemporarioTestes
    {
        private const float Tolerancia = 0.0001f;
        private const string GrupoDeTeste = "grupo-de-teste";

        // --- Expiração ---

        [Test]
        public void Temporario_AplicaEnquantoNaoExpira()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(5f, duracao: 3f));

            var expirados = atributo.Atualizar(2f);

            Assert.AreEqual(0, expirados);
            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void Temporario_ExpiraExatamenteNaDuracao()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(5f, duracao: 3f));

            var expirados = atributo.Atualizar(3f);

            Assert.AreEqual(1, expirados);
            Assert.AreEqual(10f, atributo.ValorFinal, Tolerancia);
            Assert.IsEmpty(atributo.Modificadores);
        }

        [Test]
        public void Temporario_ExpiraSomandoVariasAtualizacoes()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(5f, duracao: 2f));

            atributo.Atualizar(0.5f);
            atributo.Atualizar(1f);
            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);

            atributo.Atualizar(0.5f);
            Assert.AreEqual(10f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void AtualizarComTempoMaiorQueADuracao_RemoveDeUmaVez()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(1f, duracao: 1f));
            atributo.AdicionarModificador(Modificador.Plano(2f, duracao: 2f));

            var expirados = atributo.Atualizar(100f);

            Assert.AreEqual(2, expirados);
            Assert.AreEqual(10f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void Permanente_NaoExpiraComOTempo()
        {
            var atributo = new Atributo(10f);
            var permanente = Modificador.Plano(5f);
            atributo.AdicionarModificador(permanente);
            atributo.AdicionarModificador(Modificador.Plano(1f, duracao: 1f));

            atributo.Atualizar(1000f);

            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);
            Assert.AreSame(permanente, atributo.Modificadores[0]);
            Assert.IsFalse(atributo.TentarObterTempoRestante(permanente, out _));
        }

        [Test]
        public void TemporariosSemGrupo_TemTemposIndependentes()
        {
            var atributo = new Atributo(10f);
            var primeiro = Modificador.Plano(1f, duracao: 3f);
            atributo.AdicionarModificador(primeiro);
            atributo.Atualizar(2f);
            var segundo = Modificador.Plano(1f, duracao: 3f);

            atributo.AdicionarModificador(segundo);

            Assert.IsTrue(atributo.TentarObterTempoRestante(primeiro, out var restantePrimeiro));
            Assert.AreEqual(1f, restantePrimeiro, Tolerancia);
            atributo.Atualizar(1f);
            Assert.AreEqual(11f, atributo.ValorFinal, Tolerancia);
            Assert.AreSame(segundo, atributo.Modificadores[0]);
        }

        [Test]
        public void AtualizarComZero_NaoMudaNada()
        {
            var atributo = new Atributo(10f);
            var temporario = Modificador.Plano(5f, duracao: 1f);
            atributo.AdicionarModificador(temporario);

            Assert.AreEqual(0, atributo.Atualizar(0f));
            Assert.IsTrue(atributo.TentarObterTempoRestante(temporario, out var restante));
            Assert.AreEqual(1f, restante, Tolerancia);
        }

        [TestCase(-1f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        public void AtualizarComTempoInvalido_LancaExcecao(float tempoInvalido)
        {
            var atributo = new Atributo(10f);

            Assert.Throws<ArgumentException>(() => atributo.Atualizar(tempoInvalido));
        }

        [Test]
        public void RemoverTemporarioAntesDeExpirar_FuncionaEApagaOTempo()
        {
            var atributo = new Atributo(10f);
            var temporario = Modificador.Plano(5f, duracao: 3f);
            atributo.AdicionarModificador(temporario);

            Assert.IsTrue(atributo.RemoverModificador(temporario));

            Assert.AreEqual(10f, atributo.ValorFinal, Tolerancia);
            Assert.IsFalse(atributo.TentarObterTempoRestante(temporario, out _));
            Assert.AreEqual(0, atributo.Atualizar(10f));
        }

        [Test]
        public void TemporarioRemovidoPorOrigem_NaoContinuaContandoTempo()
        {
            var atributo = new Atributo(10f);
            var origem = new object();
            var temporario = Modificador.Plano(5f, origem, duracao: 3f);
            atributo.AdicionarModificador(temporario);

            atributo.RemoverModificadoresDaOrigem(origem);

            Assert.IsFalse(atributo.TentarObterTempoRestante(temporario, out _));
        }

        [Test]
        public void TemporarioReadicionadoDepoisDeExpirar_ComecaDoZero()
        {
            var atributo = new Atributo(10f);
            var temporario = Modificador.Plano(5f, duracao: 2f);
            atributo.AdicionarModificador(temporario);
            atributo.Atualizar(2f);

            atributo.AdicionarModificador(temporario);

            Assert.IsTrue(atributo.TentarObterTempoRestante(temporario, out var restante));
            Assert.AreEqual(2f, restante, Tolerancia);
            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);
        }

        // --- Renovação por grupo ---

        [Test]
        public void MesmoGrupo_SomaOsEfeitos()
        {
            var atributo = new Atributo(10f);

            atributo.AdicionarModificador(Modificador.Plano(2f, duracao: 5f, grupo: GrupoDeTeste));
            atributo.AdicionarModificador(Modificador.Plano(2f, duracao: 5f, grupo: GrupoDeTeste));

            Assert.AreEqual(14f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void MesmoGrupo_NovoModificadorResetaOTempoDosAnteriores()
        {
            var atributo = new Atributo(10f);
            var primeiro = Modificador.Plano(2f, duracao: 5f, grupo: GrupoDeTeste);
            atributo.AdicionarModificador(primeiro);
            atributo.Atualizar(4f);

            var segundo = Modificador.Plano(2f, duracao: 5f, grupo: GrupoDeTeste);
            atributo.AdicionarModificador(segundo);

            Assert.IsTrue(atributo.TentarObterTempoRestante(primeiro, out var restantePrimeiro));
            Assert.AreEqual(5f, restantePrimeiro, Tolerancia);
            Assert.IsTrue(atributo.TentarObterTempoRestante(segundo, out var restanteSegundo));
            Assert.AreEqual(5f, restanteSegundo, Tolerancia);
        }

        [Test]
        public void MesmoGrupo_ExpiramJuntosDepoisDoReset()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(2f, duracao: 5f, grupo: GrupoDeTeste));
            atributo.Atualizar(4f);
            atributo.AdicionarModificador(Modificador.Plano(2f, duracao: 5f, grupo: GrupoDeTeste));

            atributo.Atualizar(4f);
            Assert.AreEqual(14f, atributo.ValorFinal, Tolerancia);

            var expirados = atributo.Atualizar(1f);
            Assert.AreEqual(2, expirados);
            Assert.AreEqual(10f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void MesmoGrupo_ResetUsaADuracaoDoNovoModificador()
        {
            var atributo = new Atributo(10f);
            var primeiro = Modificador.Plano(2f, duracao: 5f, grupo: GrupoDeTeste);
            atributo.AdicionarModificador(primeiro);

            atributo.AdicionarModificador(Modificador.Plano(2f, duracao: 3f, grupo: GrupoDeTeste));

            Assert.IsTrue(atributo.TentarObterTempoRestante(primeiro, out var restante));
            Assert.AreEqual(3f, restante, Tolerancia);
        }

        [Test]
        public void GruposDiferentes_NaoSeAfetam()
        {
            var atributo = new Atributo(10f);
            var doGrupoA = Modificador.Plano(1f, duracao: 5f, grupo: "a");
            atributo.AdicionarModificador(doGrupoA);
            atributo.Atualizar(4f);

            atributo.AdicionarModificador(Modificador.Plano(1f, duracao: 5f, grupo: "b"));

            Assert.IsTrue(atributo.TentarObterTempoRestante(doGrupoA, out var restante));
            Assert.AreEqual(1f, restante, Tolerancia);
        }

        [Test]
        public void Grupo_NaoAfetaTemporariosSemGrupoNemPermanentes()
        {
            var atributo = new Atributo(10f);
            var semGrupo = Modificador.Plano(1f, duracao: 5f);
            var permanente = Modificador.Plano(1f);
            atributo.AdicionarModificador(semGrupo);
            atributo.AdicionarModificador(permanente);
            atributo.Atualizar(4f);

            atributo.AdicionarModificador(Modificador.Plano(1f, duracao: 5f, grupo: GrupoDeTeste));

            Assert.IsTrue(atributo.TentarObterTempoRestante(semGrupo, out var restante));
            Assert.AreEqual(1f, restante, Tolerancia);
            Assert.IsFalse(atributo.TentarObterTempoRestante(permanente, out _));
        }

        [Test]
        public void TentarObterTempoRestante_DeModificadorAusenteOuNulo_RetornaFalso()
        {
            var atributo = new Atributo(10f);

            Assert.IsFalse(atributo.TentarObterTempoRestante(Modificador.Plano(1f, duracao: 1f), out var restante));
            Assert.AreEqual(0f, restante);
            Assert.IsFalse(atributo.TentarObterTempoRestante(null, out _));
        }
    }
}
