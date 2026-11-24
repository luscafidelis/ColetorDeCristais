using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ColetorDeCristais.Habilidades
{
    /// <summary>
    /// Anel no chão, em volta do personagem, que aparece quando a investida está pronta e dá um pulso quando ela
    /// fica pronta (regras no <see cref="PulsoDoIndicador"/>; malha do <see cref="MalhaDeAnel"/>).
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Fica num filho do <c>Jogador</c>: escala junto com o tamanho do personagem, mas a altura é a do chão
    /// (<see cref="alturaDoChao"/>, no mundo) e a rotação não segue a do personagem.</item>
    /// <item>Some com a investida bloqueada, em andamento ou em recarga, e com o <see cref="InvestidaDoJogador"/>
    /// desligado (fora das ondas).</item>
    /// <item>O <c>MeshRenderer</c> fica desligado na cena: o anel só é ligado por este componente, e o pisca-pisca do
    /// <see cref="ColetorDeCristais.Corpo.CorpoDoJogador"/> não mexe nele.</item>
    /// </list>
    /// </remarks>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class IndicadorDaInvestida : MonoBehaviour
    {
        [Tooltip("Investida que o anel mostra. Vazio: procura no pai.")]
        [SerializeField] private InvestidaDoJogador investida;

        [Tooltip("Raio do furo do anel, na escala 1 do personagem (m).")]
        [SerializeField] private float raioInterno = 1f;

        [Tooltip("Raio de fora do anel, na escala 1 do personagem (m).")]
        [SerializeField] private float raioExterno = 1.2f;

        [Tooltip("Segmentos da volta do anel.")]
        [SerializeField] private int segmentos = 48;

        [Tooltip("Altura do anel no mundo (m): um pouco acima do chão, para não sumir dentro dele.")]
        [SerializeField] private float alturaDoChao = 0.02f;

        [Tooltip("Escala no meio do pulso, quando a investida fica pronta (1 = sem pulso).")]
        [SerializeField] private float escalaDoPulso = 1.3f;

        [Tooltip("Duração do pulso (s).")]
        [SerializeField] private float duracaoDoPulso = 0.3f;

        private MeshRenderer _desenho;
        private Mesh _malha;

        /// <summary>Estado do anel (visível e escala). Nulo se os valores do Inspector forem inválidos.</summary>
        public PulsoDoIndicador Pulso { get; private set; }

        private void Awake()
        {
            _desenho = GetComponent<MeshRenderer>();
            _desenho.enabled = false;
            _desenho.shadowCastingMode = ShadowCastingMode.Off;
            _desenho.receiveShadows = false;
            if (investida == null)
            {
                investida = GetComponentInParent<InvestidaDoJogador>();
            }

            try
            {
                MalhaDeAnel.Gerar(raioInterno, raioExterno, segmentos, out var vertices, out var triangulos);
                Pulso = new PulsoDoIndicador(escalaDoPulso, duracaoDoPulso);
                _malha = new Mesh { name = "AnelDaInvestida" };
                _malha.vertices = vertices;
                _malha.triangles = triangulos;
                _malha.RecalculateNormals();
                _malha.RecalculateBounds();
                GetComponent<MeshFilter>().sharedMesh = _malha;
            }
            catch (ArgumentException erro)
            {
                Debug.LogError($"{name}: valores inválidos no indicador da investida ({erro.Message}); o anel não aparece.", this);
                enabled = false;
            }

            if (investida == null)
            {
                Debug.LogError($"{name}: o indicador não achou um InvestidaDoJogador; o anel não aparece.", this);
                enabled = false;
            }
        }

        private void OnDisable()
        {
            if (_desenho != null)
            {
                _desenho.enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (_malha != null)
            {
                Destroy(_malha);
            }
        }

        private void LateUpdate()
        {
            var pronta = investida.isActiveAndEnabled && investida.Investida.Pronta;
            Pulso.Atualizar(pronta, Time.deltaTime);
            _desenho.enabled = Pulso.Visivel;
            if (!Pulso.Visivel)
            {
                return;
            }

            var centro = transform.parent != null ? transform.parent.position : transform.position;
            transform.SetPositionAndRotation(new Vector3(centro.x, alturaDoChao, centro.z), Quaternion.identity);
            transform.localScale = Vector3.one * Pulso.Escala;
        }
    }
}
