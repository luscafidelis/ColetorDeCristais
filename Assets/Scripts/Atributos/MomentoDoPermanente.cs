namespace ColetorDeCristais.Atributos
{
    /// <summary>Quando a parte permanente de um <see cref="BonusDeAtributo"/> entra no atributo.</summary>
    public enum MomentoDoPermanente
    {
        /// <summary>Junto com a parte temporária, no momento da aplicação.</summary>
        AoAplicar = 0,

        /// <summary>Quando o grupo da parte temporária não tiver mais temporários ativos.</summary>
        AoFimDoTemporario = 1,
    }
}
