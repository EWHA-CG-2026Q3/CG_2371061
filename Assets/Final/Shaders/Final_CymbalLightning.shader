
Shader "CG/Final_CymbalLighting"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.65, 0.55, 0.3, 1)
        _SpecularColor ("Specular Color", Color) = (1, 0.9, 0.65, 1)
        _Shininess ("Shininess", Range(1, 128)) = 64
        _Ambient ("Ambient", Range(0, 1)) = 0.15

        _ReflectionStrength ("Reflection Strength", Range(0, 1)) = 0.3
        _ReflectionSmoothness ("Reflection Smoothness", Range(0, 1)) = 0.8
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_ATLAS
            
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _SpecularColor;
                float _Shininess;
                float _Ambient;
                float _ReflectionStrength;
                float _ReflectionSmoothness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs pos =
                    GetVertexPositionInputs(input.positionOS.xyz);

                output.positionCS = pos.positionCS;
                output.positionWS = pos.positionWS;
                output.normalWS =
                    TransformObjectToWorldNormal(input.normalOS);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 N = normalize(input.normalWS);
                float3 L = normalize(GetMainLight().direction);
                float3 V = normalize(
                    GetWorldSpaceViewDir(input.positionWS)
                );

                // Lambert diffuse
                float NdotL = saturate(dot(N, L));

                float3 diffuse =
                    _BaseColor.rgb *
                    GetMainLight().color *
                    NdotL;

                // Blinn-Phong specular
                float3 H = normalize(L + V);
                float NdotH = saturate(dot(N, H));

                float specularPower =
                    pow(NdotH, _Shininess);

                float3 specular =
                    _SpecularColor.rgb *
                    GetMainLight().color *
                    specularPower *
                    NdotL;

                // Ambient
                float3 ambient =
                    _BaseColor.rgb * _Ambient;

                // Reflection Probe
                float3 reflectionVector =
                    reflect(-V, N);

                float3 reflection =
                    GlossyEnvironmentReflection(
                        reflectionVector,
                        input.positionWS,
                        1.0 - _ReflectionSmoothness,
                        1.0,
                        GetNormalizedScreenSpaceUV(input.positionCS)
                    );

                float3 finalColor =
                    ambient +
                    diffuse +
                    specular +
                    reflection * _ReflectionStrength;

                return half4(finalColor, 1);
            }

            ENDHLSL
        }
    }
}
