using System;
using ColetorDeCristais.Animacao;
using NUnit.Framework;
using UnityEngine;

namespace ColetorDeCristais.Testes.Animacao
{
    /// <summary>
    /// Testes da <see cref="CalculadoraDeDeformacao"/> (pulso parado, esticar e achatar ao andar,
    /// inclinação) e da validação de <see cref="ParametrosDeDeformacao"/>.
    /// Os números são arbitrários, não são valores de jogo.
    /// </summary>
    public class CalculadoraDeDeformacaoTestes
    {
        private const float Tolerancia = 0.0001f;

        /// <summary>Pulso de 10% a cada 1 s, esticar de 20% a cada 0,5 s, 20° de inclinação, referência 4 m/s, sem suavização.</summary>
        private static ParametrosDeDeformacao Parametros(float tempoDeResposta = 0f) =>
            new ParametrosDeDeformacao(0.1f, 1f, 0.2f, 0.5f, 20f, 4f, tempoDeResposta);

        private static float Volume(Vector3 escala) => escala.x * escala.y * escala.z;

        private static float InclinacaoEmGraus(Quaternion rotacao) => Vector3.Angle(Vector3.up, rotacao * Vector3.up);

        // --- Parado ---

        [Test]
        public void Parado_NoInicio_EstaEmRepouso()
        {
            var pose = new CalculadoraDeDeformacao().Atualizar(Vector2.zero, 0f, Parametros());

            Assert.AreEqual(Vector3.one, pose.Escala);
            Assert.AreEqual(0f, InclinacaoEmGraus(pose.Rotacao), Tolerancia);
        }

        [Test]
        public void Parado_PulsaPorIgualNosTresEixos()
        {
            var pose = new CalculadoraDeDeformacao().Atualizar(Vector2.zero, 0.25f, Parametros());

            Assert.AreEqual(1.1f, pose.Escala.x, Tolerancia);
            Assert.AreEqual(1.1f, pose.Escala.y, Tolerancia);
            Assert.AreEqual(1.1f, pose.Escala.z, Tolerancia);
        }

        [Test]
        public void Parado_EncolheNaMetadeDoCicloSeguinte()
        {
            var calculadora = new CalculadoraDeDeformacao();
            calculadora.Atualizar(Vector2.zero, 0.25f, Parametros());

            var pose = calculadora.Atualizar(Vector2.zero, 0.5f, Parametros());

            Assert.AreEqual(0.9f, pose.Escala.y, Tolerancia);
        }

        [Test]
        public void Parado_NuncaPassaDaAmplitude()
        {
            var calculadora = new CalculadoraDeDeformacao();
            for (var i = 0; i < 200; i++)
            {
                var pose = calculadora.Atualizar(Vector2.zero, 0.013f, Parametros());
                Assert.LessOrEqual(Math.Abs(pose.Escala.y - 1f), 0.1f + Tolerancia);
            }
        }

        [Test]
        public void Parado_NaoInclina()
        {
            var pose = new CalculadoraDeDeformacao().Atualizar(Vector2.zero, 0.3f, Parametros());

            Assert.AreEqual(0f, InclinacaoEmGraus(pose.Rotacao), Tolerancia);
        }

        // --- Andando ---

        [Test]
        public void Andando_NaReferencia_NaoPulsa_EsticaPreservandoOVolume()
        {
            // Um quarto do ciclo do passo (0,5 s) na velocidade de referência: altura no máximo.
            var pose = new CalculadoraDeDeformacao().Atualizar(new Vector2(0f, 4f), 0.125f, Parametros());

            Assert.AreEqual(1.2f, pose.Escala.y, Tolerancia);
            Assert.AreEqual(1f / Mathf.Sqrt(1.2f), pose.Escala.x, Tolerancia);
            Assert.AreEqual(pose.Escala.x, pose.Escala.z, Tolerancia);
            Assert.AreEqual(1f, Volume(pose.Escala), Tolerancia);
        }

        [Test]
        public void Andando_AchataNaMetadeDoPasso()
        {
            var calculadora = new CalculadoraDeDeformacao();
            calculadora.Atualizar(new Vector2(0f, 4f), 0.125f, Parametros());

            var pose = calculadora.Atualizar(new Vector2(0f, 4f), 0.25f, Parametros());

            Assert.AreEqual(0.8f, pose.Escala.y, Tolerancia);
            Assert.Greater(pose.Escala.x, 1f);
            Assert.AreEqual(1f, Volume(pose.Escala), Tolerancia);
        }

        [Test]
        public void Andando_AcimaDaReferencia_FicaNoMaximo()
        {
            var pose = new CalculadoraDeDeformacao().Atualizar(new Vector2(0f, 40f), 0.125f, Parametros());

            Assert.AreEqual(1.2f, pose.Escala.y, Tolerancia);
            Assert.AreEqual(20f, InclinacaoEmGraus(pose.Rotacao), 0.01f);
        }

        [Test]
        public void Andando_MaisDevagar_PassoMaisLentoEMenosIntenso()
        {
            // Metade da referência: intensidade 0,5, então o passo avança na metade do ritmo.
            // Em 0,25 s o passo chega a um quarto do ciclo (altura 1 + 0,2 × 0,5) e o pulso, a um quarto (1 + 0,1 × 0,5).
            var pose = new CalculadoraDeDeformacao().Atualizar(new Vector2(0f, 2f), 0.25f, Parametros());

            Assert.AreEqual(1.1f * 1.05f, pose.Escala.y, Tolerancia);
            Assert.AreEqual(10f, InclinacaoEmGraus(pose.Rotacao), 0.01f);
        }

        [TestCase(0f, 1f)]
        [TestCase(1f, 0f)]
        [TestCase(-1f, 0f)]
        [TestCase(0f, -1f)]
        [TestCase(0.6f, 0.8f)]
        public void Andando_InclinaOTopoParaADirecaoDoMovimento(float x, float y)
        {
            var direcao = new Vector2(x, y);
            var pose = new CalculadoraDeDeformacao().Atualizar(direcao * 4f, 0.01f, Parametros());

            var topo = pose.Rotacao * Vector3.up;
            var topoHorizontal = new Vector2(topo.x, topo.z).normalized;

            Assert.AreEqual(direcao.x, topoHorizontal.x, 0.001f, "x");
            Assert.AreEqual(direcao.y, topoHorizontal.y, 0.001f, "y");
            Assert.AreEqual(20f, InclinacaoEmGraus(pose.Rotacao), 0.01f);
        }

        // --- Suavização ---

        [Test]
        public void ComSuavizacao_AcompanhaAVelocidadeAosPoucos()
        {
            var calculadora = new CalculadoraDeDeformacao();

            calculadora.Atualizar(new Vector2(0f, 4f), 0.1f, Parametros(tempoDeResposta: 0.1f));

            // Depois de um tempo de resposta, a suavização exponencial chega a 1 - e^-1 (~63%) do alvo.
            Assert.AreEqual(4f * (1f - Mathf.Exp(-1f)), calculadora.VelocidadeSuavizada.y, 0.001f);
        }

        [Test]
        public void ComSuavizacao_NaoDependeDaTaxaDeQuadros()
        {
            var umQuadro = new CalculadoraDeDeformacao();
            var dezQuadros = new CalculadoraDeDeformacao();

            umQuadro.Atualizar(new Vector2(0f, 4f), 0.2f, Parametros(tempoDeResposta: 0.1f));
            for (var i = 0; i < 10; i++)
            {
                dezQuadros.Atualizar(new Vector2(0f, 4f), 0.02f, Parametros(tempoDeResposta: 0.1f));
            }

            Assert.AreEqual(umQuadro.VelocidadeSuavizada.y, dezQuadros.VelocidadeSuavizada.y, 0.001f);
        }

        [Test]
        public void AoParar_VoltaAPulsarSemInclinar()
        {
            var calculadora = new CalculadoraDeDeformacao();
            calculadora.Atualizar(new Vector2(0f, 4f), 0.1f, Parametros());

            var pose = calculadora.Atualizar(Vector2.zero, 0.1f, Parametros());

            Assert.AreEqual(0f, InclinacaoEmGraus(pose.Rotacao), Tolerancia);
            Assert.AreEqual(pose.Escala.x, pose.Escala.y, Tolerancia, "pulso parado é igual nos três eixos");
        }

        [Test]
        public void Reiniciar_VoltaAoRepouso()
        {
            var calculadora = new CalculadoraDeDeformacao();
            calculadora.Atualizar(new Vector2(0f, 4f), 0.3f, Parametros(tempoDeResposta: 0.1f));

            calculadora.Reiniciar();
            var pose = calculadora.Atualizar(Vector2.zero, 0f, Parametros());

            Assert.AreEqual(Vector2.zero, calculadora.VelocidadeSuavizada);
            Assert.AreEqual(Vector3.one, pose.Escala);
        }

        // --- Validação ---

        [Test]
        public void Atualizar_ValoresInvalidos_LancamExcecao()
        {
            var calculadora = new CalculadoraDeDeformacao();

            Assert.Throws<ArgumentException>(() => calculadora.Atualizar(Vector2.zero, -0.1f, Parametros()));
            Assert.Throws<ArgumentException>(() => calculadora.Atualizar(Vector2.zero, float.NaN, Parametros()));
            Assert.Throws<ArgumentException>(() => calculadora.Atualizar(new Vector2(float.PositiveInfinity, 0f), 0.1f, Parametros()));
        }

        [Test]
        public void Parametros_ForaDaFaixa_LancamExcecao()
        {
            Assert.Throws<ArgumentException>(() => new ParametrosDeDeformacao(-0.1f, 1f, 0.2f, 0.5f, 20f, 4f, 0f), "amplitude negativa");
            Assert.Throws<ArgumentException>(() => new ParametrosDeDeformacao(1f, 1f, 0.2f, 0.5f, 20f, 4f, 0f), "amplitude 100%");
            Assert.Throws<ArgumentException>(() => new ParametrosDeDeformacao(0.1f, 0f, 0.2f, 0.5f, 20f, 4f, 0f), "período do pulso zero");
            Assert.Throws<ArgumentException>(() => new ParametrosDeDeformacao(0.1f, 1f, 1.5f, 0.5f, 20f, 4f, 0f), "estiramento acima de 1");
            Assert.Throws<ArgumentException>(() => new ParametrosDeDeformacao(0.1f, 1f, 0.2f, -1f, 20f, 4f, 0f), "período do passo negativo");
            Assert.Throws<ArgumentException>(() => new ParametrosDeDeformacao(0.1f, 1f, 0.2f, 0.5f, 90f, 4f, 0f), "inclinação de 90°");
            Assert.Throws<ArgumentException>(() => new ParametrosDeDeformacao(0.1f, 1f, 0.2f, 0.5f, 20f, 0f, 0f), "referência zero");
            Assert.Throws<ArgumentException>(() => new ParametrosDeDeformacao(0.1f, 1f, 0.2f, 0.5f, 20f, 4f, -1f), "resposta negativa");
            Assert.Throws<ArgumentException>(() => new ParametrosDeDeformacao(float.NaN, 1f, 0.2f, 0.5f, 20f, 4f, 0f), "NaN");
        }

        [Test]
        public void Parametros_ValoresValidos_SaoGuardados()
        {
            var parametros = new ParametrosDeDeformacao(0.04f, 1f, 0.1f, 0.4f, 12f, 3.5f, 0.1f);

            Assert.AreEqual(0.04f, parametros.AmplitudeDoPulso);
            Assert.AreEqual(1f, parametros.PeriodoDoPulso);
            Assert.AreEqual(0.1f, parametros.EstiramentoMaximo);
            Assert.AreEqual(0.4f, parametros.PeriodoDoPasso);
            Assert.AreEqual(12f, parametros.InclinacaoMaxima);
            Assert.AreEqual(3.5f, parametros.VelocidadeDeReferencia);
            Assert.AreEqual(0.1f, parametros.TempoDeResposta);
        }
    }
}
