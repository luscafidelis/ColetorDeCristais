using System;
using ColetorDeCristais.Cristais;
using UnityEngine;

namespace ColetorDeCristais.Habilidades
{
    /// <summary>
    /// Habilidade passiva "ímã", em C# puro: se está liberada, quem está no raio e para onde
    /// cada item vai a cada quadro.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Começa bloqueada; <see cref="Liberar"/> libera para sempre (ex.: ao coletar o item).</item>
    /// <item>Distâncias são horizontais (no chão): a altura do item e do jogador é ignorada, e o item mantém a própria altura.</item>
    /// <item>A velocidade cresce em linha reta da borda até o jogador (ver <see cref="ParametrosDeIma"/>).
    /// <see cref="DistanciaApos"/> resolve esse movimento de forma exata, então o caminho do item
    /// não depende da taxa de quadros (com o jogador parado).</item>
    /// </list>
    /// </remarks>
    public class Ima
    {
        /// <summary>Indica se a habilidade já foi liberada.</summary>
        public bool Liberada { get; private set; }

        /// <summary>Libera a habilidade. Chamar de novo não muda nada.</summary>
        public void Liberar() => Liberada = true;

        /// <summary>
        /// Decide se o ímã puxa um item neste quadro e calcula a nova posição dele.
        /// Não puxa se o ímã não estiver liberado, se o item já foi coletado, se não tiver nível de atração
        /// (sem definição), se o nível for <see cref="AtracaoDoIma.Nenhuma"/> ou se estiver fora do raio do nível.
        /// O caminho livre até o jogador (paredes) é verificado por quem chama, com a física.
        /// </summary>
        /// <param name="coletado">Se o item já foi coletado.</param>
        /// <param name="atracao">Nível de atração do item; <c>null</c> se ele não tiver definição.</param>
        /// <param name="completa">Valores do nível completo.</param>
        /// <param name="reduzida">Valores do nível reduzido.</param>
        /// <param name="item">Posição do item.</param>
        /// <param name="jogador">Posição do jogador.</param>
        /// <param name="deltaTempo">Segundos desde o último quadro (zero ou mais).</param>
        /// <param name="novaPosicao">Nova posição do item; a mesma posição se o retorno for <c>false</c>.</param>
        /// <returns><c>true</c> se o item deve ser puxado.</returns>
        /// <exception cref="ArgumentException">Se o tempo for negativo ou não finito, ou se uma posição não for finita.</exception>
        public bool TentarCalcularPuxao(
            bool coletado,
            AtracaoDoIma? atracao,
            ParametrosDeIma completa,
            ParametrosDeIma reduzida,
            Vector3 item,
            Vector3 jogador,
            float deltaTempo,
            out Vector3 novaPosicao)
        {
            novaPosicao = item;
            if (!Liberada || coletado || !atracao.HasValue
                || !TentarObterParametros(atracao.Value, completa, reduzida, out var parametros)
                || !EstaNoAlcance(item, jogador, parametros))
            {
                return false;
            }

            novaPosicao = CalcularNovaPosicao(item, jogador, parametros, deltaTempo);
            return true;
        }

        /// <summary>Escolhe os valores do nível de atração.</summary>
        /// <returns><c>false</c> para <see cref="AtracaoDoIma.Nenhuma"/> (ou um nível desconhecido): o ímã não puxa.</returns>
        public static bool TentarObterParametros(AtracaoDoIma atracao, ParametrosDeIma completa, ParametrosDeIma reduzida, out ParametrosDeIma parametros)
        {
            switch (atracao)
            {
                case AtracaoDoIma.Completa:
                    parametros = completa;
                    return true;
                case AtracaoDoIma.Reduzida:
                    parametros = reduzida;
                    return true;
                default:
                    parametros = default;
                    return false;
            }
        }

        /// <summary>Distância horizontal (no chão) entre dois pontos.</summary>
        public static float DistanciaHorizontal(Vector3 a, Vector3 b)
        {
            var dx = a.x - b.x;
            var dz = a.z - b.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        /// <summary>Indica se o item está dentro do raio (distância horizontal menor ou igual ao raio).</summary>
        public static bool EstaNoAlcance(Vector3 item, Vector3 jogador, ParametrosDeIma parametros) =>
            DistanciaHorizontal(item, jogador) <= parametros.Raio;

        /// <summary>Velocidade de um item a esta distância do jogador, em m/s (zero fora do raio).</summary>
        public static float Velocidade(float distancia, ParametrosDeIma parametros)
        {
            if (distancia > parametros.Raio)
            {
                return 0f;
            }

            var proximidade = 1f - Mathf.Max(0f, distancia) / parametros.Raio;
            return parametros.VelocidadeMaxima * (parametros.FracaoNaBorda + (1f - parametros.FracaoNaBorda) * proximidade);
        }

        /// <summary>
        /// Distância até o jogador depois de <paramref name="deltaTempo"/> segundos de atração, partindo de
        /// <paramref name="distancia"/> (dentro do raio). Nunca fica negativa.
        /// </summary>
        /// <remarks>
        /// Com <c>v(d) = vMax × (1 - (1 - f) × d / R)</c>, a distância segue <c>d' = -vMax + k × d</c>,
        /// com <c>k = vMax × (1 - f) / R</c>. A solução é <c>d(t) = c - (c - d0) × e^(k t)</c>, com <c>c = R / (1 - f)</c>.
        /// Com <c>f = 1</c> (velocidade constante), <c>d(t) = d0 - vMax × t</c>.
        /// </remarks>
        /// <exception cref="ArgumentException">Se a distância ou o tempo não forem finitos, ou forem negativos.</exception>
        public static float DistanciaApos(float distancia, float deltaTempo, ParametrosDeIma parametros)
        {
            ValidarNaoNegativo(distancia, nameof(distancia));
            ValidarNaoNegativo(deltaTempo, nameof(deltaTempo));

            var d0 = Mathf.Min(distancia, parametros.Raio);
            var k = parametros.VelocidadeMaxima * (1f - parametros.FracaoNaBorda) / parametros.Raio;
            if (k < 1e-6f)
            {
                return Mathf.Max(0f, d0 - parametros.VelocidadeMaxima * deltaTempo);
            }

            var c = parametros.Raio / (1f - parametros.FracaoNaBorda);
            var resultado = c - (c - d0) * Mathf.Exp(k * deltaTempo);
            return float.IsNaN(resultado) ? 0f : Mathf.Max(0f, resultado);
        }

        /// <summary>
        /// Posição do item depois de um quadro de atração. Fora do raio, o item fica onde está.
        /// O item anda em linha reta, no chão, na direção do jogador, sem passar dele.
        /// </summary>
        /// <exception cref="ArgumentException">Se o tempo for negativo ou não finito, ou se uma posição não for finita.</exception>
        public static Vector3 CalcularNovaPosicao(Vector3 item, Vector3 jogador, ParametrosDeIma parametros, float deltaTempo)
        {
            ValidarFinito(item, nameof(item));
            ValidarFinito(jogador, nameof(jogador));
            ValidarNaoNegativo(deltaTempo, nameof(deltaTempo));

            var distancia = DistanciaHorizontal(item, jogador);
            if (distancia > parametros.Raio || distancia < 1e-6f)
            {
                return item;
            }

            var novaDistancia = DistanciaApos(distancia, deltaTempo, parametros);
            var direcao = new Vector3(item.x - jogador.x, 0f, item.z - jogador.z) / distancia;
            var horizontal = new Vector3(jogador.x, 0f, jogador.z) + direcao * novaDistancia;
            return new Vector3(horizontal.x, item.y, horizontal.z);
        }

        private static void ValidarNaoNegativo(float valor, string nome)
        {
            if (float.IsNaN(valor) || float.IsInfinity(valor) || valor < 0f)
            {
                throw new ArgumentException("O valor precisa ser finito e não negativo.", nome);
            }
        }

        private static void ValidarFinito(Vector3 valor, string nome)
        {
            if (!Finito(valor.x) || !Finito(valor.y) || !Finito(valor.z))
            {
                throw new ArgumentException("A posição precisa ser finita.", nome);
            }
        }

        private static bool Finito(float valor) => !float.IsNaN(valor) && !float.IsInfinity(valor);
    }
}
