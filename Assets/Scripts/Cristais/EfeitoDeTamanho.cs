using ColetorDeCristais.Atributos;
using ColetorDeCristais.Corpo;
using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Efeito de coletável que muda o tamanho do jogador (<see cref="CorpoDoJogador.Tamanho"/>) de forma permanente:
    /// soma um modificador <see cref="TipoModificador.Plano"/> (ex.: +1/6 na carne crua).
    /// Crie pelo menu <i>Assets &gt; Create &gt; Coletor de Cristais &gt; Efeitos &gt; Tamanho</i>.
    /// </summary>
    [CreateAssetMenu(fileName = "EfeitoTamanho", menuName = "Coletor de Cristais/Efeitos/Tamanho")]
    public class EfeitoDeTamanho : EfeitoDeCristal
    {
        [Tooltip("Quanto o tamanho muda a cada coleta (1 = o tamanho original do montrengo). Negativo encolhe.")]
        [SerializeField] private float valor = 1f / 6f;

        /// <summary>Quanto o tamanho muda a cada coleta.</summary>
        public float Valor => valor;

        /// <inheritdoc />
        public override void Aplicar(ColetorDoJogador coletor)
        {
            var corpo = coletor != null ? coletor.GetComponent<CorpoDoJogador>() : null;
            if (corpo == null)
            {
                Debug.LogError($"{name}: o coletor não tem CorpoDoJogador; o tamanho não mudou.", this);
                return;
            }

            corpo.Tamanho.AdicionarModificador(new Modificador(valor, TipoModificador.Plano, origem: this));
            Debug.Log($"{name}: tamanho {corpo.Tamanho.ValorFinal:0.###}.", coletor);
        }
    }
}
