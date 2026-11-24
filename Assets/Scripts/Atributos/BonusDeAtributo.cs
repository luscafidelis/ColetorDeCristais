using System;

namespace ColetorDeCristais.Atributos
{
    /// <summary>
    /// Descrição de um bônus sobre um <see cref="Atributo"/>, em C# puro: uma parte temporária
    /// (com grupo de renovação) e uma parte permanente, que pode entrar na hora ou só quando a temporária acabar.
    /// Usado pelos efeitos dos itens coletáveis (ex.: a coxa dá velocidade).
    /// </summary>
    /// <remarks>
    /// Cada <see cref="AplicarEm"/> cria modificadores novos. Aplicar de novo durante o efeito segue a regra
    /// do <see cref="Atributo"/>: o tempo do grupo reinicia e os efeitos se somam. Com
    /// <see cref="MomentoDoPermanente.AoFimDoTemporario"/>, cada aplicação agenda mais uma parte permanente
    /// para o fim do grupo, então todas entram juntas quando o efeito temporário renovado acaba.
    /// </remarks>
    public readonly struct BonusDeAtributo
    {
        /// <summary>Forma de aplicação da parte temporária.</summary>
        public TipoModificador TipoTemporario { get; }

        /// <summary>Valor da parte temporária (fração nos percentuais: 0,3 = +30%). Zero desliga a parte temporária.</summary>
        public float ValorTemporario { get; }

        /// <summary>Duração da parte temporária, em segundos.</summary>
        public float Duracao { get; }

        /// <summary>Grupo de renovação da parte temporária (ex.: o tipo de item).</summary>
        public string Grupo { get; }

        /// <summary>Forma de aplicação da parte permanente.</summary>
        public TipoModificador TipoPermanente { get; }

        /// <summary>Valor da parte permanente. Zero desliga a parte permanente.</summary>
        public float ValorPermanente { get; }

        /// <summary>Quando a parte permanente entra no atributo.</summary>
        public MomentoDoPermanente Momento { get; }

        /// <summary>Indica se o bônus tem parte temporária.</summary>
        public bool TemTemporario => ValorTemporario != 0f;

        /// <summary>Indica se o bônus tem parte permanente.</summary>
        public bool TemPermanente => ValorPermanente != 0f;

        /// <exception cref="ArgumentException">
        /// Se algum valor não for finito, se houver parte temporária sem duração positiva ou sem grupo,
        /// ou se o permanente esperar o fim de um temporário que não existe.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Se um tipo ou o momento não for um valor conhecido.</exception>
        public BonusDeAtributo(
            TipoModificador tipoTemporario,
            float valorTemporario,
            float duracao,
            string grupo,
            TipoModificador tipoPermanente,
            float valorPermanente,
            MomentoDoPermanente momento)
        {
            ValidarFinito(valorTemporario, nameof(valorTemporario));
            ValidarFinito(duracao, nameof(duracao));
            ValidarFinito(valorPermanente, nameof(valorPermanente));
            ValidarEnum(tipoTemporario, nameof(tipoTemporario));
            ValidarEnum(tipoPermanente, nameof(tipoPermanente));
            ValidarEnum(momento, nameof(momento));

            var grupoNormalizado = string.IsNullOrWhiteSpace(grupo) ? null : grupo.Trim();
            if (valorTemporario != 0f)
            {
                if (duracao <= 0f)
                {
                    throw new ArgumentException("A parte temporária precisa de duração maior que zero.", nameof(duracao));
                }

                if (grupoNormalizado == null)
                {
                    throw new ArgumentException("A parte temporária precisa de um grupo (ex.: o tipo de item).", nameof(grupo));
                }
            }
            else if (valorPermanente != 0f && momento == MomentoDoPermanente.AoFimDoTemporario)
            {
                throw new ArgumentException("O permanente só pode esperar o fim do temporário se houver parte temporária.", nameof(momento));
            }

            TipoTemporario = tipoTemporario;
            ValorTemporario = valorTemporario;
            Duracao = duracao;
            Grupo = grupoNormalizado;
            TipoPermanente = tipoPermanente;
            ValorPermanente = valorPermanente;
            Momento = momento;
        }

        /// <summary>Aplica o bônus no atributo: a parte temporária na hora e a permanente conforme o <see cref="Momento"/>.</summary>
        /// <param name="alvo">Atributo que recebe o bônus.</param>
        /// <param name="origem">Quem concedeu o bônus (vai em <see cref="Modificador.Origem"/>), ou <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Se o atributo for <c>null</c>.</exception>
        public void AplicarEm(Atributo alvo, object origem = null)
        {
            if (alvo == null)
            {
                throw new ArgumentNullException(nameof(alvo));
            }

            if (TemTemporario)
            {
                alvo.AdicionarModificador(new Modificador(ValorTemporario, TipoTemporario, origem, Duracao, Grupo));
            }

            if (!TemPermanente)
            {
                return;
            }

            var permanente = new Modificador(ValorPermanente, TipoPermanente, origem);
            if (Momento == MomentoDoPermanente.AoFimDoTemporario)
            {
                alvo.AgendarAoFimDoGrupo(Grupo, permanente);
            }
            else
            {
                alvo.AdicionarModificador(permanente);
            }
        }

        private static void ValidarFinito(float valor, string nome)
        {
            if (float.IsNaN(valor) || float.IsInfinity(valor))
            {
                throw new ArgumentException("O valor precisa ser um número finito.", nome);
            }
        }

        private static void ValidarEnum<T>(T valor, string nome) where T : Enum
        {
            if (!Enum.IsDefined(typeof(T), valor))
            {
                throw new ArgumentOutOfRangeException(nome, valor, "Valor desconhecido.");
            }
        }
    }
}
