using ColetorDeCristais.Interface;
using ColetorDeCristais.Partida;
using NUnit.Framework;

namespace ColetorDeCristais.Testes.Interface
{
    /// <summary>
    /// Testes da HUD: o estado dos ícones das habilidades (<see cref="IndicadorDeHabilidade"/>) e os textos da
    /// <see cref="HudDaPartida"/> (mensagem de cada fase e contador de itens).
    /// </summary>
    public class HudTestes
    {
        private const float Tolerancia = 0.0001f;

        // --- Ícones das habilidades ---

        [Test]
        public void Icone_NaoObtida_Bloqueada()
        {
            var indicador = IndicadorDeHabilidade.Calcular(false, 3f, 5f);

            Assert.AreEqual(EstadoDaHabilidade.Bloqueada, indicador.Estado);
            Assert.AreEqual(0f, indicador.Preenchimento);
        }

        [Test]
        public void Icone_ObtidaSemRecarga_Pronta()
        {
            var indicador = IndicadorDeHabilidade.Calcular(true, 0f, 5f);

            Assert.AreEqual(EstadoDaHabilidade.Pronta, indicador.Estado);
            Assert.AreEqual(1f, indicador.Preenchimento);
        }

        [Test]
        public void Icone_HabilidadeSemRecarga_SemprePronta()
        {
            // O ímã: obtido, sem recarga.
            Assert.AreEqual(EstadoDaHabilidade.Pronta, IndicadorDeHabilidade.Calcular(true, 0f, 0f).Estado);
        }

        [TestCase(5f, 0f)]       // acabou de usar a investida
        [TestCase(3.75f, 0.25f)]
        [TestCase(2.5f, 0.5f)]
        [TestCase(0.5f, 0.9f)]
        public void Icone_EmRecarga_EncheComOTempo(float restante, float preenchimento)
        {
            var indicador = IndicadorDeHabilidade.Calcular(true, restante, 5f);

            Assert.AreEqual(EstadoDaHabilidade.EmRecarga, indicador.Estado);
            Assert.AreEqual(preenchimento, indicador.Preenchimento, Tolerancia);
        }

        [Test]
        public void Icone_RestanteMaiorQueOTotal_FicaVazio()
        {
            var indicador = IndicadorDeHabilidade.Calcular(true, 8f, 5f);

            Assert.AreEqual(EstadoDaHabilidade.EmRecarga, indicador.Estado);
            Assert.AreEqual(0f, indicador.Preenchimento);
        }

        [TestCase(float.NaN, 5f)]
        [TestCase(-1f, 5f)]
        [TestCase(2f, float.NaN)]
        [TestCase(2f, -5f)]
        [TestCase(2f, float.PositiveInfinity)]
        public void Icone_ValoresInvalidos_Pronta(float restante, float total)
        {
            var indicador = IndicadorDeHabilidade.Calcular(true, restante, total);

            Assert.AreEqual(EstadoDaHabilidade.Pronta, indicador.Estado);
            Assert.AreEqual(1f, indicador.Preenchimento);
        }

        // --- Textos ---

        [Test]
        public void Comida_MostraQuantosFaltam()
        {
            Assert.AreEqual("Comida: 7", HudDaPartida.TextoDosItens(7));
            Assert.AreEqual("Comida: 0", HudDaPartida.TextoDosItens(0));
        }

        [Test]
        public void Itens_SemFimAntecipado_Vazio()
        {
            Assert.AreEqual(string.Empty, HudDaPartida.TextoDosItens(-1));
        }

        [Test]
        public void Mensagens_CadaFaseTemSeuTexto()
        {
            Assert.AreEqual(string.Empty, HudDaPartida.TextoDaMensagem(EstadoDaPartida.EmOnda, 1, false));
            StringAssert.StartsWith("Chefe da onda 2!", HudDaPartida.TextoDaMensagem(EstadoDaPartida.ChefeDaOnda, 2, false));
            StringAssert.StartsWith("Chefe da arena!", HudDaPartida.TextoDaMensagem(EstadoDaPartida.ChefeDaArena, 3, false));
            StringAssert.StartsWith("Arena concluída!", HudDaPartida.TextoDaMensagem(EstadoDaPartida.Vitoria, 3, false));
            StringAssert.StartsWith("Derrota", HudDaPartida.TextoDaMensagem(EstadoDaPartida.Derrota, 2, false));
        }

        [Test]
        public void Mensagens_SoMostramATeclaDepoisDaEspera()
        {
            StringAssert.DoesNotContain("qualquer tecla", HudDaPartida.TextoDaMensagem(EstadoDaPartida.Vitoria, 3, false));
            StringAssert.Contains("qualquer tecla para recomeçar", HudDaPartida.TextoDaMensagem(EstadoDaPartida.Vitoria, 3, true));
            StringAssert.Contains("qualquer tecla para vencê-lo", HudDaPartida.TextoDaMensagem(EstadoDaPartida.ChefeDaOnda, 1, true));
        }
    }
}
