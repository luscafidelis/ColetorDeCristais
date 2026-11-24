using System;
using UnityEngine;

namespace ColetorDeCristais.Nivel
{
    /// <summary>
    /// Movimento de vai e vem entre dois pontos, em velocidade constante, em C# puro (os carrinhos do mercado).
    /// </summary>
    /// <remarks>
    /// A posição é calculada direto do tempo acumulado, sem somar passos, então é a mesma em qualquer
    /// taxa de quadros. Um ciclo completo (ida e volta) leva <c>2 × distância ÷ velocidade</c>.
    /// A <c>fase</c> (0 a 1) adianta o ciclo, para vários carrinhos não andarem juntos.
    /// </remarks>
    public static class VaiEVem
    {
        /// <summary>Posição no tempo dado.</summary>
        /// <param name="inicio">Ponto onde o ciclo começa (fase 0).</param>
        /// <param name="fim">Ponto da volta.</param>
        /// <param name="velocidade">Metros por segundo (maior que zero).</param>
        /// <param name="tempo">Segundos desde o começo (zero ou mais).</param>
        /// <param name="fase">Adianto do ciclo, de 0 a 1 (0,5 começa no fim).</param>
        /// <exception cref="ArgumentException">Se algum valor estiver fora da faixa.</exception>
        public static Vector3 Posicao(Vector3 inicio, Vector3 fim, float velocidade, float tempo, float fase = 0f)
        {
            var distancia = Vector3.Distance(inicio, fim);
            var percorrido = Percorrido(distancia, velocidade, tempo, fase);
            return distancia > 0f ? Vector3.Lerp(inicio, fim, percorrido / distancia) : inicio;
        }

        /// <summary>Quanto o objeto está longe do início, de 0 a <paramref name="distancia"/>, no tempo dado.</summary>
        /// <param name="distancia">Distância entre os pontos (zero ou mais; zero fica parado).</param>
        /// <param name="velocidade">Metros por segundo (maior que zero).</param>
        /// <param name="tempo">Segundos desde o começo (zero ou mais).</param>
        /// <param name="fase">Adianto do ciclo, de 0 a 1.</param>
        /// <exception cref="ArgumentException">Se algum valor estiver fora da faixa.</exception>
        public static float Percorrido(float distancia, float velocidade, float tempo, float fase = 0f)
        {
            if (!Finito(distancia) || distancia < 0f)
            {
                throw new ArgumentException("A distância precisa ser finita e não negativa.", nameof(distancia));
            }

            if (!Finito(velocidade) || velocidade <= 0f)
            {
                throw new ArgumentException("A velocidade precisa ser finita e maior que zero.", nameof(velocidade));
            }

            if (!Finito(tempo) || tempo < 0f)
            {
                throw new ArgumentException("O tempo precisa ser finito e não negativo.", nameof(tempo));
            }

            if (!Finito(fase) || fase < 0f || fase > 1f)
            {
                throw new ArgumentException("A fase vai de 0 a 1.", nameof(fase));
            }

            if (distancia <= 0f)
            {
                return 0f;
            }

            // Em double, para o tempo de uma partida longa não perder precisão.
            var ciclo = 2.0 * distancia;
            var noCiclo = (tempo * (double)velocidade + fase * ciclo) % ciclo;
            return (float)(noCiclo <= distancia ? noCiclo : ciclo - noCiclo);
        }

        private static bool Finito(float valor) => !float.IsNaN(valor) && !float.IsInfinity(valor);
    }
}
