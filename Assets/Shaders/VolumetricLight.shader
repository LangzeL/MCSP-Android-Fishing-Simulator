Shader "Custom/VolumetricLight" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Intensity ("Intensity", Range(0,10)) = 1
        _Density ("Density", Range(0,10)) = 1
        _NoiseScale ("Noise Scale", Range(0,100)) = 10
        _NoiseSpeed ("Noise Speed", Range(0,10)) = 1
    }
    
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            float4 _Color;
            float _Intensity;
            float _Density;
            float _NoiseScale;
            float _NoiseSpeed;
            
            float noise3D(float3 p) {
                return frac(sin(dot(p, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
            }
            
            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                float3 noisePos = i.worldPos * _NoiseScale + _Time.y * _NoiseSpeed;
                float noise = noise3D(noisePos);
                float intensity = pow(abs(dot(viewDir, float3(0,1,0))), _Density);
                
                fixed4 col = _Color;
                col.a *= intensity * noise * _Intensity;
                
                float dist = length(_WorldSpaceCameraPos - i.worldPos);
                col.a *= 1.0 / (1.0 + dist * 0.1);
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}