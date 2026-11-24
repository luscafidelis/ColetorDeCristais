using System;
using ColetorDeCristais.Atributos;
using ColetorDeCristais.Movimento;
using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Efeito de coletável que muda a velocidade do jogador (<see cref="MovimentoDoJogador.Velocidade"/>):
    /// uma parte temporária e uma parte permanente, descritas por um <see cref="BonusDeAtributo"/>.
    /// Crie pelo menu <i>Assets &gt; Create &gt; Coletor de Cristais &gt; Efeitos &gt; Velocidade</i> e coloque
    /// o asset na lista de efeitos de uma <see cref="DefinicaoCristal"/>.
    /// </summary>
    /// <remarks>Os valores são decisões de design e ficam no asset; a regra de cálculo é a do <see cref="Atributo"/>.</remarks>
    [CreateAssetMenu(fileName = "EfeitoVelocidade", menuName = "Coletor de Cristais/Efeitos/Velocidade")]
    public class EfeitoDeVelocidade : EfeitoDeCristal
    {
        [Header("Parte temporária")]
        [Tooltip("Forma de aplicação do bônus temporário.")]
        [SerializeField] private TipoModificador tipoTemporario = TipoModificador.PercentualAditivo;

        [Tooltip("Valor do bônus temporário. Nos percentuais é fração: 0,3 = +30%. Zero desliga a parte temporária.")]
        [SerializeField] private float valorTemporario = 0.3f;

        [Tooltip("Duração do bônus temporário, em segundos (maior que zero).")]
        [Min(0.01f)]
        [SerializeField] private float duracao = 5f;

        [Tooltip("Grupo de renovação: coletar de novo um item do mesmo grupo reinicia o tempo e soma os efeitos.")]
        [SerializeField] private string grupo = "velocidade";

        [Header("Parte permanente")]
        [Tooltip("Forma de aplicação do bônus permanente.")]
        [SerializeField] private TipoModificador tipoPermanente = TipoModificador.Plano;

        [Tooltip("Valor do bônus permanente (no Plano, em m/s). Zero desliga a parte permanente.")]
        [SerializeField] private float valorPermanente = 0.3f;

        [Tooltip("Quando o bônus permanente entra: junto com o temporário ou só quando o temporário acabar.")]
        [SerializeField] private MomentoDoPermanente momento = MomentoDoPermanente.AoFimDoTemporario;

        /// <summary>O bônus descrito pelos campos do asset.</summary>
        /// <exception cref="ArgumentException">Se a combinação de campos for inválida (ex.: parte temporária sem grupo).</exception>
        public BonusDeAtributo Bonus =>
            new BonusDeAtributo(tipoTemporario, valorTemporario, duracao, grupo, tipoPermanente, valorPermanente, momento);

        /// <inheritdoc />
        public override void Aplicar(ColetorDoJogador coletor)
        {
            var movimento = coletor != null ? coletor.GetComponent<MovimentoDoJogador>() : null;
            if (movimento == null || movimento.Velocidade == null)
            {
                Debug.LogError($"{name}: o coletor não tem MovimentoDoJogador pronto; o efeito de velocidade não foi aplicado.", this);
                return;
            }

            if (AplicarEm(movimento.Velocidade))
            {
                Debug.Log($"{name}: velocidade agora {movimento.Velocidade.ValorFinal:F2} m/s.", coletor);
            }
        }

        /// <summary>
        /// Aplica o bônus num atributo de velocidade, com este asset como origem dos modificadores.
        /// Com valores inválidos no asset, registra um erro no Console e não aplica nada.
        /// </summary>
        /// <param name="velocidade">Atributo que recebe o bônus.</param>
        /// <returns><c>true</c> se o bônus foi aplicado.</returns>
        /// <exception cref="ArgumentNullException">Se o atributo for <c>null</c>.</exception>
        public bool AplicarEm(Atributo velocidade)
        {
            if (velocidade == null)
            {
                throw new ArgumentNullException(nameof(velocidade));
            }

            if (!TentarMontarBonus(out var bonus, out var erro))
            {
                Debug.LogError($"{name}: valores inválidos ({erro}); o efeito de velocidade não foi aplicado.", this);
                return false;
            }

            bonus.AplicarEm(velocidade, this);
            return true;
        }

        private void OnValidate()
        {
            if (!TentarMontarBonus(out _, out var erro))
            {
                Debug.LogWarning($"{name}: valores inválidos ({erro}). Corrija no Inspector; com estes valores o efeito não é aplicado.", this);
            }
        }

        private bool TentarMontarBonus(out BonusDeAtributo bonus, out string erro)
        {
            try
            {
                bonus = Bonus;
                erro = null;
                return true;
            }
            catch (ArgumentException excecao)
            {
                bonus = default;
                erro = excecao.Message;
                return false;
            }
        }
    }
}
