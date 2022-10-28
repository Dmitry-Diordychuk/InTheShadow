Shader "Custom/Outline"
{
    Properties
    {
        _Color ("Glow Color", Color ) = ( 1, 1, 1, 1)
        _Intensity ("Intensity", Float) = 2
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
       
        Blend [_SrcBlend] [_DstBlend]
        ZTest Always    // всегда рисуем, независимо от текущей глубины в буфере
        ZWrite Off      // и ничего в него не пишем
        Cull Off        // рисуем все стороны меша
        
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionHCS   : POSITION;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float2  uv          : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // Note: The pass is setup with a mesh already in clip
                // space, that's why, it's enough to just output vertex
                // positions
                output.positionCS = float4(input.positionHCS.xyz, 1.0);

                #if UNITY_UV_STARTS_AT_TOP
                output.positionCS.y *= -1;
                #endif

                output.uv = input.uv;
                return output;
            }

            TEXTURE2D_X(_SolidTexture);
            SAMPLER(sampler_SolidTexture);

            TEXTURE2D_X(_BlurTexture);
            SAMPLER(sampler_BlurTexture);

            half4 _Color;
            half _Intensity;

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                half4 prepassColor = SAMPLE_TEXTURE2D_X(_SolidTexture, sampler_SolidTexture, input.uv);
                half4 bluredColor = SAMPLE_TEXTURE2D_X(_BlurTexture, sampler_BlurTexture, input.uv);
                half4 difColor = max( 0, bluredColor - prepassColor);
                half4 color = difColor * _Color * _Intensity;
                color.a = 1;    
                return color;
            }
            ENDHLSL
        }
    }
}

