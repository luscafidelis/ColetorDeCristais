using UnityEngine;

namespace ColetorDeCristais.Animacao
{
    /// <summary>
    /// Resultado da <see cref="CalculadoraDeDeformacao"/> num quadro: a escala e a rotação
    /// a aplicar no visual, relativas à pose original dele.
    /// </summary>
    public readonly struct PoseDeDeformacao
    {
        /// <summary>Multiplicador da escala original em cada eixo (1 = sem deformação).</summary>
        public Vector3 Escala { get; }

        /// <summary>Inclinação a aplicar antes da rotação original (identidade = de pé).</summary>
        public Quaternion Rotacao { get; }

        public PoseDeDeformacao(Vector3 escala, Quaternion rotacao)
        {
            Escala = escala;
            Rotacao = rotacao;
        }
    }
}
