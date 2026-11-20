using System;
using UnityEngine;

namespace ColetorDeCristais.Movimento
{
    /// <summary>
    /// Cálculos de movimento do personagem, sem <see cref="MonoBehaviour"/> e sem estado,
    /// para poderem ser testados em EditMode. Usa só os tipos matemáticos da Unity
    /// (<see cref="Vector2"/>, <see cref="Vector3"/>, <see cref="Mathf"/>).
    /// </summary>
    /// <remarks>
    /// Convenção de eixos: a entrada 2D (x, y) vira um deslocamento no plano do chão (x, z).
    /// Entrada "para cima" (y positivo) move para +Z, que é a direção em que a câmera fixa olha.
    /// </remarks>
    public static class CalculadoraDeMovimento
    {
        /// <summary>
        /// Abaixo deste tamanho a entrada é tratada como parada (evita giro por ruído do analógico).
        /// </summary>
        public const float EntradaMinima = 0.01f;

        /// <summary>
        /// Calcula o deslocamento no plano do chão para um quadro.
        /// </summary>
        /// <param name="entrada">Direção lida do controle. Tamanho acima de 1 é reduzido a 1,
        /// para que a diagonal do teclado não seja mais rápida que as retas.</param>
        /// <param name="velocidade">Velocidade em metros por segundo. Valores negativos são tratados como zero.</param>
        /// <param name="deltaTempo">Duração do quadro em segundos; finita e maior ou igual a zero.</param>
        /// <returns>Deslocamento em metros, sempre com <c>y = 0</c>.</returns>
        /// <exception cref="ArgumentException">Se algum valor for NaN/infinito ou se o tempo for negativo.</exception>
        public static Vector3 CalcularDeslocamento(Vector2 entrada, float velocidade, float deltaTempo)
        {
            if (!EhFinito(entrada.x) || !EhFinito(entrada.y))
            {
                throw new ArgumentException("A entrada precisa ter valores finitos.", nameof(entrada));
            }

            if (!EhFinito(velocidade))
            {
                throw new ArgumentException("A velocidade precisa ser um número finito.", nameof(velocidade));
            }

            if (!EhFinito(deltaTempo) || deltaTempo < 0f)
            {
                throw new ArgumentException("O tempo do quadro precisa ser finito e maior ou igual a zero.", nameof(deltaTempo));
            }

            var direcao = Vector2.ClampMagnitude(entrada, 1f);
            var distancia = Mathf.Max(0f, velocidade) * deltaTempo;
            return new Vector3(direcao.x, 0f, direcao.y) * distancia;
        }

        /// <summary>
        /// Calcula o ângulo (em graus, no eixo Y) para o qual o personagem deve olhar.
        /// 0° olha para +Z e 90° para +X, como no <c>Transform.eulerAngles.y</c> da Unity.
        /// </summary>
        /// <param name="entrada">Direção lida do controle.</param>
        /// <param name="angulo">Ângulo alvo entre 0 e 360, ou 0 se não houver direção.</param>
        /// <returns><c>false</c> se a entrada for menor que <see cref="EntradaMinima"/> (o personagem mantém a direção atual).</returns>
        public static bool TentarCalcularAnguloAlvo(Vector2 entrada, out float angulo)
        {
            if (!EhFinito(entrada.x) || !EhFinito(entrada.y) || entrada.magnitude < EntradaMinima)
            {
                angulo = 0f;
                return false;
            }

            angulo = Mathf.Repeat(Mathf.Atan2(entrada.x, entrada.y) * Mathf.Rad2Deg, 360f);
            return true;
        }

        /// <summary>
        /// Gira um ângulo em direção ao alvo pelo caminho mais curto, sem passar do alvo.
        /// </summary>
        /// <param name="anguloAtual">Ângulo atual em graus.</param>
        /// <param name="anguloAlvo">Ângulo desejado em graus.</param>
        /// <param name="velocidadeAngular">Graus por segundo; negativo é tratado como zero.</param>
        /// <param name="deltaTempo">Duração do quadro em segundos; finita e maior ou igual a zero.</param>
        /// <returns>Novo ângulo entre 0 e 360.</returns>
        /// <exception cref="ArgumentException">Se algum valor for NaN/infinito ou se o tempo for negativo.</exception>
        public static float GirarEmDirecao(float anguloAtual, float anguloAlvo, float velocidadeAngular, float deltaTempo)
        {
            if (!EhFinito(anguloAtual) || !EhFinito(anguloAlvo) || !EhFinito(velocidadeAngular))
            {
                throw new ArgumentException("Os ângulos e a velocidade angular precisam ser finitos.");
            }

            if (!EhFinito(deltaTempo) || deltaTempo < 0f)
            {
                throw new ArgumentException("O tempo do quadro precisa ser finito e maior ou igual a zero.", nameof(deltaTempo));
            }

            var passoMaximo = Mathf.Max(0f, velocidadeAngular) * deltaTempo;
            return Mathf.Repeat(Mathf.MoveTowardsAngle(anguloAtual, anguloAlvo, passoMaximo), 360f);
        }

        private static bool EhFinito(float valor)
        {
            return !float.IsNaN(valor) && !float.IsInfinity(valor);
        }
    }
}
