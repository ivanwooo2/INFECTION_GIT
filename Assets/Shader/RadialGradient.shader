Shader "Unlit/RadialGradient"
{
    Properties
    {
        _Color ("Edge Color", Color) = (1,0,0,1)
        _Radius ("Fade Radius", Range(0,1)) = 0.7
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_instancing

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

            fixed4 _Color;
            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float distance = length(i.uv - center);
                float alpha = smoothstep(_Radius - 0.3, _Radius + 0.3, distance);
                float alpha1 = smoothstep(0.1, 0.3, distance);
                float alpha2 = smoothstep(0.4, 0.6, distance);
                float finalAlpha = saturate(alpha1 + alpha2 * 0.7);
                return fixed4(_Color.rgb, alpha * _Color.a);
            }
            ENDCG
        }
    }
}
