namespace ColetorDeCristais.Partida
{
    /// <summary>Fase em que a <see cref="CicloDaPartida"/> está.</summary>
    public enum EstadoDaPartida
    {
        /// <summary>Onda em andamento: o cronômetro corre e o jogador joga.</summary>
        EmOnda = 0,

        /// <summary>O tempo da onda acabou e vem o chefe da onda (por enquanto, um marcador de lugar).</summary>
        ChefeDaOnda = 1,

        /// <summary>O tempo da última onda acabou e vem o chefe da arena (por enquanto, um marcador de lugar).</summary>
        ChefeDaArena = 2,

        /// <summary>O chefe da arena foi derrotado: a arena foi concluída.</summary>
        Vitoria = 3,

        /// <summary>O jogador foi derrotado (ex.: vida zerada).</summary>
        Derrota = 4,
    }
}
