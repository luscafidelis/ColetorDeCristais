using System;

namespace ColetorDeCristais.Animacao
{
    /// <summary>
    /// Valores que controlam a <see cref="CalculadoraDeDeformacao"/>: o pulsar parado,
    /// o esticar e achatar ao andar e a inclinação na direção do movimento.
    /// O construtor valida tudo; um valor inválido lança <see cref="ArgumentException"/>.
    /// </summary>
    public readonly struct ParametrosDeDeformacao
    {
        /// <summary>Quanto o corpo cresce e encolhe parado (0,04 = ±4%).</summary>
        public float AmplitudeDoPulso { get; }

        /// <summary>Duração de um pulso completo parado, em segundos.</summary>
        public float PeriodoDoPulso { get; }

        /// <summary>Quanto o corpo estica e achata andando na velocidade de referência (0,1 = 10%).</summary>
        public float EstiramentoMaximo { get; }

        /// <summary>Duração de um ciclo de esticar e achatar na velocidade de referência, em segundos.</summary>
        public float PeriodoDoPasso { get; }

        /// <summary>Inclinação máxima na direção do movimento, em graus.</summary>
        public float InclinacaoMaxima { get; }

        /// <summary>Velocidade (m/s) em que o efeito de andar fica completo e o pulso parado some.</summary>
        public float VelocidadeDeReferencia { get; }

        /// <summary>Tempo (s) que a animação leva para acompanhar uma mudança de velocidade; 0 acompanha na hora.</summary>
        public float TempoDeResposta { get; }

        /// <summary>Quanto o corpo estica para a frente durante a investida (0,3 = 30%); 0 desliga.</summary>
        public float EstiramentoDaInvestida { get; }

        /// <param name="amplitudeDoPulso">Fração entre 0 e 1 (exclusive).</param>
        /// <param name="periodoDoPulso">Maior que zero.</param>
        /// <param name="estiramentoMaximo">Fração entre 0 e 1 (exclusive).</param>
        /// <param name="periodoDoPasso">Maior que zero.</param>
        /// <param name="inclinacaoMaxima">Graus entre 0 e 90 (exclusive).</param>
        /// <param name="velocidadeDeReferencia">Maior que zero.</param>
        /// <param name="tempoDeResposta">Zero ou mais.</param>
        /// <param name="estiramentoDaInvestida">Fração entre 0 e 1 (exclusive); padrão 0.</param>
        /// <exception cref="ArgumentException">Se algum valor estiver fora da faixa ou não for finito.</exception>
        public ParametrosDeDeformacao(
            float amplitudeDoPulso,
            float periodoDoPulso,
            float estiramentoMaximo,
            float periodoDoPasso,
            float inclinacaoMaxima,
            float velocidadeDeReferencia,
            float tempoDeResposta,
            float estiramentoDaInvestida = 0f)
        {
            ValidarFracao(amplitudeDoPulso, nameof(amplitudeDoPulso));
            ValidarPositivo(periodoDoPulso, nameof(periodoDoPulso));
            ValidarFracao(estiramentoMaximo, nameof(estiramentoMaximo));
            ValidarPositivo(periodoDoPasso, nameof(periodoDoPasso));
            ValidarFinito(inclinacaoMaxima, nameof(inclinacaoMaxima));
            if (inclinacaoMaxima < 0f || inclinacaoMaxima >= 90f)
            {
                throw new ArgumentException("A inclinação máxima precisa estar entre 0 e 90 graus.", nameof(inclinacaoMaxima));
            }
            ValidarPositivo(velocidadeDeReferencia, nameof(velocidadeDeReferencia));
            ValidarFinito(tempoDeResposta, nameof(tempoDeResposta));
            if (tempoDeResposta < 0f)
            {
                throw new ArgumentException("O tempo de resposta não pode ser negativo.", nameof(tempoDeResposta));
            }

            ValidarFracao(estiramentoDaInvestida, nameof(estiramentoDaInvestida));

            AmplitudeDoPulso = amplitudeDoPulso;
            PeriodoDoPulso = periodoDoPulso;
            EstiramentoMaximo = estiramentoMaximo;
            PeriodoDoPasso = periodoDoPasso;
            InclinacaoMaxima = inclinacaoMaxima;
            VelocidadeDeReferencia = velocidadeDeReferencia;
            TempoDeResposta = tempoDeResposta;
            EstiramentoDaInvestida = estiramentoDaInvestida;
        }

        private static void ValidarFracao(float valor, string nome)
        {
            ValidarFinito(valor, nome);
            if (valor < 0f || valor >= 1f)
            {
                throw new ArgumentException("O valor precisa estar entre 0 e 1 (exclusive).", nome);
            }
        }

        private static void ValidarPositivo(float valor, string nome)
        {
            ValidarFinito(valor, nome);
            if (valor <= 0f)
            {
                throw new ArgumentException("O valor precisa ser maior que zero.", nome);
            }
        }

        private static void ValidarFinito(float valor, string nome)
        {
            if (float.IsNaN(valor) || float.IsInfinity(valor))
            {
                throw new ArgumentException("O valor precisa ser um número finito.", nome);
            }
        }
    }
}
