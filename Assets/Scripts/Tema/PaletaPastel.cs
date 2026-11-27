using UnityEngine;

namespace ColetorDeCristais.Tema
{
    /// <summary>
    /// Converte uma cor para a versão pastel do mundo "fofinho", em C# puro: clareia (puxa o brilho para o branco)
    /// e tira parte da saturação, mantendo o matiz e a transparência.
    /// </summary>
    /// <remarks>
    /// Usada pelo <c>GeradorDeTexturasPastel</c> para criar as texturas pastel dos kits da Kenney, com os valores de
    /// <see cref="PaletaDoTema.ClareamentoDoMundo"/> e <see cref="PaletaDoTema.SaturacaoDoMundo"/>.
    /// </remarks>
    public static class PaletaPastel
    {
        /// <summary>Converte a cor.</summary>
        /// <param name="cor">Cor original (sRGB, de 0 a 1).</param>
        /// <param name="clareamento">Quanto o brilho anda até o branco, de 0 (nada) a 1 (tudo branco). Fora da faixa, vale o limite.</param>
        /// <param name="fatorDeSaturacao">Quanto da saturação fica, de 0 (cinza) a 1 (igual). Fora da faixa, vale o limite.</param>
        public static Color Converter(Color cor, float clareamento, float fatorDeSaturacao)
        {
            var clarear = Limitar(clareamento);
            var saturar = Limitar(fatorDeSaturacao);
            Color.RGBToHSV(cor, out var matiz, out var saturacao, out var brilho);
            var resultado = Color.HSVToRGB(matiz, saturacao * saturar, brilho + (1f - brilho) * clarear);
            resultado.a = cor.a;
            return resultado;
        }

        private static float Limitar(float valor) => float.IsNaN(valor) ? 0f : Mathf.Clamp01(valor);
    }
}
