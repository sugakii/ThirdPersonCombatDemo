Shader "Game/VFX/FireSoftParticle"
{
    Properties
    {
        _BaseColor ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };
            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color * _BaseColor;
                return output;
            }
            half4 Frag(Varyings input) : SV_Target
            {
                // A tapered flame silhouette; only alpha fades at its edges.
                float y = input.uv.y;
                float width = lerp(0.43, 0.12, smoothstep(0.3, 1.0, y));
                float bend = 0.055 * sin(y * 7.0) * y;
                float2 p = float2((input.uv.x - 0.5 - bend) / width, (y - 0.46) / 0.52);
                float mask = 1.0 - smoothstep(0.45, 1.0, length(p));
                return half4(input.color.rgb, input.color.a * mask);
            }
            ENDHLSL
        }
    }
}
