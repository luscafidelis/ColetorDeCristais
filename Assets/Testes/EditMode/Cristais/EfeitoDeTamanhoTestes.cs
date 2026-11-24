using System.Text.RegularExpressions;
using ColetorDeCristais.Corpo;
using ColetorDeCristais.Cristais;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace ColetorDeCristais.Testes.Cristais
{
    /// <summary>
    /// Testes do <see cref="EfeitoDeTamanho"/> (o efeito da carne): soma o valor no tamanho do jogador,
    /// sem limite, e registra erro quando ele não tem <see cref="CorpoDoJogador"/>.
    /// </summary>
    public class EfeitoDeTamanhoTestes
    {
        private EfeitoDeTamanho _efeito;
        private GameObject _jogador;

        [SetUp]
        public void Preparar()
        {
            _efeito = ScriptableObject.CreateInstance<EfeitoDeTamanho>();
            _efeito.name = "EfeitoDeTeste";
            DefinirValor(1f / 6f);
            _jogador = new GameObject("JogadorDeTeste");
        }

        [TearDown]
        public void Limpar()
        {
            Object.DestroyImmediate(_efeito);
            Object.DestroyImmediate(_jogador);
        }

        private void DefinirValor(float valor)
        {
            var serializado = new SerializedObject(_efeito);
            serializado.FindProperty("valor").floatValue = valor;
            serializado.ApplyModifiedPropertiesWithoutUndo();
        }

        [Test]
        public void Aplicar_SomaOValorNoTamanho()
        {
            var coletor = _jogador.AddComponent<ColetorDoJogador>();
            var corpo = _jogador.AddComponent<CorpoDoJogador>();
            corpo.Tamanho.ValorBase = 1f / 3f;

            _efeito.Aplicar(coletor);

            Assert.AreEqual(0.5f, corpo.Tamanho.ValorFinal, 1e-5f);
        }

        [Test]
        public void Aplicar_VariasVezes_CresceSemLimite()
        {
            var coletor = _jogador.AddComponent<ColetorDoJogador>();
            var corpo = _jogador.AddComponent<CorpoDoJogador>();
            corpo.Tamanho.ValorBase = 1f / 3f;

            for (var i = 0; i < 10; i++)
            {
                _efeito.Aplicar(coletor);
            }

            Assert.AreEqual(1f / 3f + 10f / 6f, corpo.Tamanho.ValorFinal, 1e-4f);
        }

        [Test]
        public void Aplicar_ValorNegativo_NaoPassaDeZero()
        {
            var coletor = _jogador.AddComponent<ColetorDoJogador>();
            var corpo = _jogador.AddComponent<CorpoDoJogador>();
            corpo.Tamanho.ValorBase = 1f / 3f;
            DefinirValor(-1f);

            _efeito.Aplicar(coletor);

            Assert.AreEqual(0f, corpo.Tamanho.ValorFinal);
        }

        [Test]
        public void Aplicar_ColetorSemCorpo_RegistraErro()
        {
            var coletor = _jogador.AddComponent<ColetorDoJogador>();

            LogAssert.Expect(LogType.Error, new Regex("não tem CorpoDoJogador"));
            _efeito.Aplicar(coletor);
        }

        [Test]
        public void Aplicar_ColetorNulo_RegistraErro()
        {
            LogAssert.Expect(LogType.Error, new Regex("não tem CorpoDoJogador"));
            _efeito.Aplicar(null);
        }
    }
}
