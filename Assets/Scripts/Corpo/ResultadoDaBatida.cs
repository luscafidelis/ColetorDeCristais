namespace ColetorDeCristais.Corpo
{
    /// <summary>O que uma batida fez.</summary>
    public enum ResultadoDaBatida
    {
        /// <summary>Nada: o personagem estava invulnerável.</summary>
        Ignorada = 0,

        /// <summary>O personagem encolheu e levou um tranco.</summary>
        Dano = 1,

        /// <summary>O personagem encolheu abaixo do mínimo: derrota.</summary>
        Derrota = 2,
    }
}
