// Contorno dos objetos com ContornoDoObjeto (usado pelo PassoDoContorno).
// Passada 0 (Mascara): desenha a silhueta em branco, sem teste de profundidade, numa textura de um canal.
// Passada 1 (Composicao): na imagem da câmera, pinta _CorDoContorno onde há máscara por perto mas não no pixel.
Shader "Hidden/ColetorDeCristais/Contorno"
{
    Properties
    {
        _CorDoContorno ("Cor do contorno", Color) = (0, 0, 0, 1)
        _DeslocamentoDoContorno ("Deslocamento (UV)", Vector) = (0.003, 0.003, 0, 0)
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Mascara"
            ZTest Always

            HLSLPROGRAM
            #pragma vertex VertMascara
            #pragma fragment FragMascara
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Atributos
            {
                float4 posicaoOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Variacoes
            {
                float4 posicaoCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Variacoes VertMascara(Atributos entrada)
            {
                Variacoes saida;
                UNITY_SETUP_INSTANCE_ID(entrada);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(saida);
                saida.posicaoCS = TransformObjectToHClip(entrada.posicaoOS.xyz);
                return saida;
            }

            half4 FragMascara(Variacoes entrada) : SV_Target
            {
                return half4(1, 1, 1, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Composicao"
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragComposicao
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            half4 _CorDoContorno;
            float4 _DeslocamentoDoContorno;

            #define AMOSTRAS 16

            half LerMascara(float2 uv)
            {
                return SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv, 0).r;
            }

            half4 FragComposicao(Varyings entrada) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(entrada);
                float2 uv = entrada.texcoord;
                half dentro = LerMascara(uv);

                // Procura máscara num círculo em volta do pixel (no raio cheio e na metade, para não pular objetos finos).
                half perto = 0;
                [unroll]
                for (int i = 0; i < AMOSTRAS; i++)
                {
                    float angulo = i * (TWO_PI / AMOSTRAS);
                    float2 direcao = float2(cos(angulo), sin(angulo)) * _DeslocamentoDoContorno.xy;
                    perto = max(perto, LerMascara(uv + direcao));
                    perto = max(perto, LerMascara(uv + direcao * 0.5));
                }

                half alfa = saturate(perto - dentro);
                clip(alfa - 0.001);
                return half4(_CorDoContorno.rgb, _CorDoContorno.a * alfa);
            }
            ENDHLSL
        }
    }
}
