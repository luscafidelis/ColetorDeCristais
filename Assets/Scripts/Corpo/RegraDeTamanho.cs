using System;

namespace ColetorDeCristais.Corpo
{
    /// <summary>
    /// Regras do tamanho do personagem, em C# puro. O tamanho é um multiplicador da escala do modelo
    /// (1 = o tamanho original do montrengo, cerca de 2 m).
    /// </summary>
    public static class RegraDeTamanho
    {
        /// <summary>
        /// Altura do centro do personagem para os pés continuarem no chão depois de mudar de tamanho.
        /// Como o corpo escala a partir do centro, a altura do centro é proporcional ao tamanho.
        /// </summary>
        /// <param name="alturaNaEscalaUm">Altura do centro com tamanho 1 (no jogo, 1 m).</param>
        /// <param name="tamanho">Tamanho atual (zero ou mais).</param>
        /// <exception cref="ArgumentException">Se algum valor não for finito ou se o tamanho for negativo.</exception>
        public static float AlturaDoCentro(float alturaNaEscalaUm, float tamanho)
        {
            if (float.IsNaN(alturaNaEscalaUm) || float.IsInfinity(alturaNaEscalaUm))
            {
                throw new ArgumentException("A altura precisa ser um número finito.", nameof(alturaNaEscalaUm));
            }

            if (float.IsNaN(tamanho) || float.IsInfinity(tamanho) || tamanho < 0f)
            {
                throw new ArgumentException("O tamanho precisa ser finito e não negativo.", nameof(tamanho));
            }

            return alturaNaEscalaUm * tamanho;
        }

        /// <summary>
        /// Altura dos pés de um <c>CharacterController</c> no mundo: a base da cápsula, contando a escala do objeto.
        /// </summary>
        /// <param name="posicaoY">Altura do objeto no mundo.</param>
        /// <param name="centroY">Altura do centro da cápsula, local (<c>CharacterController.center.y</c>).</param>
        /// <param name="altura">Altura da cápsula, local (<c>CharacterController.height</c>).</param>
        /// <param name="escalaY">Escala do objeto no eixo Y (<c>lossyScale.y</c>).</param>
        public static float AlturaDosPes(float posicaoY, float centroY, float altura, float escalaY) =>
            posicaoY + (centroY - altura * 0.5f) * escalaY;

        /// <summary>Indica se o personagem foi derrotado por ficar pequeno demais (abaixo do mínimo; igual não conta).</summary>
        public static bool Derrotado(float tamanho, float minimo) => tamanho < minimo;
    }
}
