using System;
using UnityEngine;

namespace ColetorDeCristais.Nivel
{
    /// <summary>
    /// Retângulo no chão (plano x, z), alinhado aos eixos, em C# puro. Descreve áreas do nível:
    /// o vão, a base de uma prateleira, a base de um carrinho e as áreas onde não entram itens novos.
    /// </summary>
    /// <remarks>
    /// Em <see cref="Vector2"/>, <c>x</c> é o X do mundo e <c>y</c> é o Z do mundo.
    /// Serializável, para ser editado no Inspector dentro de um asset.
    /// </remarks>
    [Serializable]
    public struct Retangulo
    {
        [Tooltip("Centro do retângulo no chão: x = X do mundo, y = Z do mundo.")]
        [SerializeField] private Vector2 centro;

        [Tooltip("Largura (X do mundo) e profundidade (Z do mundo), em metros.")]
        [SerializeField] private Vector2 tamanho;

        /// <summary>Cria um retângulo pelo centro e pelo tamanho.</summary>
        /// <exception cref="ArgumentException">Se o centro não for finito ou o tamanho não for finito e maior que zero.</exception>
        public Retangulo(Vector2 centro, Vector2 tamanho)
        {
            if (!Finito(centro.x) || !Finito(centro.y))
            {
                throw new ArgumentException("O centro precisa ser finito.", nameof(centro));
            }

            if (!Finito(tamanho.x) || !Finito(tamanho.y) || tamanho.x <= 0f || tamanho.y <= 0f)
            {
                throw new ArgumentException("O tamanho precisa ser finito e maior que zero.", nameof(tamanho));
            }

            this.centro = centro;
            this.tamanho = tamanho;
        }

        /// <summary>Cria um retângulo pelos cantos (em qualquer ordem).</summary>
        /// <exception cref="ArgumentException">Se os cantos não formarem um retângulo de área maior que zero.</exception>
        public static Retangulo PelosCantos(Vector2 a, Vector2 b) =>
            new Retangulo((a + b) * 0.5f, new Vector2(Mathf.Abs(b.x - a.x), Mathf.Abs(b.y - a.y)));

        /// <summary>Centro no chão.</summary>
        public Vector2 Centro => centro;

        /// <summary>Largura e profundidade.</summary>
        public Vector2 Tamanho => tamanho;

        /// <summary>Canto de menor x e menor z.</summary>
        public Vector2 Minimo => centro - tamanho * 0.5f;

        /// <summary>Canto de maior x e maior z.</summary>
        public Vector2 Maximo => centro + tamanho * 0.5f;

        /// <summary>Indica se o tamanho é finito e maior que zero (um asset editado à mão pode ter zero).</summary>
        public bool Valido => Finito(centro.x) && Finito(centro.y) && Finito(tamanho.x) && Finito(tamanho.y)
                              && tamanho.x > 0f && tamanho.y > 0f;

        /// <summary>Indica se o ponto está dentro do retângulo (a borda conta como dentro).</summary>
        public bool Contem(Vector2 ponto)
        {
            var min = Minimo;
            var max = Maximo;
            return ponto.x >= min.x && ponto.x <= max.x && ponto.y >= min.y && ponto.y <= max.y;
        }

        /// <summary>Ponto do retângulo mais perto do ponto dado (o próprio ponto, se estiver dentro).</summary>
        public Vector2 PontoMaisProximo(Vector2 ponto)
        {
            var min = Minimo;
            var max = Maximo;
            return new Vector2(Mathf.Clamp(ponto.x, min.x, max.x), Mathf.Clamp(ponto.y, min.y, max.y));
        }

        /// <summary>Distância do ponto até o retângulo (zero se estiver dentro).</summary>
        public float DistanciaAte(Vector2 ponto) => Vector2.Distance(ponto, PontoMaisProximo(ponto));

        /// <summary>O mesmo retângulo com outro centro.</summary>
        public Retangulo ComCentro(Vector2 novoCentro) => new Retangulo(novoCentro, tamanho);

        private static bool Finito(float valor) => !float.IsNaN(valor) && !float.IsInfinity(valor);
    }
}
