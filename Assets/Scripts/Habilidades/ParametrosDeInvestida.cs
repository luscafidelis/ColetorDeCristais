using System;

namespace ColetorDeCristais.Habilidades
{
    /// <summary>
    /// Valores da <see cref="Investida"/>: quantas vezes mais rápida que o movimento normal,
    /// por quanto tempo e a recarga. O construtor valida tudo.
    /// </summary>
    public readonly struct ParametrosDeInvestida
    {
        /// <summary>Velocidade da investida = velocidade de movimento × este multiplicador.</summary>
        public float MultiplicadorDeVelocidade { get; }

        /// <summary>Duração da investida, em segundos.</summary>
        public float Duracao { get; }

        /// <summary>Tempo até poder usar de novo, em segundos, contado a partir do início da investida.</summary>
        public float Recarga { get; }

        /// <param name="multiplicadorDeVelocidade">Maior que zero.</param>
        /// <param name="duracao">Maior que zero.</param>
        /// <param name="recarga">Zero ou mais.</param>
        /// <exception cref="ArgumentException">Se algum valor estiver fora da faixa ou não for finito.</exception>
        public ParametrosDeInvestida(float multiplicadorDeVelocidade, float duracao, float recarga)
        {
            ValidarFinito(multiplicadorDeVelocidade, nameof(multiplicadorDeVelocidade));
            ValidarFinito(duracao, nameof(duracao));
            ValidarFinito(recarga, nameof(recarga));
            if (multiplicadorDeVelocidade <= 0f)
            {
                throw new ArgumentException("O multiplicador precisa ser maior que zero.", nameof(multiplicadorDeVelocidade));
            }

            if (duracao <= 0f)
            {
                throw new ArgumentException("A duração precisa ser maior que zero.", nameof(duracao));
            }

            if (recarga < 0f)
            {
                throw new ArgumentException("A recarga não pode ser negativa.", nameof(recarga));
            }

            MultiplicadorDeVelocidade = multiplicadorDeVelocidade;
            Duracao = duracao;
            Recarga = recarga;
        }

        /// <summary>Distância percorrida numa investida sem obstáculos, para uma velocidade de movimento.</summary>
        /// <param name="velocidadeDeMovimento">Velocidade normal do personagem, em m/s; negativa conta como zero.</param>
        public float Distancia(float velocidadeDeMovimento) =>
            Math.Max(0f, velocidadeDeMovimento) * MultiplicadorDeVelocidade * Duracao;

        private static void ValidarFinito(float valor, string nome)
        {
            if (float.IsNaN(valor) || float.IsInfinity(valor))
            {
                throw new ArgumentException("O valor precisa ser um número finito.", nome);
            }
        }
    }
}
