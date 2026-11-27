using System;
using System.Collections.Generic;
using ColetorDeCristais.Nivel;
using UnityEngine;

namespace ColetorDeCristais.Partida
{
    /// <summary>
    /// Fim antecipado da onda (no tutorial), em C# puro: a onda acaba mais cedo quando o jogador coletou todos os
    /// itens que contam.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Contam os itens no chão fora das áreas ignoradas (no mercado, a área bônus do outro lado do vão).</item>
    /// <item>Um item caindo (ainda sem poder ser coletado) conta como item que falta.</item>
    /// <item>Retângulos inválidos (tamanho zero) são ignorados.</item>
    /// </list>
    /// </remarks>
    public static class RegraDoFimAntecipado
    {
        /// <summary>Indica se não falta nenhum item que conta.</summary>
        /// <param name="posicoes">Posições no chão (x = X, y = Z do mundo) dos itens que ainda podem ser coletados.</param>
        /// <param name="itensCaindo">Quantos itens estão caindo agora.</param>
        /// <param name="ignoradas">Áreas cujos itens não contam (pode ser nula).</param>
        /// <exception cref="ArgumentNullException">Se as posições forem nulas.</exception>
        public static bool TudoColetado(IEnumerable<Vector2> posicoes, int itensCaindo, IReadOnlyList<Retangulo> ignoradas) =>
            ItensQueFaltam(posicoes, itensCaindo, ignoradas) == 0;

        /// <summary>
        /// Quantos itens que contam ainda faltam (mostrado na HUD): os itens fora das áreas ignoradas mais os que
        /// estão caindo.
        /// </summary>
        /// <param name="posicoes">Posições no chão (x = X, y = Z do mundo) dos itens que ainda podem ser coletados.</param>
        /// <param name="itensCaindo">Quantos itens estão caindo agora (negativo conta como zero).</param>
        /// <param name="ignoradas">Áreas cujos itens não contam (pode ser nula).</param>
        /// <exception cref="ArgumentNullException">Se as posições forem nulas.</exception>
        public static int ItensQueFaltam(IEnumerable<Vector2> posicoes, int itensCaindo, IReadOnlyList<Retangulo> ignoradas)
        {
            if (posicoes == null)
            {
                throw new ArgumentNullException(nameof(posicoes));
            }

            var faltam = Math.Max(0, itensCaindo);
            foreach (var posicao in posicoes)
            {
                if (!Ignorada(posicao, ignoradas))
                {
                    faltam++;
                }
            }

            return faltam;
        }

        private static bool Ignorada(Vector2 posicao, IReadOnlyList<Retangulo> ignoradas)
        {
            if (ignoradas == null)
            {
                return false;
            }

            foreach (var area in ignoradas)
            {
                if (area.Valido && area.Contem(posicao))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
