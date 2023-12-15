Shader "Unlit/UnlitOcclusion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {
            "RenderType" = "Opaque"
            "Queue" = "Transparent"
        }
        LOD 100

        ZWrite off
        ZTest GEqual
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
           float4 _Color;

           struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;

                float4 vert = v.vertex;

                o.pos = UnityObjectToClipPos(vert);

                return o;
            }

            half4 frag(v2f i) : COLOR {
                return _Color;
            }

            ENDCG
        }
    }
}
