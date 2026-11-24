using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace ColetorDeCristais.Contorno
{
    /// <summary>
    /// Passada do contorno (Render Graph), criada pelo <see cref="ContornoRendererFeature"/>.
    /// </summary>
    /// <remarks>
    /// <list type="number">
    /// <item><b>Máscara:</b> desenha os desenhos de <see cref="ContornoDoObjeto.Ativos"/> numa textura de um canal
    /// (passada 0 do shader: branco, sem teste de profundidade).</item>
    /// <item><b>Composição:</b> na imagem da câmera, pinta a cor do contorno onde há máscara por perto mas não no
    /// próprio pixel (passada 1 do shader), com o deslocamento do <see cref="MedidaDoContorno"/>.</item>
    /// </list>
    /// As câmeras de prévia e de reflexo ficam sem contorno.
    /// </remarks>
    public class PassoDoContorno : ScriptableRenderPass
    {
        /// <summary>Nome do shader usado pelo contorno.</summary>
        public const string NomeDoShader = "Hidden/ColetorDeCristais/Contorno";

        private const int PassadaDaMascara = 0;
        private const int PassadaDaComposicao = 1;
        private static readonly int IdDaCor = Shader.PropertyToID("_CorDoContorno");
        private static readonly int IdDoDeslocamento = Shader.PropertyToID("_DeslocamentoDoContorno");

        private readonly Material _material;
        private Color _cor = Color.black;
        private float _espessura;

        private class DadosDaMascara
        {
            public Material Material;
        }

        private class DadosDaComposicao
        {
            public Material Material;
            public TextureHandle Mascara;
        }

        /// <summary>Cria a passada com o material do shader do contorno.</summary>
        public PassoDoContorno(Material material)
        {
            _material = material;
            profilingSampler = new ProfilingSampler("Contorno");
            // A composição lê a máscara e escreve na imagem da câmera, que precisa ser uma textura intermediária.
            requiresIntermediateTexture = true;
        }

        /// <summary>Define a cor e a espessura (pixels numa tela de 1080 de altura) do próximo quadro.</summary>
        public void Configurar(Color cor, float espessura)
        {
            _cor = cor;
            _espessura = espessura;
        }

        /// <inheritdoc />
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var dadosDaCamera = frameData.Get<UniversalCameraData>();
            var recursos = frameData.Get<UniversalResourceData>();
            if (_material == null || recursos.isActiveTargetBackBuffer
                || dadosDaCamera.cameraType == CameraType.Preview || dadosDaCamera.cameraType == CameraType.Reflection)
            {
                return;
            }

            var descritor = dadosDaCamera.cameraTargetDescriptor;
            var deslocamento = MedidaDoContorno.Deslocamento(_espessura, descritor.width, descritor.height);
            if (deslocamento == Vector2.zero)
            {
                return;
            }

            _material.SetColor(IdDaCor, _cor);
            _material.SetVector(IdDoDeslocamento, deslocamento);

            descritor.depthStencilFormat = GraphicsFormat.None;
            descritor.depthBufferBits = 0;
            descritor.msaaSamples = 1;
            descritor.graphicsFormat = GraphicsFormat.R8_UNorm;
            var mascara = UniversalRenderer.CreateRenderGraphTexture(renderGraph, descritor, "_MascaraDoContorno", true, FilterMode.Bilinear);

            using (var construtor = renderGraph.AddRasterRenderPass<DadosDaMascara>("Contorno: máscara", out var dados, profilingSampler))
            {
                dados.Material = _material;
                construtor.SetRenderAttachment(mascara, 0, AccessFlags.Write);
                construtor.AllowPassCulling(false);
                construtor.SetRenderFunc((DadosDaMascara d, RasterGraphContext contexto) => DesenharMascara(d, contexto.cmd));
            }

            using (var construtor = renderGraph.AddRasterRenderPass<DadosDaComposicao>("Contorno: composição", out var dados, profilingSampler))
            {
                dados.Material = _material;
                dados.Mascara = mascara;
                construtor.UseTexture(mascara, AccessFlags.Read);
                construtor.SetRenderAttachment(recursos.activeColorTexture, 0, AccessFlags.Write);
                construtor.SetRenderFunc((DadosDaComposicao d, RasterGraphContext contexto) =>
                    Blitter.BlitTexture(contexto.cmd, d.Mascara, new Vector4(1f, 1f, 0f, 0f), d.Material, PassadaDaComposicao));
            }
        }

        private static void DesenharMascara(DadosDaMascara dados, RasterCommandBuffer comandos)
        {
            foreach (var objeto in ContornoDoObjeto.Ativos)
            {
                if (objeto == null)
                {
                    continue;
                }

                foreach (var desenho in objeto.Desenhos)
                {
                    if (!ContornoDoObjeto.Desenhavel(desenho))
                    {
                        continue;
                    }

                    // Uma chamada por parte da malha (uma por material do desenho).
                    var partes = Mathf.Max(1, desenho.sharedMaterials.Length);
                    for (var parte = 0; parte < partes; parte++)
                    {
                        comandos.DrawRenderer(desenho, dados.Material, parte, PassadaDaMascara);
                    }
                }
            }
        }
    }
}
