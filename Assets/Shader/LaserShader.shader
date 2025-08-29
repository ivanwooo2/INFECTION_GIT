Shader "Unlit/LaserShader"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,0,0,1)
        _Speed ("Speed", Range(0, 5)) = 1
        _Intensity ("Intensity", Range(0, 5)) = 1
        _NoiseScale ("Noise Scale", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _MainColor;
            float _Speed;
            float _Intensity;
            float _NoiseScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y * _Speed;
                float noise = sin(i.uv.y * 10 + time) * _NoiseScale;
                float alpha = (sin(time * 5 + i.uv.x * 10) * 0.5 + 0.5) * _Intensity;
                alpha *= 1 - smoothstep(0.4, 0.5, abs(i.uv.x - 0.5));
                return _MainColor * (alpha + noise);
            }
            ENDCG
        }
    }
}
