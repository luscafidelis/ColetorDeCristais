using System.Text.RegularExpressions;
using ColetorDeCristais.Cristais;
using ColetorDeCristais.Habilidades;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ColetorDeCristais.Testes.Cristais
{
    /// <summary>
    /// Testes do <see cref="EfeitoLiberarInvestida"/> (o efeito do café): libera a investida
    /// do jogador que coletou e registra erro quando ele não tem <see cref="InvestidaDoJogador"/>.
    /// </summary>
    public class EfeitoLiberarInvestidaTestes
    {
        private EfeitoLiberarInvestida _efeito;
        private GameObject _jogador;

        [SetUp]
        public void Preparar()
        {
            _efeito = ScriptableObject.CreateInstance<EfeitoLiberarInvestida>();
            _efeito.name = "EfeitoDeTeste";
            _jogador = new GameObject("JogadorDeTeste");
        }

        [TearDown]
        public void Limpar()
        {
            Object.DestroyImmediate(_efeito);
            Object.DestroyImmediate(_jogador);
        }

        [Test]
        public void Aplicar_LiberaAInvestidaDoColetor()
        {
            var coletor = _jogador.AddComponent<ColetorDoJogador>();
            var investida = _jogador.AddComponent<InvestidaDoJogador>(); // traz CharacterController e MovimentoDoJogador
            Assert.IsFalse(investida.Investida.Liberada);

            _efeito.Aplicar(coletor);

            Assert.IsTrue(investida.Investida.Liberada);
        }

        [Test]
        public void Aplicar_DuasVezes_ContinuaLiberada()
        {
            var coletor = _jogador.AddComponent<ColetorDoJogador>();
            var investida = _jogador.AddComponent<InvestidaDoJogador>();

            _efeito.Aplicar(coletor);
            _efeito.Aplicar(coletor);

            Assert.IsTrue(investida.Investida.Liberada);
        }

        [Test]
        public void Aplicar_ColetorSemInvestida_RegistraErro()
        {
            var coletor = _jogador.AddComponent<ColetorDoJogador>();

            LogAssert.Expect(LogType.Error, new Regex("não tem InvestidaDoJogador"));
            _efeito.Aplicar(coletor);
        }

        [Test]
        public void Aplicar_ColetorNulo_RegistraErro()
        {
            LogAssert.Expect(LogType.Error, new Regex("não tem InvestidaDoJogador"));
            _efeito.Aplicar(null);
        }
    }
}
