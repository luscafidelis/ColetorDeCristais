using System;
using System.Collections.Generic;
using ColetorDeCristais.Nivel;
using UnityEngine;

namespace ColetorDeCristais.Corpo
{
    /// <summary>Sorteio de um item que cai com uma chance (ex.: a carne estragada a cada dano) e onde ele pode cair, em C# puro.</summary>
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

        /// <summary>
        /// Indica se um item pode cair no ponto: não pode dentro de nenhuma das zonas proibidas (vãos e áreas sem
        /// itens novos do nível), onde não seria coletado.
        /// </summary>
        /// <param name="ponto">Ponto no chão (x, z).</param>
        /// <param name="zonasProibidas">Zonas sem itens (nulo ou vazio: pode em qualquer lugar). Retângulos inválidos são ignorados.</param>
        public static bool LocalPermitido(Vector2 ponto, IReadOnlyList<Retangulo> zonasProibidas) =>
            !RegraDeQueda.Caiu(ponto, false, zonasProibidas);
    }
}
