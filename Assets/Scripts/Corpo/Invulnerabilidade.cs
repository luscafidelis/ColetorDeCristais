using System;

namespace ColetorDeCristais.Corpo
{
    /// <summary>
    /// Tempo sem receber dano depois de uma batida, em C# puro, com o pisca-pisca que mostra isso na tela.
    /// </summary>
    public sealed class Invulnerabilidade
    {
        // Em double: somar muitos quadros em float deixaria um resto minúsculo no fim.
        private double _restante;
        private double _decorrido;

        /// <summary>Indica se o personagem está invulnerável agora.</summary>
        public bool Ativa => _restante > 0d;

        /// <summary>Começa (ou recomeça) a invulnerabilidade.</summary>
        /// <param name="duracao">Segundos (zero ou mais; zero não deixa invulnerável).</param>
        /// <exception cref="ArgumentException">Se a duração for negativa ou não finita.</exception>
        public void Iniciar(float duracao)
        {
            if (float.IsNaN(duracao) || float.IsInfinity(duracao) || duracao < 0f)
            {
                throw new ArgumentException("A duração precisa ser finita e não negativa.", nameof(duracao));
            }

            _restante = duracao;
            _decorrido = 0d;
        }

        /// <summary>Encerra a invulnerabilidade agora.</summary>
        public void Encerrar() => _restante = 0d;

        /// <summary>Faz o tempo passar.</summary>
        /// <exception cref="ArgumentException">Se o tempo for negativo ou não finito.</exception>
        public void Atualizar(float deltaTempo)
        {
            if (float.IsNaN(deltaTempo) || float.IsInfinity(deltaTempo) || deltaTempo < 0f)
            {
                throw new ArgumentException("O tempo do quadro precisa ser finito e não negativo.", nameof(deltaTempo));
            }

            if (!Ativa)
            {
                return;
            }

            _restante = Math.Max(0d, _restante - deltaTempo);
            _decorrido += deltaTempo;
        }

        /// <summary>
        /// Se o personagem aparece neste momento: sempre fora da invulnerabilidade; durante ela, alterna entre
        /// aparecer e sumir, começando aparecendo.
        /// </summary>
        /// <param name="piscadasPorSegundo">Quantas vezes por segundo ele some e volta (zero ou menos: não pisca).</param>
        public bool Visivel(float piscadasPorSegundo)
        {
            if (!Ativa || piscadasPorSegundo <= 0f || float.IsNaN(piscadasPorSegundo))
            {
                return true;
            }

            var meiosCiclos = (long)Math.Floor(_decorrido * piscadasPorSegundo * 2d);
            return meiosCiclos % 2 == 0;
        }
    }
}
