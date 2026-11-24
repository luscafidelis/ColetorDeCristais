using System.Text.RegularExpressions;
using ColetorDeCristais.Atributos;
using ColetorDeCristais.Cristais;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace ColetorDeCristais.Testes.Cristais
{
    /// <summary>
    /// Testes da ponte entre os assets de efeito e o jogador: <see cref="EfeitoDeVelocidade"/>
    /// e a aplicação dos efeitos em <see cref="ColetorDoJogador.Registrar"/>.
    /// Os valores padrão do asset (+30% por 5 s e +0,3 permanente no fim) são os da coxa.
    /// </summary>
    public class EfeitoDeVelocidadeTestes
    {
        private const float Tolerancia = 0.0001f;

        private EfeitoDeVelocidade _efeito;
        private GameObject _jogador;

        [SetUp]
        public void Preparar()
        {
            _efeito = ScriptableObject.CreateInstance<EfeitoDeVelocidade>();
            _efeito.name = "EfeitoDeTeste";
        }

        [TearDown]
        public void Limpar()
        {
            Object.DestroyImmediate(_efeito);
            if (_jogador != null)
            {
                Object.DestroyImmediate(_jogador);
            }
        }

        private void DefinirCampo(string campo, float valor)
        {
            var serializado = new SerializedObject(_efeito);
            serializado.FindProperty(campo).floatValue = valor;
            serializado.ApplyModifiedPropertiesWithoutUndo();
        }

        // --- EfeitoDeVelocidade ---

        [Test]
        public void AplicarEm_UsaOsValoresDoAsset_EOAssetComoOrigem()
        {
            var velocidade = new Atributo(3.5f, valorMinimo: 0f);

            Assert.IsTrue(_efeito.AplicarEm(velocidade));

            Assert.AreEqual(3.5f * 1.3f, velocidade.ValorFinal, Tolerancia);
            Assert.AreEqual(1, velocidade.ModificadoresAgendados.Count, "o permanente espera o fim do temporário");

            velocidade.Atualizar(5f);
            Assert.AreEqual(3.8f, velocidade.ValorFinal, Tolerancia);

            Assert.AreEqual(1, velocidade.RemoverModificadoresDaOrigem(_efeito), "o permanente tem o asset como origem");
        }

        [Test]
        public void AplicarEm_ComValoresInvalidos_RegistraErroENaoAplica()
        {
            DefinirCampo("duracao", 0f); // parte temporária sem duração
            var velocidade = new Atributo(3.5f);

            LogAssert.Expect(LogType.Error, new Regex("EfeitoDeTeste: valores inválidos"));
            Assert.IsFalse(_efeito.AplicarEm(velocidade));

            Assert.AreEqual(3.5f, velocidade.ValorFinal, Tolerancia);
            Assert.AreEqual(0, velocidade.Modificadores.Count);
            Assert.AreEqual(0, velocidade.ModificadoresAgendados.Count);
        }

        [Test]
        public void AplicarEm_AtributoNulo_LancaExcecao()
        {
            Assert.Throws<System.ArgumentNullException>(() => _efeito.AplicarEm(null));
        }

        [Test]
        public void Aplicar_ColetorSemMovimento_RegistraErro()
        {
            _jogador = new GameObject("JogadorDeTeste");
            var coletor = _jogador.AddComponent<ColetorDoJogador>();

            LogAssert.Expect(LogType.Error, new Regex("não tem MovimentoDoJogador"));
            _efeito.Aplicar(coletor);
        }

        // --- ColetorDoJogador.Registrar ---

        [Test]
        public void Registrar_EfeitoComErro_NaoImpedeOsOutrosNemAContagem()
        {
            _jogador = new GameObject("JogadorDeTeste");
            var coletor = _jogador.AddComponent<ColetorDoJogador>();
            var falha = ScriptableObject.CreateInstance<EfeitoQueFalhaParaTeste>();
            var registro = ScriptableObject.CreateInstance<EfeitoQueRegistraParaTeste>();
            var definicao = ScriptableObject.CreateInstance<DefinicaoCristal>();
            try
            {
                var serializado = new SerializedObject(definicao);
                serializado.FindProperty("identificador").stringValue = "teste";
                var efeitos = serializado.FindProperty("efeitos");
                efeitos.arraySize = 2;
                efeitos.GetArrayElementAtIndex(0).objectReferenceValue = falha;
                efeitos.GetArrayElementAtIndex(1).objectReferenceValue = registro;
                serializado.ApplyModifiedPropertiesWithoutUndo();

                LogAssert.Expect(LogType.Exception, new Regex("falha de teste"));
                Assert.DoesNotThrow(() => coletor.Registrar(definicao));

                Assert.AreEqual(1, registro.Aplicacoes, "o efeito seguinte foi aplicado");
                Assert.AreEqual(1, coletor.Contador.Total);
            }
            finally
            {
                Object.DestroyImmediate(falha);
                Object.DestroyImmediate(registro);
                Object.DestroyImmediate(definicao);
            }
        }
    }

    /// <summary>Efeito de teste que sempre lança exceção.</summary>
    public class EfeitoQueFalhaParaTeste : EfeitoDeCristal
    {
        public override void Aplicar(ColetorDoJogador coletor) =>
            throw new System.InvalidOperationException("falha de teste");
    }

    /// <summary>Efeito de teste que conta quantas vezes foi aplicado.</summary>
    public class EfeitoQueRegistraParaTeste : EfeitoDeCristal
    {
        public int Aplicacoes { get; private set; }

        public override void Aplicar(ColetorDoJogador coletor) => Aplicacoes++;
    }
}
