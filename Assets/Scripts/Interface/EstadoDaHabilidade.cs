namespace ColetorDeCristais.Interface
{
    /// <summary>Como o ícone de uma habilidade aparece na HUD (<see cref="IndicadorDeHabilidade"/>).</summary>
    public enum EstadoDaHabilidade
    {
        /// <summary>A habilidade ainda não foi obtida: ícone apagado.</summary>
        Bloqueada = 0,

        /// <summary>Obtida, mas em recarga: o ícone escurece e vai enchendo.</summary>
        EmRecarga = 1,

        /// <summary>Obtida e pronta para usar: ícone aceso.</summary>
        Pronta = 2,
    }
}
