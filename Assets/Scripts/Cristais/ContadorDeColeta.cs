using System;
using System.Collections.Generic;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Conta os cristais coletados, no total e por tipo (identificador da definição). C# puro.
    /// </summary>
    public sealed class ContadorDeColeta
    {
        private readonly Dictionary<string, int> _porIdentificador = new Dictionary<string, int>(StringComparer.Ordinal);

        /// <summary>Total de cristais coletados, de todos os tipos.</summary>
        public int Total { get; private set; }

        /// <summary>Registra a coleta de um cristal.</summary>
        /// <param name="identificador">Identificador da definição do cristal; não pode ser nulo nem vazio.</param>
        /// <exception cref="ArgumentException">Se o identificador for nulo, vazio ou só espaços.</exception>
        public void Registrar(string identificador)
        {
            if (string.IsNullOrWhiteSpace(identificador))
            {
                throw new ArgumentException("O identificador do cristal não pode ser vazio.", nameof(identificador));
            }

            _porIdentificador.TryGetValue(identificador, out var quantidade);
            _porIdentificador[identificador] = quantidade + 1;
            Total++;
        }

        /// <summary>Quantos cristais de um tipo foram coletados.</summary>
        /// <param name="identificador">Identificador da definição; nulo ou desconhecido retorna 0.</param>
        public int Quantidade(string identificador)
        {
            if (identificador == null)
            {
                return 0;
            }

            return _porIdentificador.TryGetValue(identificador, out var quantidade) ? quantidade : 0;
        }

        /// <summary>Zera a contagem (ex.: ao reiniciar a partida).</summary>
        public void Zerar()
        {
            _porIdentificador.Clear();
            Total = 0;
        }
    }
}
