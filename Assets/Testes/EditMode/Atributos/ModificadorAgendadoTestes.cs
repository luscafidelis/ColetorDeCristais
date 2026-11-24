using System;
using ColetorDeCristais.Atributos;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Atributos
{
    /// <summary>
    /// Testes de <see cref="Atributo.AgendarAoFimDoGrupo"/>: o modificador agendado só entra
    /// quando o grupo fica sem temporários ativos. Os números são arbitrários, não são valores de jogo.
    /// </summary>
    public class ModificadorAgendadoTestes
    {
        private const float Tolerancia = 0.0001f;
        private const string Grupo = "grupo-de-teste";

        [Test]
        public void Agendado_NaoValeEnquantoOGrupoEstaAtivo()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(5f, duracao: 3f, grupo: Grupo));

            atributo.AgendarAoFimDoGrupo(Grupo, Modificador.Plano(1f));
            atributo.Atualizar(2f);

            Assert.AreEqual(15f, atributo.ValorFinal, Tolerancia);
            Assert.AreEqual(1, atributo.ModificadoresAgendados.Count);
        }

        [Test]
        public void Agendado_EntraQuandoOGrupoExpira()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(5f, duracao: 3f, grupo: Grupo));
            var agendado = Modificador.Plano(1f);
            atributo.AgendarAoFimDoGrupo(Grupo, agendado);

            atributo.Atualizar(3f);

            Assert.AreEqual(11f, atributo.ValorFinal, Tolerancia);
            Assert.AreEqual(0, atributo.ModificadoresAgendados.Count);
            CollectionAssert.Contains(atributo.Modificadores, agendado);
        }

        [Test]
        public void Agendado_SemGrupoAtivo_EntraNaHora()
        {
            var atributo = new Atributo(10f);

            atributo.AgendarAoFimDoGrupo(Grupo, Modificador.Plano(1f));

            Assert.AreEqual(11f, atributo.ValorFinal, Tolerancia);
            Assert.AreEqual(0, atributo.ModificadoresAgendados.Count);
        }

        [Test]
        public void Agendado_RenovarOGrupoAdiaOAgendado()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(5f, duracao: 3f, grupo: Grupo));
            atributo.AgendarAoFimDoGrupo(Grupo, Modificador.Plano(1f));

            atributo.Atualizar(2f);
            atributo.AdicionarModificador(Modificador.Plano(5f, duracao: 3f, grupo: Grupo)); // renova: 3 s de novo
            atributo.Atualizar(2f);

            Assert.AreEqual(20f, atributo.ValorFinal, Tolerancia, "ainda no efeito renovado, sem o agendado");

            atributo.Atualizar(1f);

            Assert.AreEqual(11f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void Agendado_OutroGrupoAtivoNaoSegura()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(5f, duracao: 3f, grupo: Grupo));
            atributo.AdicionarModificador(Modificador.Plano(2f, duracao: 10f, grupo: "outro"));
            atributo.AgendarAoFimDoGrupo(Grupo, Modificador.Plano(1f));

            atributo.Atualizar(3f);

            Assert.AreEqual(13f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void Agendado_EntraTambemQuandoOTemporarioERemovido()
        {
            var atributo = new Atributo(10f);
            var temporario = Modificador.Plano(5f, duracao: 3f, grupo: Grupo);
            atributo.AdicionarModificador(temporario);
            atributo.AgendarAoFimDoGrupo(Grupo, Modificador.Plano(1f));

            atributo.RemoverModificador(temporario);

            Assert.AreEqual(11f, atributo.ValorFinal, Tolerancia);
        }

        [Test]
        public void RemoverDaOrigem_DescartaOsAgendadosDaOrigem()
        {
            var atributo = new Atributo(10f);
            var origem = new object();
            atributo.AdicionarModificador(Modificador.Plano(5f, origem, 3f, Grupo));
            atributo.AgendarAoFimDoGrupo(Grupo, Modificador.Plano(1f, origem));

            var removidos = atributo.RemoverModificadoresDaOrigem(origem);

            Assert.AreEqual(1, removidos);
            Assert.AreEqual(10f, atributo.ValorFinal, Tolerancia);
            Assert.AreEqual(0, atributo.ModificadoresAgendados.Count);
        }

        [Test]
        public void RemoverTodos_DescartaOsAgendados()
        {
            var atributo = new Atributo(10f);
            atributo.AdicionarModificador(Modificador.Plano(5f, duracao: 3f, grupo: Grupo));
            atributo.AgendarAoFimDoGrupo(Grupo, Modificador.Plano(1f));

            atributo.RemoverTodosModificadores();
            atributo.Atualizar(5f);

            Assert.AreEqual(10f, atributo.ValorFinal, Tolerancia);
            Assert.AreEqual(0, atributo.ModificadoresAgendados.Count);
        }

        [Test]
        public void Agendar_ValoresInvalidos_LancamExcecao()
        {
            var atributo = new Atributo(10f);
            var modificador = Modificador.Plano(1f);

            Assert.Throws<ArgumentNullException>(() => atributo.AgendarAoFimDoGrupo(null, modificador));
            Assert.Throws<ArgumentException>(() => atributo.AgendarAoFimDoGrupo("  ", modificador));
            Assert.Throws<ArgumentNullException>(() => atributo.AgendarAoFimDoGrupo(Grupo, null));

            atributo.AdicionarModificador(Modificador.Plano(5f, duracao: 3f, grupo: Grupo));
            atributo.AgendarAoFimDoGrupo(Grupo, modificador);
            Assert.Throws<InvalidOperationException>(() => atributo.AgendarAoFimDoGrupo(Grupo, modificador), "já agendado");
        }
    }
}
