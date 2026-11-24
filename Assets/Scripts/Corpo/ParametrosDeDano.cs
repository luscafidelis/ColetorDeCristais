using System;

namespace ColetorDeCristais.Corpo
{
    /// <summary>
    /// Valores do dano no corpo: quanto uma batida encolhe, o tamanho mínimo, o tranco e a invulnerabilidade.
    /// O construtor valida tudo.
    /// </summary>
    public readonly struct ParametrosDeDano
    {
        /// <summary>Quanto cada batida tira do tamanho (ex.: 1/6).</summary>
        public float Dano { get; }

        /// <summary>Abaixo deste tamanho, o personagem é derrotado (ex.: 1/16).</summary>
        public float TamanhoMinimo { get; }

        /// <summary>Distância do tranco, em metros.</summary>
        public float DistanciaDoTranco { get; }

        /// <summary>Duração do tranco, em segundos.</summary>
        public float DuracaoDoTranco { get; }

        /// <summary>Segundos sem receber outro dano depois de uma batida.</summary>
        public float TempoDeInvulnerabilidade { get; }

        /// <param name="dano">Zero ou mais.</param>
        /// <param name="tamanhoMinimo">Zero ou mais.</param>
        /// <param name="distanciaDoTranco">Zero ou mais.</param>
        /// <param name="duracaoDoTranco">Maior que zero.</param>
        /// <param name="tempoDeInvulnerabilidade">Zero ou mais.</param>
        /// <exception cref="ArgumentException">Se algum valor estiver fora da faixa ou não for finito.</exception>
        public ParametrosDeDano(float dano, float tamanhoMinimo, float distanciaDoTranco, float duracaoDoTranco, float tempoDeInvulnerabilidade)
        {
            ValidarNaoNegativo(dano, nameof(dano));
            ValidarNaoNegativo(tamanhoMinimo, nameof(tamanhoMinimo));
            ValidarNaoNegativo(distanciaDoTranco, nameof(distanciaDoTranco));
            ValidarNaoNegativo(tempoDeInvulnerabilidade, nameof(tempoDeInvulnerabilidade));
            ValidarNaoNegativo(duracaoDoTranco, nameof(duracaoDoTranco));
            if (duracaoDoTranco <= 0f)
            {
                throw new ArgumentException("A duração do tranco precisa ser maior que zero.", nameof(duracaoDoTranco));
            }

            Dano = dano;
            TamanhoMinimo = tamanhoMinimo;
            DistanciaDoTranco = distanciaDoTranco;
            DuracaoDoTranco = duracaoDoTranco;
            TempoDeInvulnerabilidade = tempoDeInvulnerabilidade;
        }

        private static void ValidarNaoNegativo(float valor, string nome)
        {
            if (float.IsNaN(valor) || float.IsInfinity(valor) || valor < 0f)
            {
                throw new ArgumentException("O valor precisa ser finito e não negativo.", nome);
            }
        }
    }
}
