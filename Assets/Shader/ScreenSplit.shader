Shader "Custom/ScreenSplit" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _SplitPosition ("Split Position", Range(0,1)) = 0.5
        _SplitAmount ("Split Amount", Range(0,1)) = 0
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _SplitPosition;
            float _SplitAmount;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float worldY = i.uv.y;

                float splitPoint = _SplitPosition;
                float offset = _SplitAmount * 0.2;
                
                if(worldY > splitPoint) {
                    i.uv.y += offset;
                } else {
                    i.uv.y -= offset;
                }

                float edge = smoothstep(0.45, 0.55, abs(worldY - splitPoint));
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a *= 1 - edge;

                return col;
            }
            ENDCG
        }
    }
}