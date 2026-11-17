using System;

namespace ColetorDeCristais.Atributos
{
    /// <summary>
    /// Alteração aplicada sobre o valor base de um <see cref="Atributo"/>.
    /// Pode ser permanente (sem <see cref="Duracao"/>) ou temporária (com duração em segundos).
    /// É imutável: o tempo restante de um modificador temporário é controlado pelo <see cref="Atributo"/>.
    /// A remoção é feita pela própria instância (igualdade por referência) ou pela <see cref="Origem"/>.
    /// </summary>
    public sealed class Modificador
    {
        /// <summary>
        /// Valor do modificador. Nos tipos percentuais é uma fração (<c>0.25</c> = 25%).
        /// Pode ser negativo para reduzir o atributo.
        /// </summary>
        public float Valor { get; }

        /// <summary>Forma de aplicação do modificador.</summary>
        public TipoModificador Tipo { get; }

        /// <summary>
        /// Quem criou o modificador (ex.: o item que o concedeu). Opcional.
        /// Permite remover de uma vez todos os modificadores de uma mesma origem.
        /// </summary>
        public object Origem { get; }

        /// <summary>
        /// Duração em segundos de um modificador temporário, contada a partir de quando ele é
        /// adicionado ao atributo. <c>null</c> indica um modificador permanente.
        /// </summary>
        public float? Duracao { get; }

        /// <summary>
        /// Grupo de renovação de um modificador temporário (ex.: o tipo de item que o concedeu).
        /// Ao adicionar um temporário de um grupo que já está ativo no atributo, todos os
        /// temporários desse grupo voltam a ter a duração completa do novo modificador,
        /// e os efeitos se somam. <c>null</c> indica um temporário independente.
        /// </summary>
        public string Grupo { get; }

        /// <summary>Indica se o modificador expira com o tempo.</summary>
        public bool EhTemporario => Duracao.HasValue;

        /// <summary>Cria um modificador.</summary>
        /// <param name="valor">Valor do modificador; não pode ser NaN nem infinito.</param>
        /// <param name="tipo">Forma de aplicação.</param>
        /// <param name="origem">Quem criou o modificador, ou <c>null</c>.</param>
        /// <param name="duracao">Duração em segundos (maior que zero e finita), ou <c>null</c> para permanente.</param>
        /// <param name="grupo">Grupo de renovação, só para temporários; <c>null</c> ou vazio para nenhum.</param>
        /// <exception cref="ArgumentException">
        /// Se o valor não for finito, se a duração não for finita e positiva, ou se houver grupo sem duração.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Se o tipo não for um valor conhecido.</exception>
        public Modificador(float valor, TipoModificador tipo, object origem = null, float? duracao = null, string grupo = null)
        {
            if (float.IsNaN(valor) || float.IsInfinity(valor))
            {
                throw new ArgumentException("O valor do modificador precisa ser um número finito.", nameof(valor));
            }

            if (!Enum.IsDefined(typeof(TipoModificador), tipo))
            {
                throw new ArgumentOutOfRangeException(nameof(tipo), tipo, "Tipo de modificador desconhecido.");
            }

            if (duracao.HasValue && (float.IsNaN(duracao.Value) || float.IsInfinity(duracao.Value) || duracao.Value <= 0f))
            {
                throw new ArgumentException("A duração precisa ser um número finito maior que zero.", nameof(duracao));
            }

            var grupoNormalizado = string.IsNullOrEmpty(grupo) ? null : grupo;
            if (grupoNormalizado != null && !duracao.HasValue)
            {
                throw new ArgumentException("Só modificadores temporários (com duração) podem ter grupo.", nameof(grupo));
            }

            Valor = valor;
            Tipo = tipo;
            Origem = origem;
            Duracao = duracao;
            Grupo = grupoNormalizado;
        }

        /// <summary>Atalho para um modificador <see cref="TipoModificador.Plano"/>.</summary>
        public static Modificador Plano(float valor, object origem = null, float? duracao = null, string grupo = null)
        {
            return new Modificador(valor, TipoModificador.Plano, origem, duracao, grupo);
        }

        /// <summary>Atalho para um modificador <see cref="TipoModificador.PercentualAditivo"/> (fração).</summary>
        public static Modificador PercentualAditivo(float fracao, object origem = null, float? duracao = null, string grupo = null)
        {
            return new Modificador(fracao, TipoModificador.PercentualAditivo, origem, duracao, grupo);
        }

        /// <summary>Atalho para um modificador <see cref="TipoModificador.PercentualMultiplicativo"/> (fração).</summary>
        public static Modificador PercentualMultiplicativo(float fracao, object origem = null, float? duracao = null, string grupo = null)
        {
            return new Modificador(fracao, TipoModificador.PercentualMultiplicativo, origem, duracao, grupo);
        }
    }
}
