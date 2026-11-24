using ColetorDeCristais.Atributos;
using UnityEngine;

namespace ColetorDeCristais.Corpo
{
    /// <summary>
    /// Corpo do jogador: guarda o tamanho (um <see cref="Atributo"/>) e o aplica na escala do personagem,
    /// mantendo os pés no chão.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>O tamanho multiplica a escala do objeto inteiro: o desenho e o <c>CharacterController</c> crescem juntos,
    /// então a colisão acompanha o que se vê. A velocidade, a investida e o ímã não mudam com o tamanho.</item>
    /// <item>Itens mudam o tamanho com modificadores no <see cref="Tamanho"/> (ex.: a carne, pelo
    /// <see cref="ColetorDeCristais.Cristais.EfeitoDeTamanho"/>).</item>
    /// <item>A altura do centro só é ajustada quando o tamanho muda, para não brigar com quem mexe na altura
    /// (ex.: a descida da queda no vão).</item>
    /// </list>
    /// </remarks>
    public class CorpoDoJogador : MonoBehaviour
    {
        [Tooltip("Valores do corpo (asset DefinicaoCorpo).")]
        [SerializeField] private DefinicaoCorpo definicao;

        private float _alturaNaEscalaUm;
        private float _tamanhoAplicado = float.NaN;

        /// <summary>
        /// Tamanho do personagem, como multiplicador da escala do modelo (mínimo 0). A base vem da
        /// <see cref="DefinicaoCorpo"/>; aplique modificadores aqui.
        /// </summary>
        public Atributo Tamanho { get; } = new Atributo(1f, valorMinimo: 0f);

        private void Awake()
        {
            if (definicao == null)
            {
                Debug.LogError($"{name}: o corpo não tem DefinicaoCorpo; o personagem fica no tamanho 1.", this);
            }
            else
            {
                Tamanho.ValorBase = definicao.TamanhoInicial;
            }

            var escala = transform.localScale.y;
            _alturaNaEscalaUm = escala > 0f ? transform.position.y / escala : transform.position.y;
            AplicarTamanho();
        }

        private void LateUpdate()
        {
            if (!Mathf.Approximately(Tamanho.ValorFinal, _tamanhoAplicado))
            {
                AplicarTamanho();
            }
        }

        private void AplicarTamanho()
        {
            var tamanho = Tamanho.ValorFinal;
            _tamanhoAplicado = tamanho;
            transform.localScale = Vector3.one * tamanho;
            var posicao = transform.position;
            posicao.y = RegraDeTamanho.AlturaDoCentro(_alturaNaEscalaUm, tamanho);
            transform.position = posicao;
            // O CharacterController precisa enxergar a escala e a posição novas antes do próximo Move.
            Physics.SyncTransforms();
        }
    }
}
