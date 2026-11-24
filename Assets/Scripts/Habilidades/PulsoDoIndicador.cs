using System;

namespace ColetorDeCristais.Habilidades
{
    /// <summary>
    /// Estado do indicador da investida, em C# puro: aparece só com a investida pronta e dá um pulso (cresce e volta)
    /// no momento em que ela fica pronta.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>O pulso acontece em toda passagem de "não pronta" para "pronta": quando a recarga acaba, quando a
    /// investida é liberada e na primeira atualização já pronta.</item>
    /// <item>A escala segue meia senoide: 1 no início, o pico no meio da duração e 1 de novo no fim.</item>
    /// <item>Deixar de estar pronta esconde o indicador e cancela o pulso.</item>
    /// </list>
    /// </remarks>
    public class PulsoDoIndicador
    {
        private readonly float _escalaDoPico;
        private readonly float _duracao;
        private bool _prontaAntes;
        private double _tempoDoPulso = double.PositiveInfinity;

        /// <summary>Cria o estado do indicador, escondido.</summary>
        /// <param name="escalaDoPico">Escala no meio do pulso (≥ 1; 1 desliga o pulso).</param>
        /// <param name="duracao">Duração do pulso em segundos (≥ 0; 0 desliga o pulso).</param>
        /// <exception cref="ArgumentException">Se algum valor for inválido.</exception>
        public PulsoDoIndicador(float escalaDoPico, float duracao)
        {
            if (float.IsNaN(escalaDoPico) || float.IsInfinity(escalaDoPico) || escalaDoPico < 1f)
            {
                throw new ArgumentException("A escala do pico precisa ser finita e maior ou igual a 1.", nameof(escalaDoPico));
            }

            if (float.IsNaN(duracao) || float.IsInfinity(duracao) || duracao < 0f)
            {
                throw new ArgumentException("A duração do pulso precisa ser finita e maior ou igual a zero.", nameof(duracao));
            }

            _escalaDoPico = escalaDoPico;
            _duracao = duracao;
        }

        /// <summary>Indica se o indicador aparece.</summary>
        public bool Visivel { get; private set; }

        /// <summary>Escala do indicador agora (1 fora do pulso).</summary>
        public float Escala { get; private set; } = 1f;

        /// <summary>Indica se o pulso está acontecendo.</summary>
        public bool EmPulso => Visivel && _tempoDoPulso < _duracao;

        /// <summary>Avança o tempo e atualiza <see cref="Visivel"/> e <see cref="Escala"/>.</summary>
        /// <param name="pronta">Se a investida está pronta agora.</param>
        /// <param name="deltaTempo">Tempo do quadro (negativo conta como zero).</param>
        public void Atualizar(bool pronta, float deltaTempo)
        {
            if (!pronta)
            {
                _prontaAntes = false;
                _tempoDoPulso = double.PositiveInfinity;
                Visivel = false;
                Escala = 1f;
                return;
            }

            if (!_prontaAntes)
            {
                // Ficou pronta agora: o pulso começa neste quadro, na escala 1.
                _prontaAntes = true;
                _tempoDoPulso = 0.0;
            }
            else if (deltaTempo > 0f)
            {
                _tempoDoPulso += deltaTempo;
            }

            Visivel = true;
            Escala = EscalaNoTempo(_tempoDoPulso);
        }

        private float EscalaNoTempo(double tempo)
        {
            if (_duracao <= 0f || tempo >= _duracao)
            {
                return 1f;
            }

            var u = tempo / _duracao;
            return 1f + (_escalaDoPico - 1f) * (float)Math.Sin(Math.PI * u);
        }
    }
}
