using ColetorDeCristais.Atributos;
using UnityEngine;

namespace ColetorDeCristais.Corpo
{
    /// <summary>
    /// Dano no corpo, em C# puro: uma batida encolhe o tamanho, dá um tranco e deixa o personagem invulnerável por
    /// um tempo; abaixo do tamanho mínimo, é derrota.
    /// </summary>
    public sealed class DanoNoCorpo
    {
        /// <summary>Tranco em andamento (para onde o personagem é jogado).</summary>
        public Tranco Tranco { get; } = new Tranco();

        /// <summary>Tempo sem receber outro dano.</summary>
        public Invulnerabilidade Invulnerabilidade { get; } = new Invulnerabilidade();

        /// <summary>
        /// Recebe uma batida. Invulnerável, não acontece nada. Senão, soma um modificador permanente de −dano no
        /// tamanho, começa o tranco na direção dada e a invulnerabilidade.
        /// </summary>
        /// <param name="tamanho">Tamanho do personagem (o atributo do <see cref="CorpoDoJogador"/>).</param>
        /// <param name="direcao">Para onde o personagem é jogado, no chão (x = X, y = Z do mundo).</param>
        /// <param name="parametros">Valores do dano.</param>
        /// <exception cref="System.ArgumentNullException">Se o tamanho for nulo.</exception>
        public ResultadoDaBatida ReceberBatida(Atributo tamanho, Vector2 direcao, ParametrosDeDano parametros)
        {
            if (tamanho == null)
            {
                throw new System.ArgumentNullException(nameof(tamanho));
            }

            if (Invulnerabilidade.Ativa)
            {
                return ResultadoDaBatida.Ignorada;
            }

            if (parametros.Dano > 0f)
            {
                tamanho.AdicionarModificador(new Modificador(-parametros.Dano, TipoModificador.Plano, origem: this));
            }

            Tranco.Iniciar(direcao, parametros.DistanciaDoTranco, parametros.DuracaoDoTranco);
            Invulnerabilidade.Iniciar(parametros.TempoDeInvulnerabilidade);
            return RegraDeTamanho.Derrotado(tamanho.ValorFinal, parametros.TamanhoMinimo)
                ? ResultadoDaBatida.Derrota
                : ResultadoDaBatida.Dano;
        }

        /// <summary>Faz o tempo passar e devolve o deslocamento do tranco neste quadro.</summary>
        public Vector2 Atualizar(float deltaTempo)
        {
            Invulnerabilidade.Atualizar(deltaTempo);
            return Tranco.Atualizar(deltaTempo);
        }

        /// <summary>Para o tranco e encerra a invulnerabilidade (ex.: fora da onda).</summary>
        public void Interromper()
        {
            Tranco.Cancelar();
            Invulnerabilidade.Encerrar();
        }
    }
}
