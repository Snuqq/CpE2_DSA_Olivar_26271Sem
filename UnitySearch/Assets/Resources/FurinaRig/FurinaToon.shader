Shader "Search/FurinaToon"
{
    Properties { _MainTex ("Character texture", 2D) = "white" {} }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            struct Input { float4 vertex : POSITION; float3 normal : NORMAL; float2 uv : TEXCOORD0; };
            struct Output { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; float3 normal : TEXCOORD1; };
            Output vert(Input v)
            {
                Output o; o.vertex = UnityObjectToClipPos(v.vertex); o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal); return o;
            }
            fixed4 frag(Output i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv); clip(color.a - .3);
                float light = dot(normalize(i.normal), normalize(float3(-.4, .7, -.6)));
                color.rgb *= light > .1 ? 1.0 : .82;
                return fixed4(color.rgb, 1);
            }
            ENDCG
        }
    }
}
