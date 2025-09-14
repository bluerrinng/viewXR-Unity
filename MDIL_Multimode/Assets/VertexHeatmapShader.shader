Shader "Custom/VertexHeatmapShader"
{
    Properties
    {
        _ColorLow("Low Color", Color) = (0, 0, 1, 1) // 파랑
        _ColorHigh("High Color", Color) = (1, 0, 0, 1) // 빨강
        _Radius("Radius", Float) = 0.3
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        float4 _ColorLow;
        float4 _ColorHigh;
        float _Radius;

        int _Points_Length;
        float4 _Points0;
        float4 _Points1;
        float4 _Points2;
        float4 _Points3;
        float4 _Points4;
        float4 _Points5;
        float4 _Points6;
        float4 _Points7;
        float4 _Points8;
        float4 _Points9;
        // ... 필요한 만큼 더 선언 (최대 128개)

        struct Input {
            float3 worldPos;
        };

        float4 getHeatmapColor(float3 worldPos)
        {
            float heat = 0;

            [unroll]
            for (int i = 0; i < _Points_Length; i++)
            {
                float4 p;
                if (i == 0) p = _Points0;
                else if (i == 1) p = _Points1;
                else if (i == 2) p = _Points2;
                else if (i == 3) p = _Points3;
                else if (i == 4) p = _Points4;
                else if (i == 5) p = _Points5;
                else if (i == 6) p = _Points6;
                else if (i == 7) p = _Points7;
                else if (i == 8) p = _Points8;
                else if (i == 9) p = _Points9;
                // ... 계속 추가

                float dist = distance(worldPos, p.xyz);
                float weight = saturate(1.0 - dist / _Radius);
                heat += weight * p.w;
            }

            heat = saturate(heat); // [0,1]로 정규화
            return lerp(_ColorLow, _ColorHigh, heat);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 heatColor = getHeatmapColor(IN.worldPos);
            o.Albedo = heatColor.rgb;
            o.Alpha = heatColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
