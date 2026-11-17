namespace ColetorDeCristais.Atributos
{
    /// <summary>
    /// Forma como um <see cref="Modificador"/> altera o valor de um <see cref="Atributo"/>.
    /// A ordem de aplicação é fixa e segue a ordem desta enumeração:
    /// primeiro os planos, depois os percentuais aditivos e por fim os percentuais multiplicativos.
    /// </summary>
    public enum TipoModificador
    {
        /// <summary>
        /// Soma o valor diretamente ao valor base. Ex.: <c>2</c> soma 2 unidades.
        /// </summary>
        Plano = 0,

        /// <summary>
        /// Percentual que se soma aos outros percentuais aditivos antes de multiplicar.
        /// Usa fração: <c>0.1</c> é +10%. Dois de +10% dão +20% no total.
        /// </summary>
        PercentualAditivo = 1,

        /// <summary>
        /// Percentual que multiplica o resultado de forma independente, depois dos aditivos.
        /// Usa fração: <c>0.1</c> é ×1,1. Dois de +10% dão ×1,21 no total.
        /// </summary>
        PercentualMultiplicativo = 2,
    }
}
