using System;
using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Cálculos da animação de giro e flutuação do cristal, sem estado e sem <see cref="MonoBehaviour"/>.
    /// Tudo é função do tempo, então a animação não acumula erro de um quadro para o outro.
    /// </summary>
    public static class CalculadoraDeFlutuacao
    {
        /// <summary>
        /// Deslocamento vertical (senoide) em relação à posição de repouso.
        /// </summary>
        /// <param name="tempo">Tempo em segundos.</param>
        /// <param name="amplitude">Quanto sobe e desce, em metros; negativa conta como zero.</param>
        /// <param name="periodo">Duração de um ciclo completo, em segundos; precisa ser maior que zero.</param>
        /// <returns>Valor entre <c>-amplitude</c> e <c>+amplitude</c>; começa em 0 no tempo 0.</returns>
        /// <exception cref="ArgumentException">Se algum valor não for finito ou se o período não for positivo.</exception>
        public static float DeslocamentoVertical(float tempo, float amplitude, float periodo)
        {
            ValidarFinito(tempo, nameof(tempo));
            ValidarFinito(amplitude, nameof(amplitude));
            ValidarFinito(periodo, nameof(periodo));
            if (periodo <= 0f)
            {
                throw new ArgumentException("O período precisa ser maior que zero.", nameof(periodo));
            }

            return Mathf.Max(0f, amplitude) * Mathf.Sin(2f * Mathf.PI * tempo / periodo);
        }

        /// <summary>
        /// Ângulo de giro no eixo vertical num dado tempo.
        /// </summary>
        /// <param name="tempo">Tempo em segundos.</param>
        /// <param name="velocidadeAngular">Graus por segundo (negativo gira para o outro lado).</param>
        /// <returns>Ângulo entre 0 e 360.</returns>
        /// <exception cref="ArgumentException">Se algum valor não for finito.</exception>
        public static float AnguloDeGiro(float tempo, float velocidadeAngular)
        {
            ValidarFinito(tempo, nameof(tempo));
            ValidarFinito(velocidadeAngular, nameof(velocidadeAngular));

            return Mathf.Repeat(velocidadeAngular * tempo, 360f);
        }

        private static void ValidarFinito(float valor, string nome)
        {
            if (float.IsNaN(valor) || float.IsInfinity(valor))
            {
                throw new ArgumentException("O valor precisa ser um número finito.", nome);
            }
        }
    }
}
