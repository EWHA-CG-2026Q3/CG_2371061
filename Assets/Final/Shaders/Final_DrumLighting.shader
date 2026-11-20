
Shader "CG/Final_DrumLighting"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.15, 0.12, 0.1, 1)
        _LightColor ("Light Color", Color) = (1, 0.8, 0.6, 1)
        _Ambient ("Ambient", Range(0, 1)) = 0.2
        _DiffuseStrength ("Diffuse Strength", Range(0, 2)) = 1
        _SpecularStrength ("Specular Strength", Range(0, 2)) = 0.5
        _Shininess ("Shininess", Range(1, 128)) = 32
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

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _LightColor;
                float _Ambient;
                float _DiffuseStrength;
                float _SpecularStrength;
                float _Shininess;
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

                // Ambient
                float3 ambient =
                    _BaseColor.rgb * _Ambient;

                // Lambert diffuse
                float NdotL = saturate(dot(N, L));

                float3 diffuse =
                    _BaseColor.rgb *
                    _LightColor.rgb *
                    NdotL *
                    _DiffuseStrength;

                // Phong specular
                float3 R = reflect(-L, N);

                float RdotV = saturate(dot(R, V));

                float specularPower =
                    pow(RdotV, _Shininess);

                float3 specular =
                    _LightColor.rgb *
                    specularPower *
                    _SpecularStrength *
                    NdotL;

                float3 finalColor =
                    ambient + diffuse + specular;

                return half4(finalColor, 1);
            }

            ENDHLSL
        }
    }
}
