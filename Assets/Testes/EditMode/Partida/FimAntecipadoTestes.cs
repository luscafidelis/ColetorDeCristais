using System;
using System.Collections.Generic;
using ColetorDeCristais.Nivel;
using ColetorDeCristais.Partida;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Partida
{
    /// <summary>
    /// Testes do fim antecipado da onda: <see cref="RegraDoFimAntecipado"/> (o que conta como "tudo coletado") e
    /// <see cref="CicloDaPartida.EncerrarOnda"/> (a onda acaba e vem o chefe).
    /// </summary>
    public class FimAntecipadoTestes
    {
        // A área bônus do mercado: x de -10 a 10, z de 7 a 10.
        private static readonly IReadOnlyList<Retangulo> AreaBonus = new[] { new Retangulo(new Vector2(0f, 8.5f), new Vector2(20f, 3f)) };

        [Test]
        public void SemItens_TudoColetado()
        {
            Assert.IsTrue(RegraDoFimAntecipado.TudoColetado(new Vector2[0], 0, AreaBonus));
        }

        [Test]
        public void ItemDesteLadoDoVao_Falta()
        {
            Assert.IsFalse(RegraDoFimAntecipado.TudoColetado(new[] { new Vector2(0f, 2.3f) }, 0, AreaBonus));
        }

        [Test]
        public void SoItensNaAreaBonus_TudoColetado()
        {
            var bonus = new[] { new Vector2(0f, 8.6f), new Vector2(-9f, 9.2f), new Vector2(9f, 8.2f) };
            Assert.IsTrue(RegraDoFimAntecipado.TudoColetado(bonus, 0, AreaBonus));
        }

        [Test]
        public void ItemNaBordaDaAreaBonus_NaoConta()
        {
            Assert.IsTrue(RegraDoFimAntecipado.TudoColetado(new[] { new Vector2(0f, 7f) }, 0, AreaBonus));
        }

        [Test]
        public void ItemCaindo_Falta()
        {
            Assert.IsFalse(RegraDoFimAntecipado.TudoColetado(new Vector2[0], 1, AreaBonus));
        }

        [Test]
        public void SemAreasIgnoradas_TodosContam()
        {
            Assert.IsFalse(RegraDoFimAntecipado.TudoColetado(new[] { new Vector2(0f, 8.6f) }, 0, null));
        }

        [Test]
        public void AreaInvalida_EIgnorada()
        {
            var invalida = new[] { default(Retangulo) }; // tamanho zero, como um item novo de lista no Inspector
            Assert.IsFalse(RegraDoFimAntecipado.TudoColetado(new[] { new Vector2(0f, 8.5f) }, 0, invalida));
        }

        [Test]
        public void PosicoesNulas_Lanca()
        {
            Assert.Throws<ArgumentNullException>(() => RegraDoFimAntecipado.TudoColetado(null, 0, AreaBonus));
        }

        [Test]
        public void EncerrarOnda_NaUnicaOnda_VemOChefeDaArena()
        {
            var ciclo = new CicloDaPartida(new ParametrosDaPartida(120f, 1));
            ciclo.Atualizar(30f);

            Assert.IsTrue(ciclo.EncerrarOnda());

            Assert.AreEqual(EstadoDaPartida.ChefeDaArena, ciclo.Estado);
            Assert.AreEqual(0f, ciclo.TempoRestante);
        }

        [Test]
        public void EncerrarOnda_AntesDaUltima_VemOChefeDaOnda()
        {
            var ciclo = new CicloDaPartida(new ParametrosDaPartida(120f, 3));

            Assert.IsTrue(ciclo.EncerrarOnda());

            Assert.AreEqual(EstadoDaPartida.ChefeDaOnda, ciclo.Estado);
            Assert.IsTrue(ciclo.DerrotarChefe());
            Assert.AreEqual(2, ciclo.Onda);
            Assert.AreEqual(120f, ciclo.TempoRestante);
        }

        [Test]
        public void EncerrarOnda_ForaDeUmaOnda_NaoFazNada()
        {
            var ciclo = new CicloDaPartida(new ParametrosDaPartida(120f, 1));
            ciclo.EncerrarOnda();

            Assert.IsFalse(ciclo.EncerrarOnda());
            ciclo.DerrotarJogador();
            Assert.IsFalse(ciclo.EncerrarOnda());
            Assert.AreEqual(EstadoDaPartida.Derrota, ciclo.Estado);
        }
    }
}
