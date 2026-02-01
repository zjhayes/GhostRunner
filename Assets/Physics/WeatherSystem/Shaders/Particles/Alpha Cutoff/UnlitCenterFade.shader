Shader "Custom/URP/Particles/UnlitCenterFade"
{
    Properties
    {
        _BaseColor ("Color", Color) = (0.7,0.7,0.7,0.4)
        _BaseMap ("Texture", 2D) = "white" {}

        _CenterStrength ("Center Strength", Range(0, 3)) = 1.5
        _CenterRadius ("Center Radius", Range(0.1, 1)) = 0.6

        _SoftParticlesFade ("Soft Particles Fade", Range(0, 5)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _SOFTPARTICLES_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
                float4 screenPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D_X_FLOAT(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float _CenterStrength;
                float _CenterRadius;
                float _SoftParticlesFade;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.color = IN.color;
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                UNITY_TRANSFER_FOG(OUT, OUT.positionHCS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Base texture + color
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                col *= _BaseColor * IN.color;

                // ---- Screen-center fade ----
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                float2 centered = screenUV - 0.5;
                float dist = length(centered);

                float centerFade = saturate(1 - smoothstep(_CenterRadius, 1.0, dist));
                centerFade = pow(centerFade, _CenterStrength);
                col.a *= centerFade;

                // ---- Soft particles ----
                #if defined(_SOFTPARTICLES_ON)
                    float sceneZ = LinearEyeDepth(
                        SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV)
                    );
                    float partZ = LinearEyeDepth(IN.screenPos.z / IN.screenPos.w);
                    float fade = saturate(_SoftParticlesFade * (sceneZ - partZ));
                    col.a *= fade;
                #endif

                UNITY_APPLY_FOG(IN.fogCoord, col);
                return col;
            }
            ENDHLSL
        }
    }
}
