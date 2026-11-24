using System;
using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Trajetória de um item que cai (de uma prateleira, do personagem), em C# puro: anda em linha reta no chão e,
    /// na vertical, faz um arco que sobe um pouco e desce até a altura final.
    /// </summary>
    /// <remarks>
    /// Com o progresso <c>u = tempo / duração</c> (de 0 a 1), a posição horizontal é a interpolação linear entre o início
    /// e o fim, e a altura é a interpolação linear mais <c>4 × alturaDoArco × u × (1 − u)</c> (zero nas pontas, máximo
    /// no meio).
    /// </remarks>
    public static class TrajetoriaDeQueda
    {
        /// <summary>Posição do item num instante da queda (presa ao fim depois da duração).</summary>
        /// <param name="inicio">De onde o item sai.</param>
        /// <param name="fim">Onde o item termina.</param>
        /// <param name="alturaDoArco">Quanto o arco sobe acima da linha reta no meio da queda (zero ou mais).</param>
        /// <param name="duracao">Duração da queda, em segundos (maior que zero).</param>
        /// <param name="tempo">Segundos desde o início da queda (negativo conta como zero).</param>
        /// <exception cref="ArgumentException">Se algum valor não for finito, o arco for negativo ou a duração não for positiva.</exception>
        public static Vector3 Posicao(Vector3 inicio, Vector3 fim, float alturaDoArco, float duracao, float tempo)
        {
            if (!Finito(alturaDoArco) || alturaDoArco < 0f)
            {
                throw new ArgumentException("A altura do arco precisa ser finita e não negativa.", nameof(alturaDoArco));
            }

            if (!Finito(duracao) || duracao <= 0f)
            {
                throw new ArgumentException("A duração precisa ser finita e maior que zero.", nameof(duracao));
            }

            if (!Finito(tempo))
            {
                throw new ArgumentException("O tempo precisa ser finito.", nameof(tempo));
            }

            var u = Mathf.Clamp01(tempo / duracao);
            var posicao = Vector3.Lerp(inicio, fim, u);
            posicao.y += 4f * alturaDoArco * u * (1f - u);
            return posicao;
        }

        /// <summary>Indica se a queda já terminou.</summary>
        public static bool Terminou(float duracao, float tempo) => tempo >= duracao;

        private static bool Finito(float valor) => !float.IsNaN(valor) && !float.IsInfinity(valor);
    }
}
