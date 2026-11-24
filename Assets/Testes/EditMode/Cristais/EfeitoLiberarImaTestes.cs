using System.Text.RegularExpressions;
using ColetorDeCristais.Cristais;
using ColetorDeCristais.Habilidades;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ColetorDeCristais.Testes.Cristais
{
    /// <summary>
    /// Testes do <see cref="EfeitoLiberarIma"/> (o efeito da rosquinha): libera o ímã do jogador
    /// que coletou e registra erro quando ele não tem <see cref="ImaDoJogador"/>.
    /// </summary>
    public class EfeitoLiberarImaTestes
    {
        private EfeitoLiberarIma _efeito;
        private GameObject _jogador;

        [SetUp]
        public void Preparar()
        {
            _efeito = ScriptableObject.CreateInstance<EfeitoLiberarIma>();
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
        public void Aplicar_LiberaOImaDoColetor()
        {
            var coletor = _jogador.AddComponent<ColetorDoJogador>();
            var ima = _jogador.AddComponent<ImaDoJogador>();
            Assert.IsFalse(ima.Ima.Liberada);

            _efeito.Aplicar(coletor);
            _efeito.Aplicar(coletor);

            Assert.IsTrue(ima.Ima.Liberada);
        }

        [Test]
        public void Aplicar_ColetorSemIma_RegistraErro()
        {
            var coletor = _jogador.AddComponent<ColetorDoJogador>();

            LogAssert.Expect(LogType.Error, new Regex("não tem ImaDoJogador"));
            _efeito.Aplicar(coletor);
        }

        [Test]
        public void Aplicar_ColetorNulo_RegistraErro()
        {
            LogAssert.Expect(LogType.Error, new Regex("não tem ImaDoJogador"));
            _efeito.Aplicar(null);
        }
    }
}
