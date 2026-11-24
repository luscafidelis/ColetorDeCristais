using System;
using System.Collections.Generic;
using ColetorDeCristais.Nivel;
using UnityEngine;
using Random = System.Random;

namespace ColetorDeCristais.Partida
{
    /// <summary>
    /// Sorteia posições no chão (x, z) para itens novos, em C# puro: dentro de uma área quadrada e a uma
    /// distância mínima do jogador, dos itens que já existem e umas das outras, e fora das áreas proibidas
    /// do nível (vão, prateleiras, área bônus).
    /// </summary>
    /// <remarks>
    /// Usa tentativas aleatórias: cada posição sorteada só é aceita se respeitar as distâncias.
    /// Se a área estiver cheia, pode devolver menos posições que o pedido (quem chama decide o que fazer).
    /// Com o mesmo <see cref="Random"/> (mesma semente), o resultado é sempre o mesmo.
    /// </remarks>
    public static class GeradorDePosicoes
    {
        /// <summary>Sorteia até <paramref name="quantidade"/> posições.</summary>
        /// <param name="quantidade">Quantas posições sortear (zero ou mais).</param>
        /// <param name="limite">A área vai de -limite a +limite em x e em z (maior que zero).</param>
        /// <param name="jogador">Posição do jogador no chão (x, z).</param>
        /// <param name="ocupadas">Posições de itens que já existem (x, z).</param>
        /// <param name="distanciaMinima">Distância mínima até o jogador, os itens existentes e entre as novas (zero ou mais).</param>
        /// <param name="sorteio">Gerador de números aleatórios.</param>
        /// <param name="tentativasPorPosicao">Quantos sorteios tentar por posição antes de desistir dela.</param>
        /// <param name="proibidas">Áreas onde nenhum item novo entra (nulo: nenhuma). Desenhe-as já com a folga desejada;
        /// retângulos inválidos são ignorados.</param>
        /// <returns>As posições sorteadas (pode ter menos que o pedido).</returns>
        /// <exception cref="ArgumentException">Se algum valor estiver fora da faixa.</exception>
        public static List<Vector2> Gerar(
            int quantidade,
            float limite,
            Vector2 jogador,
            IReadOnlyList<Vector2> ocupadas,
            float distanciaMinima,
            Random sorteio,
            int tentativasPorPosicao = 100,
            IReadOnlyList<Retangulo> proibidas = null)
        {
            if (quantidade < 0)
            {
                throw new ArgumentException("A quantidade não pode ser negativa.", nameof(quantidade));
            }

            if (float.IsNaN(limite) || float.IsInfinity(limite) || limite <= 0f)
            {
                throw new ArgumentException("O limite precisa ser finito e maior que zero.", nameof(limite));
            }

            if (float.IsNaN(distanciaMinima) || float.IsInfinity(distanciaMinima) || distanciaMinima < 0f)
            {
                throw new ArgumentException("A distância mínima precisa ser finita e não negativa.", nameof(distanciaMinima));
            }

            if (tentativasPorPosicao < 1)
            {
                throw new ArgumentException("É preciso pelo menos uma tentativa.", nameof(tentativasPorPosicao));
            }

            if (sorteio == null)
            {
                throw new ArgumentNullException(nameof(sorteio));
            }

            var minimaAoQuadrado = distanciaMinima * distanciaMinima;
            var novas = new List<Vector2>(quantidade);
            for (var i = 0; i < quantidade; i++)
            {
                for (var tentativa = 0; tentativa < tentativasPorPosicao; tentativa++)
                {
                    var candidata = new Vector2(Sortear(sorteio, limite), Sortear(sorteio, limite));
                    if (ForaDasProibidas(candidata, proibidas) && Livre(candidata, jogador, ocupadas, novas, minimaAoQuadrado))
                    {
                        novas.Add(candidata);
                        break;
                    }
                }
            }

            return novas;
        }

        private static float Sortear(Random sorteio, float limite) => (float)(sorteio.NextDouble() * 2.0 - 1.0) * limite;

        private static bool ForaDasProibidas(Vector2 candidata, IReadOnlyList<Retangulo> proibidas)
        {
            if (proibidas == null)
            {
                return true;
            }

            for (var i = 0; i < proibidas.Count; i++)
            {
                if (proibidas[i].Valido && proibidas[i].Contem(candidata))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool Livre(Vector2 candidata, Vector2 jogador, IReadOnlyList<Vector2> ocupadas, List<Vector2> novas, float minimaAoQuadrado)
        {
            if ((candidata - jogador).sqrMagnitude < minimaAoQuadrado)
            {
                return false;
            }

            if (ocupadas != null)
            {
                for (var i = 0; i < ocupadas.Count; i++)
                {
                    if ((candidata - ocupadas[i]).sqrMagnitude < minimaAoQuadrado)
                    {
                        return false;
                    }
                }
            }

            for (var i = 0; i < novas.Count; i++)
            {
                if ((candidata - novas[i]).sqrMagnitude < minimaAoQuadrado)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
