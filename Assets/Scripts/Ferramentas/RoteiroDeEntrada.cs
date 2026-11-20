using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColetorDeCristais.Ferramentas
{
    /// <summary>Uma etapa de um <see cref="RoteiroDeEntrada"/>: segurar uma entrada por um tempo.</summary>
    [Serializable]
    public struct EtapaDoRoteiro
    {
        [Tooltip("Entrada simulada, como a do analógico (x = direita, y = para cima/frente).")]
        public Vector2 Entrada;

        [Tooltip("Por quanto tempo a entrada é segurada, em segundos.")]
        public float Duracao;

        public EtapaDoRoteiro(Vector2 entrada, float duracao)
        {
            Entrada = entrada;
            Duracao = duracao;
        }
    }

    /// <summary>
    /// Sequência de entradas simuladas ao longo do tempo (ex.: "parado 0,8 s, diagonal 2,6 s").
    /// Usada pelo gravador de quadros para animar o personagem sem teclado. C# puro.
    /// </summary>
    public sealed class RoteiroDeEntrada
    {
        private readonly EtapaDoRoteiro[] _etapas;

        /// <summary>Cria o roteiro.</summary>
        /// <exception cref="ArgumentNullException">Se a lista for nula.</exception>
        /// <exception cref="ArgumentException">Se alguma duração for negativa, NaN ou infinita.</exception>
        public RoteiroDeEntrada(IEnumerable<EtapaDoRoteiro> etapas)
        {
            if (etapas == null)
            {
                throw new ArgumentNullException(nameof(etapas));
            }

            _etapas = new List<EtapaDoRoteiro>(etapas).ToArray();
            foreach (var etapa in _etapas)
            {
                if (float.IsNaN(etapa.Duracao) || float.IsInfinity(etapa.Duracao) || etapa.Duracao < 0f)
                {
                    throw new ArgumentException("A duração de cada etapa precisa ser finita e maior ou igual a zero.", nameof(etapas));
                }

                DuracaoTotal += etapa.Duracao;
            }
        }

        /// <summary>Soma das durações das etapas, em segundos.</summary>
        public float DuracaoTotal { get; }

        /// <summary>Entrada no instante informado; fora do roteiro (antes de 0 ou depois do fim), zero.</summary>
        public Vector2 EntradaNoTempo(float tempo)
        {
            if (tempo < 0f)
            {
                return Vector2.zero;
            }

            foreach (var etapa in _etapas)
            {
                if (tempo < etapa.Duracao)
                {
                    return etapa.Entrada;
                }

                tempo -= etapa.Duracao;
            }

            return Vector2.zero;
        }
    }
}
