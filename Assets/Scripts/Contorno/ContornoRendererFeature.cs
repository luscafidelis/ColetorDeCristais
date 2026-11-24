using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ColetorDeCristais.Contorno
{
    /// <summary>
    /// Renderer Feature do URP que desenha o contorno dos objetos com <see cref="ContornoDoObjeto"/>. Fica nos
    /// renderers do projeto (<c>Assets/Settings/PC_Renderer</c> e <c>Mobile_Renderer</c>).
    /// </summary>
    /// <remarks>
    /// O trabalho fica no <see cref="PassoDoContorno"/>: uma máscara com a silhueta dos objetos, desenhada sem teste de
    /// profundidade (por isso o contorno aparece atrás das prateleiras), e uma passada na tela inteira que pinta a cor
    /// do contorno em volta da máscara.
    /// </remarks>
    public class ContornoRendererFeature : ScriptableRendererFeature
    {
        [Tooltip("Shader do contorno (Hidden/ColetorDeCristais/Contorno).")]
        [SerializeField] private Shader shader;

        [Tooltip("Cor do contorno.")]
        [SerializeField] private Color cor = Color.black;

        [Tooltip("Espessura do contorno, em pixels numa tela de 1080 de altura (acompanha a resolução).")]
        [SerializeField] private float espessura = 2f;

        [Tooltip("Momento do desenho. Antes dos transparentes, o texto da tela (Canvas na câmera) fica por cima.")]
        [SerializeField] private RenderPassEvent momento = RenderPassEvent.BeforeRenderingTransparents;

        // Avisa só uma vez da falta do shader: o Create roda a cada mudança no Inspector e a cada recarga do renderer.
        private static bool _avisouSemShader;

        private Material _material;
        private PassoDoContorno _passo;

        /// <inheritdoc />
        public override void Create()
        {
            CoreUtils.Destroy(_material);
            _material = null;
            _passo = null;
            if (shader == null)
            {
                shader = Shader.Find(PassoDoContorno.NomeDoShader);
            }

            if (shader == null || !shader.isSupported)
            {
                if (!_avisouSemShader)
                {
                    _avisouSemShader = true;
                    Debug.LogWarning($"{name}: shader do contorno ausente ou sem suporte; o contorno não aparece.");
                }

                return;
            }

            _avisouSemShader = false;
            _material = CoreUtils.CreateEngineMaterial(shader);
            _passo = new PassoDoContorno(_material) { renderPassEvent = momento };
        }

        /// <inheritdoc />
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_material == null || _passo == null || ContornoDoObjeto.Ativos.Count == 0)
            {
                return;
            }

            _passo.renderPassEvent = momento;
            _passo.Configurar(cor, espessura);
            renderer.EnqueuePass(_passo);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_material);
            _material = null;
        }
    }
}
