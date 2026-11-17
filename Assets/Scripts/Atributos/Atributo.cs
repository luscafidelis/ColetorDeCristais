using System;
using System.Collections.Generic;

namespace ColetorDeCristais.Atributos
{
    /// <summary>
    /// Atributo numérico (ex.: velocidade) com valor base e uma lista de modificadores
    /// permanentes e temporários. C# puro, sem dependência da Unity, para poder ser testado em EditMode.
    /// </summary>
    /// <remarks>
    /// Cálculo do <see cref="ValorFinal"/>, sempre nesta ordem:
    /// <list type="number">
    /// <item><c>soma = ValorBase + Σ planos</c></item>
    /// <item><c>soma × (1 + Σ percentuais aditivos)</c></item>
    /// <item><c>× Π (1 + cada percentual multiplicativo)</c></item>
    /// <item>o resultado é limitado a [<see cref="ValorMinimo"/>, <see cref="ValorMaximo"/>].</item>
    /// </list>
    /// Os limites valem só para o valor final; o valor base e os modificadores não são limitados.
    /// Sem limites informados, o valor final pode ficar negativo (ex.: percentual aditivo menor que -100%).
    /// <para>
    /// Modificadores temporários só perdem tempo quando <see cref="Atualizar"/> é chamado
    /// (ex.: uma vez por quadro, com o tempo do quadro). Ao chegar a zero, são removidos.
    /// </para>
    /// </remarks>
    public sealed class Atributo
    {
        private readonly List<Modificador> _modificadores = new List<Modificador>();
        private readonly Dictionary<Modificador, float> _tempoRestante = new Dictionary<Modificador, float>();
        private float _valorBase;
        private float _valorFinalEmCache;
        private bool _precisaRecalcular = true;

        /// <summary>Cria um atributo.</summary>
        /// <param name="valorBase">Valor sem modificadores; precisa ser finito.</param>
        /// <param name="valorMinimo">Menor valor final permitido. Padrão: sem limite.</param>
        /// <param name="valorMaximo">Maior valor final permitido. Padrão: sem limite.</param>
        /// <exception cref="ArgumentException">Se algum valor for NaN, se a base não for finita ou se o mínimo for maior que o máximo.</exception>
        public Atributo(float valorBase, float valorMinimo = float.NegativeInfinity, float valorMaximo = float.PositiveInfinity)
        {
            if (float.IsNaN(valorMinimo) || float.IsNaN(valorMaximo))
            {
                throw new ArgumentException("Os limites do atributo não podem ser NaN.");
            }

            if (valorMinimo > valorMaximo)
            {
                throw new ArgumentException("O valor mínimo não pode ser maior que o valor máximo.", nameof(valorMinimo));
            }

            ValorMinimo = valorMinimo;
            ValorMaximo = valorMaximo;
            ValorBase = valorBase;
        }

        /// <summary>Menor valor final permitido.</summary>
        public float ValorMinimo { get; }

        /// <summary>Maior valor final permitido.</summary>
        public float ValorMaximo { get; }

        /// <summary>Valor do atributo sem modificadores. Pode ser alterado a qualquer momento.</summary>
        /// <exception cref="ArgumentException">Se o valor não for finito.</exception>
        public float ValorBase
        {
            get => _valorBase;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                {
                    throw new ArgumentException("O valor base precisa ser um número finito.", nameof(value));
                }

                _valorBase = value;
                _precisaRecalcular = true;
            }
        }

        /// <summary>Modificadores ativos, na ordem em que foram adicionados.</summary>
        public IReadOnlyList<Modificador> Modificadores => _modificadores;

        /// <summary>Valor com todos os modificadores aplicados e limitado ao intervalo do atributo.</summary>
        public float ValorFinal
        {
            get
            {
                if (_precisaRecalcular)
                {
                    _valorFinalEmCache = Calcular();
                    _precisaRecalcular = false;
                }

                return _valorFinalEmCache;
            }
        }

        /// <summary>
        /// Adiciona um modificador. Se for temporário, começa com a duração completa.
        /// Se for temporário com <see cref="Modificador.Grupo"/>, os temporários do mesmo grupo
        /// que já estão no atributo também voltam à duração completa do novo modificador
        /// (os efeitos continuam somados).
        /// </summary>
        /// <exception cref="ArgumentNullException">Se o modificador for <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Se a mesma instância já estiver no atributo.</exception>
        public void AdicionarModificador(Modificador modificador)
        {
            if (modificador == null)
            {
                throw new ArgumentNullException(nameof(modificador));
            }

            if (_modificadores.Contains(modificador))
            {
                throw new InvalidOperationException("Este modificador já foi adicionado ao atributo.");
            }

            if (modificador.EhTemporario)
            {
                var duracao = modificador.Duracao.Value;
                if (modificador.Grupo != null)
                {
                    RenovarGrupo(modificador.Grupo, duracao);
                }

                _tempoRestante[modificador] = duracao;
            }

            _modificadores.Add(modificador);
            _precisaRecalcular = true;
        }

        /// <summary>Remove um modificador específico.</summary>
        /// <returns><c>true</c> se o modificador estava no atributo e foi removido.</returns>
        public bool RemoverModificador(Modificador modificador)
        {
            if (modificador == null || !_modificadores.Remove(modificador))
            {
                return false;
            }

            _tempoRestante.Remove(modificador);
            _precisaRecalcular = true;
            return true;
        }

        /// <summary>Remove todos os modificadores cuja <see cref="Modificador.Origem"/> é a origem informada.</summary>
        /// <param name="origem">Origem a remover; <c>null</c> não remove nada.</param>
        /// <returns>Quantidade de modificadores removidos.</returns>
        public int RemoverModificadoresDaOrigem(object origem)
        {
            if (origem == null)
            {
                return 0;
            }

            return RemoverOnde(modificador => Equals(modificador.Origem, origem));
        }

        /// <summary>Remove todos os modificadores, permanentes e temporários.</summary>
        public void RemoverTodosModificadores()
        {
            if (_modificadores.Count == 0)
            {
                return;
            }

            _modificadores.Clear();
            _tempoRestante.Clear();
            _precisaRecalcular = true;
        }

        /// <summary>
        /// Avança o tempo dos modificadores temporários e remove os que chegaram a zero.
        /// Modificadores permanentes não são afetados.
        /// </summary>
        /// <param name="deltaTempo">Tempo decorrido em segundos; finito e maior ou igual a zero.</param>
        /// <returns>Quantidade de modificadores que expiraram nesta chamada.</returns>
        /// <exception cref="ArgumentException">Se o tempo for negativo, NaN ou infinito.</exception>
        public int Atualizar(float deltaTempo)
        {
            if (float.IsNaN(deltaTempo) || float.IsInfinity(deltaTempo) || deltaTempo < 0f)
            {
                throw new ArgumentException("O tempo decorrido precisa ser um número finito maior ou igual a zero.", nameof(deltaTempo));
            }

            if (deltaTempo == 0f || _tempoRestante.Count == 0)
            {
                return 0;
            }

            var temporarios = new List<Modificador>(_tempoRestante.Keys);
            foreach (var modificador in temporarios)
            {
                _tempoRestante[modificador] -= deltaTempo;
            }

            return RemoverOnde(modificador => _tempoRestante.TryGetValue(modificador, out var restante) && restante <= 0f);
        }

        /// <summary>Obtém o tempo restante de um modificador temporário que está no atributo.</summary>
        /// <param name="modificador">Modificador consultado.</param>
        /// <param name="tempoRestante">Segundos restantes, ou zero se não houver.</param>
        /// <returns><c>true</c> se o modificador está no atributo e é temporário.</returns>
        public bool TentarObterTempoRestante(Modificador modificador, out float tempoRestante)
        {
            if (modificador != null && _tempoRestante.TryGetValue(modificador, out tempoRestante))
            {
                return true;
            }

            tempoRestante = 0f;
            return false;
        }

        private void RenovarGrupo(string grupo, float duracao)
        {
            foreach (var existente in _modificadores)
            {
                if (existente.Grupo == grupo && _tempoRestante.ContainsKey(existente))
                {
                    _tempoRestante[existente] = duracao;
                }
            }
        }

        private int RemoverOnde(Predicate<Modificador> condicao)
        {
            var removidos = 0;
            for (var indice = _modificadores.Count - 1; indice >= 0; indice--)
            {
                var modificador = _modificadores[indice];
                if (!condicao(modificador))
                {
                    continue;
                }

                _modificadores.RemoveAt(indice);
                _tempoRestante.Remove(modificador);
                removidos++;
            }

            if (removidos > 0)
            {
                _precisaRecalcular = true;
            }

            return removidos;
        }

        private float Calcular()
        {
            var somaPlanos = 0f;
            var somaPercentuaisAditivos = 0f;
            var produtoMultiplicativos = 1f;

            foreach (var modificador in _modificadores)
            {
                switch (modificador.Tipo)
                {
                    case TipoModificador.Plano:
                        somaPlanos += modificador.Valor;
                        break;
                    case TipoModificador.PercentualAditivo:
                        somaPercentuaisAditivos += modificador.Valor;
                        break;
                    case TipoModificador.PercentualMultiplicativo:
                        produtoMultiplicativos *= 1f + modificador.Valor;
                        break;
                }
            }

            var valor = (_valorBase + somaPlanos) * (1f + somaPercentuaisAditivos) * produtoMultiplicativos;
            return Math.Min(Math.Max(valor, ValorMinimo), ValorMaximo);
        }
    }
}
