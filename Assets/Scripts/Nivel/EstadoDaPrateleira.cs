using System;
using UnityEngine;
using Random = System.Random;

namespace ColetorDeCristais.Nivel
{
    /// <summary>
    /// Segredo das prateleiras, em C# puro: uma investida contra a prateleira derruba um produto, uma vez só.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Só a investida derruba; encostar andando não faz nada.</item>
    /// <item>Cada prateleira derruba uma vez (<see cref="Derrubou"/>).</item>
    /// <item>O produto cai no corredor, na frente da face que o personagem bateu, para o lado
    /// (<see cref="PosicaoDoProduto"/>), para não ser coletado na hora sem o jogador ver.</item>
    /// </list>
    /// </remarks>
    public class EstadoDaPrateleira
    {
        /// <summary>Indica se a prateleira já derrubou o produto dela.</summary>
        public bool Derrubou { get; private set; }

        /// <summary>Registra uma batida. Derruba só na primeira batida durante uma investida.</summary>
        /// <param name="emInvestida">Se a batida aconteceu durante uma investida.</param>
        /// <returns><c>true</c> se o produto deve cair agora.</returns>
        public bool TentarDerrubar(bool emInvestida)
        {
            if (!emInvestida || Derrubou)
            {
                return false;
            }

            Derrubou = true;
            return true;
        }

        /// <summary>Sorteia qual produto cai.</summary>
        /// <param name="quantidadeDeOpcoes">Quantos produtos existem.</param>
        /// <param name="sorteio">Gerador de números aleatórios.</param>
        /// <returns>Índice do produto, ou -1 se não houver opções.</returns>
        public static int SortearProduto(int quantidadeDeOpcoes, Random sorteio)
        {
            if (sorteio == null)
            {
                throw new ArgumentNullException(nameof(sorteio));
            }

            return quantidadeDeOpcoes <= 0 ? -1 : sorteio.Next(quantidadeDeOpcoes);
        }

        /// <summary>
        /// Onde o produto cai: na frente da face da prateleira mais perto do personagem, afastado dela, e deslocado
        /// para o lado ao longo da face.
        /// </summary>
        /// <param name="prateleira">Base da prateleira.</param>
        /// <param name="personagem">Centro do personagem no chão (x, z).</param>
        /// <param name="afastamento">Distância da face até o produto (zero ou mais).</param>
        /// <param name="deslocamentoLateral">Deslocamento ao longo da face (o sinal escolhe o lado).</param>
        /// <exception cref="ArgumentException">Se algum valor não for finito ou o afastamento for negativo.</exception>
        public static Vector2 PosicaoDoProduto(Retangulo prateleira, Vector2 personagem, float afastamento, float deslocamentoLateral)
        {
            if (!Finito(personagem.x) || !Finito(personagem.y) || !Finito(deslocamentoLateral))
            {
                throw new ArgumentException("A posição e o deslocamento precisam ser finitos.");
            }

            if (!Finito(afastamento) || afastamento < 0f)
            {
                throw new ArgumentException("O afastamento precisa ser finito e não negativo.", nameof(afastamento));
            }

            var normal = NormalDaFace(prateleira, personagem);
            var lateral = new Vector2(-normal.y, normal.x);
            var naFace = prateleira.PontoMaisProximo(personagem);
            // Com o personagem dentro da prateleira (não deveria acontecer), parte da face pela normal escolhida.
            if (prateleira.Contem(personagem))
            {
                var meio = prateleira.Tamanho * 0.5f;
                naFace = new Vector2(
                    normal.x != 0f ? prateleira.Centro.x + normal.x * meio.x : personagem.x,
                    normal.y != 0f ? prateleira.Centro.y + normal.y * meio.y : personagem.y);
            }

            return naFace + normal * afastamento + lateral * deslocamentoLateral;
        }

        /// <summary>
        /// Normal (para fora) da face da prateleira virada para o personagem: o eixo em que ele está mais longe
        /// do centro, proporcionalmente ao tamanho.
        /// </summary>
        public static Vector2 NormalDaFace(Retangulo prateleira, Vector2 personagem)
        {
            var relativo = personagem - prateleira.Centro;
            var meio = prateleira.Tamanho * 0.5f;
            var emX = meio.x > 0f ? Mathf.Abs(relativo.x) / meio.x : float.PositiveInfinity;
            var emZ = meio.y > 0f ? Mathf.Abs(relativo.y) / meio.y : float.PositiveInfinity;
            return emX >= emZ
                ? new Vector2(relativo.x >= 0f ? 1f : -1f, 0f)
                : new Vector2(0f, relativo.y >= 0f ? 1f : -1f);
        }

        private static bool Finito(float valor) => !float.IsNaN(valor) && !float.IsInfinity(valor);
    }
}
