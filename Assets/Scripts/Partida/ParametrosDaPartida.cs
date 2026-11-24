using System;

namespace ColetorDeCristais.Partida
{
    /// <summary>Valores da <see cref="CicloDaPartida"/>: duração de cada onda e quantidade de ondas da arena. O construtor valida.</summary>
    public readonly struct ParametrosDaPartida
    {
        /// <summary>Duração de cada onda, em segundos.</summary>
        public float DuracaoDaOnda { get; }

        /// <summary>Ondas por arena; o chefe da última é o chefe da arena.</summary>
        public int QuantidadeDeOndas { get; }

        /// <param name="duracaoDaOnda">Maior que zero e finita.</param>
        /// <param name="quantidadeDeOndas">Um ou mais.</param>
        /// <exception cref="ArgumentException">Se algum valor estiver fora da faixa.</exception>
        public ParametrosDaPartida(float duracaoDaOnda, int quantidadeDeOndas)
        {
            if (float.IsNaN(duracaoDaOnda) || float.IsInfinity(duracaoDaOnda) || duracaoDaOnda <= 0f)
            {
                throw new ArgumentException("A duração da onda precisa ser finita e maior que zero.", nameof(duracaoDaOnda));
            }

            if (quantidadeDeOndas < 1)
            {
                throw new ArgumentException("A arena precisa de pelo menos uma onda.", nameof(quantidadeDeOndas));
            }

            DuracaoDaOnda = duracaoDaOnda;
            QuantidadeDeOndas = quantidadeDeOndas;
        }
    }
}
