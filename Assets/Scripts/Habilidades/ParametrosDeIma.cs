using System;

namespace ColetorDeCristais.Habilidades
{
    /// <summary>
    /// Valores de um nível de atração do <see cref="Ima"/>: o raio e a velocidade com que o item vem.
    /// A velocidade cresce em linha reta da borda do raio até o jogador:
    /// na borda vale <c>VelocidadeMaxima × FracaoNaBorda</c>, junto do jogador vale <c>VelocidadeMaxima</c>.
    /// O construtor valida tudo.
    /// </summary>
    public readonly struct ParametrosDeIma
    {
        /// <summary>Distância horizontal máxima, em metros, para o item ser puxado.</summary>
        public float Raio { get; }

        /// <summary>Velocidade do item junto do jogador, em m/s.</summary>
        public float VelocidadeMaxima { get; }

        /// <summary>Fração da velocidade máxima na borda do raio (1 = velocidade constante).</summary>
        public float FracaoNaBorda { get; }

        /// <param name="raio">Maior que zero.</param>
        /// <param name="velocidadeMaxima">Maior que zero.</param>
        /// <param name="fracaoNaBorda">Entre 0 e 1.</param>
        /// <exception cref="ArgumentException">Se algum valor estiver fora da faixa ou não for finito.</exception>
        public ParametrosDeIma(float raio, float velocidadeMaxima, float fracaoNaBorda)
        {
            ValidarFinito(raio, nameof(raio));
            ValidarFinito(velocidadeMaxima, nameof(velocidadeMaxima));
            ValidarFinito(fracaoNaBorda, nameof(fracaoNaBorda));
            if (raio <= 0f)
            {
                throw new ArgumentException("O raio precisa ser maior que zero.", nameof(raio));
            }

            if (velocidadeMaxima <= 0f)
            {
                throw new ArgumentException("A velocidade precisa ser maior que zero.", nameof(velocidadeMaxima));
            }

            if (fracaoNaBorda < 0f || fracaoNaBorda > 1f)
            {
                throw new ArgumentException("A fração na borda precisa estar entre 0 e 1.", nameof(fracaoNaBorda));
            }

            Raio = raio;
            VelocidadeMaxima = velocidadeMaxima;
            FracaoNaBorda = fracaoNaBorda;
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
