Shader "UI/BoxCutoutOverlay"
{
    Properties
    {
        _Color("Overlay Color", Color) = (0, 0, 0, 0.7)
        _HoleCenter("Hole Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _HoleSize("Hole Size (UV)", Vector) = (0.2, 0.2, 0, 0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float4 _HoleCenter;
            float4 _HoleSize;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 min = _HoleCenter.xy - _HoleSize.xy * 0.5;
                float2 max = _HoleCenter.xy + _HoleSize.xy * 0.5;

                bool inHole = uv.x >= min.x && uv.x <= max.x && uv.y >= min.y && uv.y <= max.y;
                return inHole ? fixed4(0, 0, 0, 0) : _Color;
            }
            ENDCG
        }
    }
}
