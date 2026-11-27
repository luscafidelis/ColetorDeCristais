using System;

namespace ColetorDeCristais.Interface
{
    /// <summary>
    /// Estado do ícone de uma habilidade na HUD, em C# puro: bloqueada, em recarga (com quanto já encheu) ou pronta.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Sem recarga (ex.: o ímã, recarga total zero), uma habilidade obtida está sempre pronta.</item>
    /// <item>Valores inválidos (NaN, infinito, negativos) contam como zero: o ícone nunca quebra a tela.</item>
    /// </list>
    /// </remarks>
    public readonly struct IndicadorDeHabilidade
    {
        private IndicadorDeHabilidade(EstadoDaHabilidade estado, float preenchimento)
        {
            Estado = estado;
            Preenchimento = preenchimento;
        }

        /// <summary>Como o ícone aparece.</summary>
        public EstadoDaHabilidade Estado { get; }

        /// <summary>Quanto o ícone está cheio, de 0 a 1: 0 bloqueada, 1 pronta e, na recarga, a parte já recarregada.</summary>
        public float Preenchimento { get; }

        /// <summary>Calcula o estado do ícone.</summary>
        /// <param name="obtida">Se a habilidade já foi obtida (liberada).</param>
        /// <param name="recargaRestante">Segundos até ficar pronta (zero: pronta).</param>
        /// <param name="recargaTotal">Duração da recarga em segundos (zero: sem recarga).</param>
        public static IndicadorDeHabilidade Calcular(bool obtida, float recargaRestante, float recargaTotal)
        {
            if (!obtida)
            {
                return new IndicadorDeHabilidade(EstadoDaHabilidade.Bloqueada, 0f);
            }

            var restante = Valido(recargaRestante);
            var total = Valido(recargaTotal);
            if (restante <= 0f || total <= 0f)
            {
                return new IndicadorDeHabilidade(EstadoDaHabilidade.Pronta, 1f);
            }

            var preenchimento = Math.Min(1f, Math.Max(0f, 1f - restante / total));
            return new IndicadorDeHabilidade(EstadoDaHabilidade.EmRecarga, preenchimento);
        }

        private static float Valido(float valor) => float.IsNaN(valor) || float.IsInfinity(valor) || valor < 0f ? 0f : valor;
    }
}
