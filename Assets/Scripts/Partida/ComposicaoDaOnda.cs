using System;

namespace ColetorDeCristais.Partida
{
    /// <summary>
    /// Quantos itens novos cada onda traz, em C# puro. A primeira onda usa os itens já colocados na cena;
    /// a partir da segunda, entram itens novos (somados aos que sobraram):
    /// <list type="bullet">
    /// <item>as mesmas comidas da primeira onda;</item>
    /// <item>cristais de experiência em dobro a cada onda (ex.: 4, 8, 16);</item>
    /// <item>um item de habilidade (café ou rosquinha), sorteado.</item>
    /// </list>
    /// </summary>
    public static class ComposicaoDaOnda
    {
        /// <summary>Cristais de experiência da onda: o da primeira onda × 2^(onda - 1).</summary>
        /// <param name="onda">Onda (1 ou mais).</param>
        /// <param name="naPrimeiraOnda">Cristais de experiência na primeira onda (zero ou mais).</param>
        /// <exception cref="ArgumentException">Se a onda for menor que 1, a base for negativa ou o resultado não couber num int.</exception>
        public static int CristaisDeExperiencia(int onda, int naPrimeiraOnda)
        {
            if (onda < 1)
            {
                throw new ArgumentException("A onda começa em 1.", nameof(onda));
            }

            if (naPrimeiraOnda < 0)
            {
                throw new ArgumentException("A quantidade não pode ser negativa.", nameof(naPrimeiraOnda));
            }

            return checked(naPrimeiraOnda * (1 << Math.Min(onda - 1, 30)));
        }

        /// <summary>Indica se a onda traz itens novos (só a partir da segunda; a primeira usa os itens da cena).</summary>
        public static bool TrazItensNovos(int onda) => onda >= 2;

        /// <summary>Sorteia qual item de habilidade entra na onda.</summary>
        /// <param name="quantidadeDeOpcoes">Quantos itens de habilidade existem (ex.: 2, café e rosquinha).</param>
        /// <param name="sorteio">Gerador de números aleatórios.</param>
        /// <returns>Índice do item sorteado, ou -1 se não houver opções.</returns>
        public static int SortearItemDeHabilidade(int quantidadeDeOpcoes, Random sorteio)
        {
            if (sorteio == null)
            {
                throw new ArgumentNullException(nameof(sorteio));
            }

            return quantidadeDeOpcoes <= 0 ? -1 : sorteio.Next(quantidadeDeOpcoes);
        }
    }
}
