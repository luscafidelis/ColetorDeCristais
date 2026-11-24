namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Quanto o ímã do jogador puxa um tipo de cristal. Definido em cada <see cref="DefinicaoCristal"/>;
    /// os valores de cada nível (raio e velocidade) ficam na <c>DefinicaoIma</c>.
    /// </summary>
    public enum AtracaoDoIma
    {
        /// <summary>Raio e velocidade completos (ex.: cristais de experiência).</summary>
        Completa = 0,

        /// <summary>Raio e velocidade reduzidos (ex.: comidas com efeito).</summary>
        Reduzida = 1,

        /// <summary>O ímã não puxa (ex.: itens que liberam habilidades).</summary>
        Nenhuma = 2,
    }
}
