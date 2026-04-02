Shader "Custom/Shader1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)


        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0.001, 0.03)) = 0.01
        _OutlineShadowDarken ("Outline Shadow Darken", Range(0,1)) = 0.5

        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.45
        _BackLightStrength ("Back Light Strength", Range(0,1)) = 0.35

        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularThreshold ("Specular Threshold", Range(0,1)) = 0.75
        _SpecularIntensity ("Specular Intensity", Range(0,2)) = 1.0

        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(1,8)) = 3.0
        _RimIntensity ("Rim Intensity", Range(0,2)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        // -------------------------
        // PASS 1 — OUTLINE
        // -------------------------
        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
            };

            float _OutlineThickness;
            float4 _OutlineColor;
            float _OutlineShadowDarken;

          Varyings vert(Attributes IN)
{
           Varyings OUT;

            float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
            float3 normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));

            // Convert to clip space
            float4 posCS = TransformWorldToHClip(posWS);

            // Expand in screen space instead of world space
            float2 screenDir = normalize(posCS.xy);
            posCS.xy += screenDir * _OutlineThickness * posCS.w;

            OUT.positionCS = posCS;
            OUT.normalWS = normalWS;

            return OUT;
}


            half4 frag(Varyings IN) : SV_Target
            {
                Light mainLight = GetMainLight();
                float NdotL = saturate(dot(normalize(IN.normalWS), normalize(mainLight.direction)));

                float shadowFactor = lerp(1.0 - _OutlineShadowDarken, 1.0, NdotL);

                return float4(_OutlineColor.rgb * shadowFactor, _OutlineColor.a);
            }

            ENDHLSL
        }

        // -------------------------
        // PASS 2 — LIGHTING
        // -------------------------
        Pass
        {
            Name "ToonShading"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 viewDirWS  : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            float4 _Color;

            float _ShadowThreshold;
            float _BackLightStrength;

            float4 _SpecularColor;
            float _SpecularThreshold;
            float _SpecularIntensity;

            float4 _RimColor;
            float _RimPower;
            float _RimIntensity;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);

                OUT.positionCS = TransformWorldToHClip(posWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                OUT.viewDirWS = normalize(GetCameraPositionWS() - posWS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;

                Light mainLight = GetMainLight();
                float3 L = normalize(mainLight.direction);
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(IN.viewDirWS);

                // -------------------------
                // 1. Diffuse
                // -------------------------
                float NdotL = dot(N, L);

                // Back light (prevents black backside)
                float backLight = saturate(-NdotL) * _BackLightStrength;

                // Hard shadow step
                float shadowStep = step(_ShadowThreshold, saturate(NdotL));

                float3 diffuse = texColor.rgb * (shadowStep + backLight);

                // -------------------------
                // 2. STYLIZED SPECULAR
                // -------------------------
                float3 H = normalize(L + V);
                float NdotH = saturate(dot(N, H));
                float specRaw = pow(NdotH, 32.0);
                float specStep = step(_SpecularThreshold, specRaw);
                float3 specular = _SpecularColor.rgb * specStep * _SpecularIntensity;

                // -------------------------
                // 3. RIM LIGHT (ink-like)
                // -------------------------
                float rim = 1.0 - saturate(dot(N, V));
                rim = pow(rim, _RimPower);
                float3 rimLight = _RimColor.rgb * rim * _RimIntensity;

                float3 finalColor = diffuse + specular + rimLight;

                return float4(finalColor, texColor.a);
            }

            ENDHLSL
        }
    }
}
