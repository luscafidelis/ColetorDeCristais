using System;

namespace ColetorDeCristais.Partida
{
    /// <summary>
    /// Estado da partida numa arena, em C# puro: onda atual, cronômetro regressivo e a fase
    /// (<see cref="EstadoDaPartida"/>).
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Começa na onda 1, com o tempo cheio. O tempo só corre em <see cref="EstadoDaPartida.EmOnda"/>.</item>
    /// <item>Quando o tempo acaba (ou a onda é encerrada antes, por <see cref="EncerrarOnda"/>), vem o chefe: o da onda,
    /// ou o da arena na última onda.</item>
    /// <item><see cref="DerrotarChefe"/> passa para a próxima onda (tempo cheio) ou, depois do chefe da arena, para a vitória.</item>
    /// <item><see cref="DerrotarJogador"/> leva à derrota (ex.: vida zerada) a partir de qualquer fase em jogo.</item>
    /// <item>Vitória e derrota encerram a partida; <see cref="Reiniciar"/> volta ao começo.</item>
    /// </list>
    /// </remarks>
    public class CicloDaPartida
    {
        private readonly ParametrosDaPartida _parametros;

        /// <summary>Cria a partida já na onda 1.</summary>
        public CicloDaPartida(ParametrosDaPartida parametros)
        {
            _parametros = parametros;
            Reiniciar();
        }

        /// <summary>Onda atual (começa em 1).</summary>
        public int Onda { get; private set; }

        /// <summary>Quantidade de ondas da arena.</summary>
        public int QuantidadeDeOndas => _parametros.QuantidadeDeOndas;

        /// <summary>Segundos que faltam na onda atual (zero depois que o tempo acaba).</summary>
        public float TempoRestante { get; private set; }

        /// <summary>Fase atual da partida.</summary>
        public EstadoDaPartida Estado { get; private set; }

        /// <summary>Indica se o jogador está jogando uma onda (cronômetro correndo).</summary>
        public bool EmOnda => Estado == EstadoDaPartida.EmOnda;

        /// <summary>Indica se é a vez de um chefe (da onda ou da arena).</summary>
        public bool EmChefe => Estado == EstadoDaPartida.ChefeDaOnda || Estado == EstadoDaPartida.ChefeDaArena;

        /// <summary>Indica se a partida acabou (vitória ou derrota).</summary>
        public bool Encerrada => Estado == EstadoDaPartida.Vitoria || Estado == EstadoDaPartida.Derrota;

        /// <summary>Avança o cronômetro. Só tem efeito durante uma onda.</summary>
        /// <param name="deltaTempo">Segundos desde o último quadro (zero ou mais).</param>
        /// <returns><c>true</c> se o tempo acabou neste passo (a fase mudou para um chefe).</returns>
        /// <exception cref="ArgumentException">Se o tempo for negativo ou não finito.</exception>
        public bool Atualizar(float deltaTempo)
        {
            if (float.IsNaN(deltaTempo) || float.IsInfinity(deltaTempo) || deltaTempo < 0f)
            {
                throw new ArgumentException("O tempo do quadro precisa ser finito e não negativo.", nameof(deltaTempo));
            }

            if (!EmOnda)
            {
                return false;
            }

            TempoRestante = Math.Max(0f, TempoRestante - deltaTempo);
            if (TempoRestante > 0f)
            {
                return false;
            }

            Estado = Onda < _parametros.QuantidadeDeOndas ? EstadoDaPartida.ChefeDaOnda : EstadoDaPartida.ChefeDaArena;
            return true;
        }

        /// <summary>
        /// Encerra a onda atual agora (ex.: no tutorial, quando o jogador coletou tudo): zera o tempo e passa para o
        /// chefe, como se o tempo tivesse acabado. Sem efeito fora de uma onda.
        /// </summary>
        /// <returns><c>true</c> se a onda foi encerrada.</returns>
        public bool EncerrarOnda()
        {
            if (!EmOnda)
            {
                return false;
            }

            TempoRestante = 0f;
            Estado = Onda < _parametros.QuantidadeDeOndas ? EstadoDaPartida.ChefeDaOnda : EstadoDaPartida.ChefeDaArena;
            return true;
        }

        /// <summary>
        /// Registra que o chefe atual foi derrotado: começa a próxima onda, ou, depois do chefe da arena, vem a vitória.
        /// </summary>
        /// <returns><c>true</c> se começou uma nova onda; <c>false</c> se foi a vitória ou se não havia chefe.</returns>
        public bool DerrotarChefe()
        {
            switch (Estado)
            {
                case EstadoDaPartida.ChefeDaOnda:
                    Onda++;
                    TempoRestante = _parametros.DuracaoDaOnda;
                    Estado = EstadoDaPartida.EmOnda;
                    return true;
                case EstadoDaPartida.ChefeDaArena:
                    Estado = EstadoDaPartida.Vitoria;
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>Registra que o jogador foi derrotado (ex.: vida zerada). Sem efeito se a partida já acabou.</summary>
        /// <returns><c>true</c> se a partida passou para a derrota.</returns>
        public bool DerrotarJogador()
        {
            if (Encerrada)
            {
                return false;
            }

            Estado = EstadoDaPartida.Derrota;
            return true;
        }

        /// <summary>Volta para a onda 1, com o tempo cheio.</summary>
        public void Reiniciar()
        {
            Onda = 1;
            TempoRestante = _parametros.DuracaoDaOnda;
            Estado = EstadoDaPartida.EmOnda;
        }

        /// <summary>
        /// Formata segundos como "m:ss" para um cronômetro regressivo: arredonda para cima, então mostra
        /// "2:00" no início de uma onda de 120 s e só mostra "0:00" quando o tempo acabou.
        /// </summary>
        public static string FormatarTempo(float segundos)
        {
            var inteiros = SegundosExibidos(segundos);
            return $"{inteiros / 60}:{inteiros % 60:00}";
        }

        /// <summary>
        /// Segundos inteiros que o cronômetro mostra (arredondados para cima, de 0 a 359999).
        /// Só muda uma vez por segundo, então serve para saber quando refazer o texto.
        /// </summary>
        public static int SegundosExibidos(float segundos) =>
            float.IsNaN(segundos) ? 0 : (int)Math.Ceiling(Math.Max(0f, Math.Min(segundos, 359999f)));
    }
}
