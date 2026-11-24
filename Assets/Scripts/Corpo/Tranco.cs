using System;
using UnityEngine;

namespace ColetorDeCristais.Corpo
{
    /// <summary>
    /// Tranco no chão, em C# puro: um deslocamento numa direção, rápido no começo e desacelerando até parar
    /// (fração percorrida = 1 − (1 − t/duração)²).
    /// </summary>
    /// <remarks>
    /// O deslocamento de cada quadro vem da fração acumulada (em <c>double</c>), então o total é exatamente a
    /// distância em qualquer taxa de quadros. Vetores no chão: <c>x</c> = X do mundo, <c>y</c> = Z do mundo.
    /// </remarks>
    public sealed class Tranco
    {
        private Vector2 _direcao;
        private float _distancia;
        private double _duracao;
        private double _tempo;

        /// <summary>Indica se o tranco ainda está andando.</summary>
        public bool EmAndamento { get; private set; }

        /// <summary>Começa um tranco (substitui o anterior). Sem direção ou sem distância, não faz nada.</summary>
        /// <param name="direcao">Direção no chão (não precisa estar normalizada).</param>
        /// <param name="distancia">Distância total, em metros.</param>
        /// <param name="duracao">Duração, em segundos (maior que zero).</param>
        /// <exception cref="ArgumentException">Se a distância for negativa, a duração não for positiva ou algum valor não for finito.</exception>
        public void Iniciar(Vector2 direcao, float distancia, float duracao)
        {
            if (float.IsNaN(distancia) || float.IsInfinity(distancia) || distancia < 0f)
            {
                throw new ArgumentException("A distância precisa ser finita e não negativa.", nameof(distancia));
            }

            if (float.IsNaN(duracao) || float.IsInfinity(duracao) || duracao <= 0f)
            {
                throw new ArgumentException("A duração precisa ser finita e maior que zero.", nameof(duracao));
            }

            if (direcao.sqrMagnitude < 1e-12f || distancia == 0f)
            {
                Cancelar();
                return;
            }

            _direcao = direcao.normalized;
            _distancia = distancia;
            _duracao = duracao;
            _tempo = 0d;
            EmAndamento = true;
        }

        /// <summary>Para o tranco onde está.</summary>
        public void Cancelar() => EmAndamento = false;

        /// <summary>Avança o tranco e devolve o deslocamento deste quadro (zero se parado).</summary>
        /// <param name="deltaTempo">Segundos desde o último quadro (zero ou mais).</param>
        /// <exception cref="ArgumentException">Se o tempo for negativo ou não finito.</exception>
        public Vector2 Atualizar(float deltaTempo)
        {
            if (float.IsNaN(deltaTempo) || float.IsInfinity(deltaTempo) || deltaTempo < 0f)
            {
                throw new ArgumentException("O tempo do quadro precisa ser finito e não negativo.", nameof(deltaTempo));
            }

            if (!EmAndamento)
            {
                return Vector2.zero;
            }

            var antes = Fracao(_tempo / _duracao);
            _tempo += deltaTempo;
            var depois = Fracao(_tempo / _duracao);
            if (_tempo >= _duracao)
            {
                EmAndamento = false;
            }

            return _direcao * (float)((depois - antes) * _distancia);
        }

        private static double Fracao(double progresso)
        {
            var u = Math.Min(1d, Math.Max(0d, progresso));
            return 1d - (1d - u) * (1d - u);
        }
    }
}
