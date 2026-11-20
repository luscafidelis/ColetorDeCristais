using UnityEngine;

namespace ColetorDeCristais.Cristais
{
    /// <summary>
    /// Faz o objeto girar no eixo vertical e subir e descer suavemente em volta da posição inicial.
    /// Use num filho visual (não no objeto com o collider), para o trigger ficar parado.
    /// Os cálculos ficam na <see cref="CalculadoraDeFlutuacao"/>.
    /// </summary>
    public class GiroEFlutuacao : MonoBehaviour
    {
        [Tooltip("Graus por segundo no eixo vertical.")]
        [SerializeField] private float velocidadeDeGiro = 90f;

        [Tooltip("Quanto sobe e desce, em metros.")]
        [Min(0f)]
        [SerializeField] private float amplitude = 0.15f;

        [Tooltip("Duração de uma subida e descida completa, em segundos.")]
        [Min(0.01f)]
        [SerializeField] private float periodo = 2f;

        private Vector3 _posicaoInicial;
        private Quaternion _rotacaoInicial;

        private void Awake()
        {
            _posicaoInicial = transform.localPosition;
            _rotacaoInicial = transform.localRotation;
        }

        private void Update()
        {
            var tempo = Time.time;
            var deslocamento = CalculadoraDeFlutuacao.DeslocamentoVertical(tempo, amplitude, periodo);
            var angulo = CalculadoraDeFlutuacao.AnguloDeGiro(tempo, velocidadeDeGiro);

            transform.localPosition = _posicaoInicial + Vector3.up * deslocamento;
            transform.localRotation = Quaternion.Euler(0f, angulo, 0f) * _rotacaoInicial;
        }
    }
}
