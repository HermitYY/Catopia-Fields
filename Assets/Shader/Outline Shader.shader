Shader "UI/OutlineClean"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _Thickness ("Thickness (px)", Float) = 1
        _AlphaThreshold ("Alpha Threshold", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _Color;

            fixed4 _OutlineColor;
            float _Thickness;
            float _AlphaThreshold;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // 如果当前像素本身就不透明 → 原样绘制
                if (col.a > _AlphaThreshold)
                    return col;

                // 对透明像素 → 检查四周是否有非透明像素
                float2 offset = _MainTex_TexelSize.xy * _Thickness;

                float a1 = tex2D(_MainTex, i.uv + float2(offset.x, 0)).a;
                float a2 = tex2D(_MainTex, i.uv + float2(-offset.x, 0)).a;
                float a3 = tex2D(_MainTex, i.uv + float2(0, offset.y)).a;
                float a4 = tex2D(_MainTex, i.uv + float2(0, -offset.y)).a;

                float hasEdge = step(_AlphaThreshold, max(max(a1, a2), max(a3, a4)));

                // 如果透明像素附近有非透明像素 → 画描边
                if (hasEdge > 0.5)
                    return _OutlineColor;

                // 否则透明
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
