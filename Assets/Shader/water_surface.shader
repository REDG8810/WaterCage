Shader "Unlit/water_surface"
{
    Properties
    {
        _Cube("Reflection Cube", CUBE) = "" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpScale("Bump Scale", Range(0, 1)) = 0.5
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float3 worldPos : TEXCOORD1;
                    float3 worldNormal : TEXCOORD2;
                    float3 viewDir : TEXCOORD3;
                    float4 pos : SV_POSITION;
                };

                samplerCUBE _Cube;
                sampler2D _BumpMap;
                float _BumpScale;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                    o.uv = v.uv;
                    return o;
                }

                half3 CustomUnpackNormalWithScale(half4 packedNormal, float scale)
                {
                    half3 normal = UnpackNormal(packedNormal);
                    normal.xy *= scale;
                    return normalize(normal);
                }

                half4 frag(v2f i) : SV_Target
                {
                    // �@���}�b�v����@�������擾
                    half3 localNormal = CustomUnpackNormalWithScale(tex2D(_BumpMap, i.uv), _BumpScale);

                    // ���_�����Ɩ@�����甽�˕������v�Z
                    half3 reflDir = reflect(-i.viewDir, localNormal);
                    half4 reflColor = texCUBE(_Cube, reflDir);

                    // �t���l�����˗����v�Z
                    half F0 = 0.02;
                    half vdotn = dot(i.viewDir, localNormal);
                    half fresnel = F0 + (1 - F0) * pow(1 - vdotn, 5);
                    reflColor.a = fresnel;

                    return reflColor;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
