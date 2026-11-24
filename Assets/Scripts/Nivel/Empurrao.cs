using System;
using UnityEngine;

namespace ColetorDeCristais.Nivel
{
    /// <summary>
    /// Empurrão de um obstáculo em movimento (ex.: um carrinho) no personagem, em C# puro.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>O obstáculo é um <see cref="Retangulo"/> no chão e o personagem é um círculo (o raio do <c>CharacterController</c>).</item>
    /// <item>Se, depois de o obstáculo andar, o círculo ficou encostado por dentro dele, o personagem é empurrado
    /// na direção do movimento do obstáculo, só o bastante para sair, mais uma folga.</item>
    /// <item>Quem só encosta do lado (sem sobrepor) não é empurrado: o obstáculo passa raspando.</item>
    /// </list>
    /// A conta usa a forma do obstáculo "engordada" pelo raio (convexa), então a saída é achada por busca binária.
    /// </remarks>
    public static class Empurrao
    {
        private const int Iteracoes = 40;

        /// <summary>Deslocamento a aplicar no personagem (zero se não houver empurrão).</summary>
        /// <param name="obstaculo">Base do obstáculo já na posição nova.</param>
        /// <param name="movimento">Quanto o obstáculo andou neste quadro (x, z).</param>
        /// <param name="personagem">Centro do personagem no chão (x, z).</param>
        /// <param name="raio">Raio do personagem (maior que zero).</param>
        /// <param name="folga">Distância extra para o personagem ficar fora (zero ou mais).</param>
        /// <exception cref="ArgumentException">Se algum valor estiver fora da faixa.</exception>
        public static Vector2 Calcular(Retangulo obstaculo, Vector2 movimento, Vector2 personagem, float raio, float folga = 0.01f)
        {
            if (!Finito(movimento.x) || !Finito(movimento.y) || !Finito(personagem.x) || !Finito(personagem.y))
            {
                throw new ArgumentException("O movimento e a posição precisam ser finitos.");
            }

            if (!Finito(raio) || raio <= 0f)
            {
                throw new ArgumentException("O raio precisa ser finito e maior que zero.", nameof(raio));
            }

            if (!Finito(folga) || folga < 0f)
            {
                throw new ArgumentException("A folga precisa ser finita e não negativa.", nameof(folga));
            }

            var alcance = raio + folga;
            if (movimento.sqrMagnitude < 1e-12f || obstaculo.DistanciaAte(personagem) >= raio)
            {
                return Vector2.zero;
            }

            // Saída do retângulo engordado pelo alcance, na direção do movimento. A forma é convexa
            // e o ponto de partida está dentro, então "dentro" ao longo da reta é um único trecho.
            var direcao = movimento.normalized;
            var maximo = obstaculo.Tamanho.magnitude + 2f * alcance;
            var baixo = 0f;
            var alto = maximo;
            for (var i = 0; i < Iteracoes; i++)
            {
                var meio = (baixo + alto) * 0.5f;
                if (obstaculo.DistanciaAte(personagem + direcao * meio) < alcance)
                {
                    baixo = meio;
                }
                else
                {
                    alto = meio;
                }
            }

            return direcao * alto;
        }

        private static bool Finito(float valor) => !float.IsNaN(valor) && !float.IsInfinity(valor);
    }
}
