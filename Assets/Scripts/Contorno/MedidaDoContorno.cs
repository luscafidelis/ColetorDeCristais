using UnityEngine;

namespace ColetorDeCristais.Contorno
{
    /// <summary>
    /// Conta da espessura do contorno, em C# puro: a espessura é dada em pixels numa tela de 1080 de altura e vira um
    /// deslocamento em coordenadas de textura (UV), para o contorno ter a mesma aparência em qualquer resolução.
    /// </summary>
    public static class MedidaDoContorno
    {
        /// <summary>Altura de tela em que a espessura vale exatamente o número de pixels pedido.</summary>
        public const float AlturaDeReferencia = 1080f;

        /// <summary>
        /// Deslocamento em UV para a espessura pedida. Na vertical, <c>espessura / 1080</c>; na horizontal, o mesmo
        /// número de pixels, convertido pela largura. Espessura inválida (negativa, NaN, infinita) ou tela sem tamanho
        /// dão zero (sem contorno).
        /// </summary>
        /// <param name="espessura">Espessura em pixels numa tela de 1080 de altura.</param>
        /// <param name="largura">Largura da imagem em pixels.</param>
        /// <param name="altura">Altura da imagem em pixels.</param>
        public static Vector2 Deslocamento(float espessura, int largura, int altura)
        {
            if (float.IsNaN(espessura) || float.IsInfinity(espessura) || espessura <= 0f || largura <= 0 || altura <= 0)
            {
                return Vector2.zero;
            }

            var vertical = espessura / AlturaDeReferencia;
            return new Vector2(vertical * altura / largura, vertical);
        }
    }
}
