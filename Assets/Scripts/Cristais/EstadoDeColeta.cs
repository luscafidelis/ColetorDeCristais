namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Regra de coleta de um cristal, em C# puro: só um coletor válido coleta,
    /// e cada cristal é coletado no máximo uma vez.
    /// </summary>
    /// <remarks>
    /// "Uma vez" importa porque o motor de física pode avisar mais de uma entrada no trigger
    /// no mesmo quadro (ex.: o jogador encostando com dois colliders), antes de o cristal ser destruído.
    /// </remarks>
    public sealed class EstadoDeColeta
    {
        /// <summary>Indica se o cristal já foi coletado.</summary>
        public bool Coletado { get; private set; }

        /// <summary>
        /// Tenta coletar o cristal.
        /// </summary>
        /// <param name="coletorValido"><c>true</c> se quem encostou pode coletar (o jogador).</param>
        /// <returns><c>true</c> só na primeira coleta válida; nas demais, <c>false</c>.</returns>
        public bool TentarColetar(bool coletorValido)
        {
            if (!coletorValido || Coletado)
            {
                return false;
            }

            Coletado = true;
            return true;
        }
    }
}
