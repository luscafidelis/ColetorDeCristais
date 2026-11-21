using System;
using UnityEngine;

namespace ColetorDeCristais.Animacao
{
    /// <summary>
    /// Animação procedural de um corpo mole, em C# puro: parado, ele pulsa (cresce e encolhe por igual);
    /// andando, estica e achata no ritmo do passo e se inclina na direção do movimento.
    /// </summary>
    /// <remarks>
    /// Guarda só o estado da animação: as fases do pulso e do passo e a velocidade suavizada.
    /// A cada quadro, <see cref="Atualizar"/> recebe a velocidade do corpo no referencial dele
    /// (x = para a direita, y = para a frente, em m/s) e devolve a escala e a rotação do visual.
    /// <list type="bullet">
    /// <item>A intensidade de andar vai de 0 (parado) a 1 (na velocidade de referência ou mais).
    /// O pulso parado é multiplicado por <c>1 - intensidade</c>, o esticar e achatar por <c>intensidade</c>.</item>
    /// <item>O esticar e achatar preserva o volume: quando a altura cresce por um fator <c>f</c>,
    /// a largura e a profundidade mudam por <c>1/√f</c>.</item>
    /// <item>O ritmo do passo acompanha a velocidade: na velocidade de referência, um ciclo dura
    /// <see cref="ParametrosDeDeformacao.PeriodoDoPasso"/>; mais devagar, o ciclo fica mais lento.</item>
    /// <item>A inclinação gira o topo do corpo para o lado do movimento, proporcional à intensidade.</item>
    /// </list>
    /// Os parâmetros vêm em cada chamada, então podem mudar durante o jogo (ex.: pelo Inspector).
    /// </remarks>
    public class CalculadoraDeDeformacao
    {
        private float _fasePulso;
        private float _fasePasso;
        private Vector2 _velocidadeSuavizada;

        /// <summary>Velocidade suavizada usada no último cálculo, em m/s.</summary>
        public Vector2 VelocidadeSuavizada => _velocidadeSuavizada;

        /// <summary>
        /// Avança a animação e calcula a pose do visual.
        /// </summary>
        /// <param name="velocidadeLocal">Velocidade horizontal no referencial do corpo: x para a direita, y para a frente (m/s).</param>
        /// <param name="deltaTempo">Tempo desde o último quadro, em segundos (zero ou mais).</param>
        /// <param name="parametros">Valores da animação.</param>
        /// <exception cref="ArgumentException">Se a velocidade ou o tempo não forem finitos, ou se o tempo for negativo.</exception>
        public PoseDeDeformacao Atualizar(Vector2 velocidadeLocal, float deltaTempo, ParametrosDeDeformacao parametros)
        {
            if (!Finito(velocidadeLocal.x) || !Finito(velocidadeLocal.y))
            {
                throw new ArgumentException("A velocidade precisa ser finita.", nameof(velocidadeLocal));
            }
            if (!Finito(deltaTempo) || deltaTempo < 0f)
            {
                throw new ArgumentException("O tempo do quadro precisa ser finito e não negativo.", nameof(deltaTempo));
            }

            _velocidadeSuavizada = Suavizar(_velocidadeSuavizada, velocidadeLocal, deltaTempo, parametros.TempoDeResposta);

            var rapidez = _velocidadeSuavizada.magnitude;
            var intensidade = Mathf.Clamp01(rapidez / parametros.VelocidadeDeReferencia);

            _fasePulso = Mathf.Repeat(_fasePulso + deltaTempo / parametros.PeriodoDoPulso, 1f);
            _fasePasso = Mathf.Repeat(_fasePasso + deltaTempo * intensidade / parametros.PeriodoDoPasso, 1f);

            var pulso = 1f + parametros.AmplitudeDoPulso * (1f - intensidade) * Onda(_fasePulso);
            var altura = 1f + parametros.EstiramentoMaximo * intensidade * Onda(_fasePasso);
            var largura = 1f / Mathf.Sqrt(altura);
            var escala = new Vector3(largura, altura, largura) * pulso;

            return new PoseDeDeformacao(escala, CalcularInclinacao(intensidade, parametros.InclinacaoMaxima));
        }

        /// <summary>Volta ao repouso: fases zeradas e corpo parado.</summary>
        public void Reiniciar()
        {
            _fasePulso = 0f;
            _fasePasso = 0f;
            _velocidadeSuavizada = Vector2.zero;
        }

        private Quaternion CalcularInclinacao(float intensidade, float inclinacaoMaxima)
        {
            if (intensidade <= 0f)
            {
                return Quaternion.identity;
            }

            // Eixo horizontal perpendicular ao movimento: girar em volta dele leva o topo (+Y) para a direção do movimento.
            var direcao = _velocidadeSuavizada.normalized;
            var eixo = new Vector3(direcao.y, 0f, -direcao.x);
            return Quaternion.AngleAxis(inclinacaoMaxima * intensidade, eixo);
        }

        private static Vector2 Suavizar(Vector2 atual, Vector2 alvo, float deltaTempo, float tempoDeResposta)
        {
            if (tempoDeResposta <= 0f)
            {
                return alvo;
            }

            // Suavização exponencial: não depende da taxa de quadros.
            var fator = 1f - Mathf.Exp(-deltaTempo / tempoDeResposta);
            return Vector2.Lerp(atual, alvo, fator);
        }

        private static float Onda(float fase) => Mathf.Sin(2f * Mathf.PI * fase);

        private static bool Finito(float valor) => !float.IsNaN(valor) && !float.IsInfinity(valor);
    }
}
