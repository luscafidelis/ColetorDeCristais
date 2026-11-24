using System;
using UnityEngine;

namespace ColetorDeCristais.Habilidades
{
    /// <summary>
    /// Estado da habilidade "investida" (avanço rápido), em C# puro: se está liberada,
    /// se está em andamento, a recarga e o deslocamento de cada quadro.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Começa bloqueada; <see cref="Liberar"/> libera para sempre (ex.: ao coletar o item).</item>
    /// <item><see cref="TentarIniciar"/> só funciona liberada, fora de uma investida e sem recarga.
    /// A velocidade é a de movimento no início × o multiplicador, e fica fixa até o fim.</item>
    /// <item>A recarga começa junto com a investida (ela já conta durante o avanço).</item>
    /// <item><see cref="Atualizar"/> avança o tempo e devolve o deslocamento do quadro. O último quadro
    /// usa só o tempo que faltava, então a distância total é exatamente velocidade × duração.</item>
    /// </list>
    /// Quem aplica o deslocamento (o <c>CharacterController</c>) decide as colisões: uma parede para o avanço.
    /// </remarks>
    public class Investida
    {
        private float _tempoRestante;
        private Vector3 _velocidade;

        /// <summary>Indica se a habilidade já foi liberada.</summary>
        public bool Liberada { get; private set; }

        /// <summary>Indica se uma investida está acontecendo agora.</summary>
        public bool EmAndamento => _tempoRestante > 0f;

        /// <summary>Segundos até poder usar de novo (zero quando pronta).</summary>
        public float RecargaRestante { get; private set; }

        /// <summary>Indica se <see cref="TentarIniciar"/> funcionaria agora.</summary>
        public bool Pronta => Liberada && !EmAndamento && RecargaRestante <= 0f;

        /// <summary>Velocidade da investida atual (direção × m/s); zero fora de uma investida.</summary>
        public Vector3 Velocidade => EmAndamento ? _velocidade : Vector3.zero;

        /// <summary>Libera a habilidade. Chamar de novo não muda nada.</summary>
        public void Liberar() => Liberada = true;

        /// <summary>
        /// Começa uma investida, se a habilidade estiver pronta.
        /// </summary>
        /// <param name="direcao">Direção horizontal do avanço (y é ignorado; não precisa estar normalizada).</param>
        /// <param name="velocidadeDeMovimento">Velocidade normal do personagem agora, em m/s.</param>
        /// <param name="parametros">Multiplicador, duração e recarga.</param>
        /// <returns><c>true</c> se a investida começou.</returns>
        /// <exception cref="ArgumentException">Se a direção ou a velocidade não forem finitas.</exception>
        public bool TentarIniciar(Vector3 direcao, float velocidadeDeMovimento, ParametrosDeInvestida parametros)
        {
            if (!Finito(direcao.x) || !Finito(direcao.y) || !Finito(direcao.z))
            {
                throw new ArgumentException("A direção precisa ser finita.", nameof(direcao));
            }

            if (!Finito(velocidadeDeMovimento))
            {
                throw new ArgumentException("A velocidade precisa ser finita.", nameof(velocidadeDeMovimento));
            }

            var horizontal = new Vector3(direcao.x, 0f, direcao.z);
            if (!Pronta || horizontal.sqrMagnitude < 1e-8f || velocidadeDeMovimento <= 0f)
            {
                return false;
            }

            _velocidade = horizontal.normalized * (velocidadeDeMovimento * parametros.MultiplicadorDeVelocidade);
            _tempoRestante = parametros.Duracao;
            RecargaRestante = parametros.Recarga;
            return true;
        }

        /// <summary>
        /// Interrompe a investida em andamento (ex.: o jogador foi travado no fim de uma onda).
        /// O resto do avanço é descartado; a recarga continua, porque a investida já foi usada.
        /// Sem efeito fora de uma investida.
        /// </summary>
        public void Cancelar() => _tempoRestante = 0f;

        /// <summary>Avança o tempo da investida e da recarga.</summary>
        /// <param name="deltaTempo">Segundos desde o último quadro (zero ou mais).</param>
        /// <returns>Deslocamento a aplicar neste quadro (zero fora de uma investida).</returns>
        /// <exception cref="ArgumentException">Se o tempo for negativo ou não finito.</exception>
        public Vector3 Atualizar(float deltaTempo)
        {
            if (!Finito(deltaTempo) || deltaTempo < 0f)
            {
                throw new ArgumentException("O tempo do quadro precisa ser finito e não negativo.", nameof(deltaTempo));
            }

            RecargaRestante = Mathf.Max(0f, RecargaRestante - deltaTempo);
            if (!EmAndamento)
            {
                return Vector3.zero;
            }

            var passo = Mathf.Min(deltaTempo, _tempoRestante);
            _tempoRestante -= passo;
            return _velocidade * passo;
        }

        /// <summary>
        /// Direção da investida: a da entrada de movimento, se houver; senão, para onde o personagem está virado.
        /// </summary>
        /// <param name="entrada">Entrada de movimento (x → X do mundo, y → Z), como no <c>MovimentoDoJogador</c>.</param>
        /// <param name="frente">Frente do personagem no mundo (só a parte horizontal conta).</param>
        /// <param name="entradaMinima">Abaixo deste tamanho, a entrada é ignorada.</param>
        /// <returns>Direção horizontal normalizada; +Z se nem a entrada nem a frente servirem.</returns>
        public static Vector3 CalcularDirecao(Vector2 entrada, Vector3 frente, float entradaMinima)
        {
            if (Finito(entrada.x) && Finito(entrada.y) && entrada.magnitude >= entradaMinima && entrada.sqrMagnitude > 0f)
            {
                return new Vector3(entrada.x, 0f, entrada.y).normalized;
            }

            var horizontal = new Vector3(frente.x, 0f, frente.z);
            return horizontal.sqrMagnitude > 1e-8f ? horizontal.normalized : Vector3.forward;
        }

        private static bool Finito(float valor) => !float.IsNaN(valor) && !float.IsInfinity(valor);
    }
}
