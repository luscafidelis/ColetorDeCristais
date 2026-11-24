using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColetorDeCristais.Nivel
{
    /// <summary>
    /// Quando o personagem cai num vão, em C# puro, e a descida mostrada depois da queda.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Cai quem está com o centro dentro de um vão e <b>não</b> está numa investida. Durante a investida
    /// o personagem passa por cima; se ela acabar com ele ainda sobre o vão, ele cai.</item>
    /// <item>O jogo não tem gravidade: a queda é esta regra, e a descida é só o desenho.</item>
    /// </list>
    /// </remarks>
    public static class RegraDeQueda
    {
        /// <summary>Indica se o personagem caiu.</summary>
        /// <param name="posicao">Centro do personagem no chão (x, z).</param>
        /// <param name="emInvestida">Se uma investida está em andamento.</param>
        /// <param name="vaos">Os vãos do nível (nulo ou vazio: não há onde cair). Retângulos inválidos são ignorados.</param>
        public static bool Caiu(Vector2 posicao, bool emInvestida, IReadOnlyList<Retangulo> vaos)
        {
            if (emInvestida || vaos == null)
            {
                return false;
            }

            for (var i = 0; i < vaos.Count; i++)
            {
                if (vaos[i].Valido && vaos[i].Contem(posicao))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Quanto o personagem já desceu, <paramref name="tempo"/> segundos depois de cair (queda livre, com limite).</summary>
        /// <param name="tempo">Segundos desde a queda (zero ou mais).</param>
        /// <param name="gravidade">Aceleração, em m/s² (maior que zero).</param>
        /// <param name="profundidade">Descida máxima, em metros (zero ou mais).</param>
        /// <exception cref="ArgumentException">Se algum valor estiver fora da faixa.</exception>
        public static float Descida(float tempo, float gravidade, float profundidade)
        {
            if (!Finito(tempo) || tempo < 0f)
            {
                throw new ArgumentException("O tempo precisa ser finito e não negativo.", nameof(tempo));
            }

            if (!Finito(gravidade) || gravidade <= 0f)
            {
                throw new ArgumentException("A gravidade precisa ser finita e maior que zero.", nameof(gravidade));
            }

            if (!Finito(profundidade) || profundidade < 0f)
            {
                throw new ArgumentException("A profundidade precisa ser finita e não negativa.", nameof(profundidade));
            }

            return Mathf.Min(0.5f * gravidade * tempo * tempo, profundidade);
        }

        private static bool Finito(float valor) => !float.IsNaN(valor) && !float.IsInfinity(valor);
    }
}
