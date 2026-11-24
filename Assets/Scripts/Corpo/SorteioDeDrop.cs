using System;

namespace ColetorDeCristais.Corpo
{
    /// <summary>Sorteio de um item que cai com uma chance (ex.: a carne estragada a cada dano), em C# puro.</summary>
    public static class SorteioDeDrop
    {
        /// <summary>
        /// Indica se o item cai, dado um número sorteado de 0 (inclusive) a 1 (exclusive), como o de
        /// <c>System.Random.NextDouble()</c>: cai quando o número é menor que a chance.
        /// </summary>
        /// <param name="chance">Chance de 0 a 1 (0,33 = 33%). Fora da faixa, vale o limite mais perto.</param>
        /// <param name="sorteio">Número sorteado, de 0 a 1.</param>
        /// <exception cref="ArgumentException">Se a chance ou o sorteio for NaN.</exception>
        public static bool Sorteou(float chance, double sorteio)
        {
            if (float.IsNaN(chance) || double.IsNaN(sorteio))
            {
                throw new ArgumentException("A chance e o sorteio não podem ser NaN.");
            }

            return sorteio < Math.Min(1f, Math.Max(0f, chance));
        }
    }
}
