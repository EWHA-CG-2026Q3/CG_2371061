
Shader "CG/Final_Spotlight"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.12, 0.15, 0.2, 1)
        _Ambient ("Ambient", Range(0, 1)) = 0.2
        _DiffuseStrength ("Diffuse Strength", Range(0, 2)) = 1
        _SpecularColor ("Specular Color", Color) = (0.8, 0.85, 1, 1)
        _SpecularStrength ("Specular Strength", Range(0, 2)) = 0.8
        _Shininess ("Shininess", Range(1, 128)) = 64
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _Ambient;
                float _DiffuseStrength;
                float4 _SpecularColor;
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
                float4 shadowCoord : TEXCOORD2;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs pos =
                    GetVertexPositionInputs(input.positionOS.xyz);

                VertexNormalInputs norm =
                    GetVertexNormalInputs(input.normalOS);

                output.positionCS = pos.positionCS;
                output.positionWS = pos.positionWS;
                output.normalWS = norm.normalWS;
                output.shadowCoord =
                    GetShadowCoord(pos);

                return output;
            }

            float3 CalculateLighting(
                float3 normal,
                float3 viewDir,
                float3 lightDir,
                float3 lightColor,
                float attenuation)
            {
                float NdotL = max(dot(normal, lightDir), 0);

                float3 diffuse =
                    _BaseColor.rgb *
                    lightColor *
                    NdotL *
                    _DiffuseStrength;

                float3 halfDir =
                    normalize(lightDir + viewDir);

                float spec =
                    pow(max(dot(normal, halfDir), 0), _Shininess);

                float3 specular =
                    _SpecularColor.rgb *
                    lightColor *
                    spec *
                    _SpecularStrength;

                return (diffuse + specular) * attenuation;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.normalWS);

                float3 viewDir =
                    normalize(GetWorldSpaceViewDir(input.positionWS));

                float3 color =
                    _BaseColor.rgb * _Ambient;

                // Main Directional Light
                Light mainLight =
                    GetMainLight(input.shadowCoord);

                color += CalculateLighting(
                    normal,
                    viewDir,
                    mainLight.direction,
                    mainLight.color,
                    mainLight.distanceAttenuation *
                    mainLight.shadowAttenuation
                );

                // Additional Point / Spot Lights
                #ifdef _ADDITIONAL_LIGHTS

                uint lightCount = GetAdditionalLightsCount();

                for (uint i = 0; i < lightCount; i++)
                {
                    Light light =
                        GetAdditionalLight(i, input.positionWS);

                    color += CalculateLighting(
                        normal,
                        viewDir,
                        light.direction,
                        light.color,
                        light.distanceAttenuation *
                        light.shadowAttenuation
                    );
                }

                #endif

                return half4(color, _BaseColor.a);
            }

            ENDHLSL
        }
    }
}
