using System;
using UnityEngine;

namespace ColetorDeCristais.Habilidades
{
    /// <summary>
    /// Gera, em C# puro, os vértices e triângulos de um anel plano no chão (plano XZ, virado para cima), centrado
    /// na origem. Usado pelo <see cref="IndicadorDaInvestida"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Os vértices vêm aos pares por segmento: o interno no índice <c>2i</c> e o externo em <c>2i + 1</c>.</item>
    /// <item>Cada segmento tem 2 triângulos, na ordem horária vista de cima (a face da frente na Unity), então o
    /// anel aparece para uma câmera acima dele.</item>
    /// </list>
    /// </remarks>
    public static class MalhaDeAnel
    {
        /// <summary>Menor quantidade de segmentos aceita.</summary>
        public const int SegmentosMinimos = 3;

        /// <summary>Gera o anel.</summary>
        /// <param name="raioInterno">Raio do furo (≥ 0).</param>
        /// <param name="raioExterno">Raio de fora (maior que o interno).</param>
        /// <param name="segmentos">Quantidade de segmentos da volta (≥ 3).</param>
        /// <param name="vertices">Os vértices, com y = 0.</param>
        /// <param name="triangulos">Os índices dos triângulos, 3 por triângulo.</param>
        /// <exception cref="ArgumentException">Se os raios não forem finitos ou não seguirem as regras acima, ou se
        /// houver menos de 3 segmentos.</exception>
        public static void Gerar(float raioInterno, float raioExterno, int segmentos, out Vector3[] vertices, out int[] triangulos)
        {
            if (float.IsNaN(raioInterno) || float.IsInfinity(raioInterno) || raioInterno < 0f)
            {
                throw new ArgumentException("O raio interno precisa ser finito e maior ou igual a zero.", nameof(raioInterno));
            }

            if (float.IsNaN(raioExterno) || float.IsInfinity(raioExterno) || raioExterno <= raioInterno)
            {
                throw new ArgumentException("O raio externo precisa ser finito e maior que o interno.", nameof(raioExterno));
            }

            if (segmentos < SegmentosMinimos)
            {
                throw new ArgumentException($"O anel precisa de pelo menos {SegmentosMinimos} segmentos.", nameof(segmentos));
            }

            vertices = new Vector3[segmentos * 2];
            triangulos = new int[segmentos * 6];
            for (var i = 0; i < segmentos; i++)
            {
                var angulo = 2.0 * Math.PI * i / segmentos;
                var cos = (float)Math.Cos(angulo);
                var sen = (float)Math.Sin(angulo);
                vertices[2 * i] = new Vector3(cos * raioInterno, 0f, sen * raioInterno);
                vertices[2 * i + 1] = new Vector3(cos * raioExterno, 0f, sen * raioExterno);

                var interno = 2 * i;
                var externo = 2 * i + 1;
                var internoSeguinte = 2 * ((i + 1) % segmentos);
                var externoSeguinte = internoSeguinte + 1;
                var t = i * 6;
                triangulos[t] = interno;
                triangulos[t + 1] = internoSeguinte;
                triangulos[t + 2] = externo;
                triangulos[t + 3] = externo;
                triangulos[t + 4] = internoSeguinte;
                triangulos[t + 5] = externoSeguinte;
            }
        }
    }
}
