Shader "Custom/InstancedIndirectColor" {
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader{
        Tags { "RenderType" = "Opaque" }
    	Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            struct MeshProperties {
                float4x4 mat;
                float4 color;
            };

            sampler2D _MainTex;
            StructuredBuffer<MeshProperties> _Properties;

            v2f vert(appdata_t i, uint instanceID: SV_InstanceID) 
            {
                UNITY_SETUP_INSTANCE_ID(i);

                v2f o;
                o.uv = i.uv;
                float4 pos = mul(_Properties[instanceID].mat, i.vertex);
                o.vertex = UnityObjectToClipPos(pos);
                o.color = _Properties[instanceID].color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float4 col = i.color * tex2D(_MainTex, i.uv);
                clip(col.a - 0.5);
                return i.color;
            }

            ENDCG
        }
    }
}
