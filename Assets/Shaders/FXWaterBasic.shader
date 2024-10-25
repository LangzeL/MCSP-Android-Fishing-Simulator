Shader "FX/Water (Vertical)" {
	Properties {
		_horizonColor ("Horizon color", COLOR)  = ( .172 , .463 , .435 , 0)
		_deepColor ("Deep color", Color) = (0.0, 0.2, 0.3, 1.0)
		_WaveScale ("Wave scale", Range (0.02,0.15)) = .07
		_WaveStrength ("Wave strength", Range(0.1, 2.0)) = 0.5
		_DepthFactor ("Depth factor", Range(0.1, 10.0)) = 1.0
		[NoScaleOffset] _ColorControl ("Reflective color (RGB) fresnel (A) ", 2D) = "" { }
		[NoScaleOffset] _BumpMap ("Waves Normalmap ", 2D) = "" { }
		WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	uniform float4 _horizonColor;
	uniform float4 _deepColor;
	uniform float4 WaveSpeed;
	uniform float _WaveScale;
	uniform float _WaveStrength;
	uniform float _DepthFactor;
	uniform float4 _WaveOffset;
	
	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	
	struct v2f {
		float4 pos : SV_POSITION;
		float2 bumpuv[2] : TEXCOORD0;
		float3 viewDir : TEXCOORD2;
		float4 screenPos : TEXCOORD3;
		float depth : TEXCOORD4;
		UNITY_FOG_COORDS(5)
	};
	
	v2f vert(appdata v)
	{
		v2f o;
		float4 wpos = mul(unity_ObjectToWorld, v.vertex);
		
		float2 verticalUV = float2(wpos.x, wpos.y) * _WaveScale;
		
		o.bumpuv[0] = verticalUV + _Time.x * WaveSpeed.xy * 0.4;
		o.bumpuv[1] = verticalUV * 1.4 + _Time.x * WaveSpeed.zw * 0.3;
		
		o.pos = UnityObjectToClipPos(v.vertex);
		o.screenPos = ComputeScreenPos(o.pos);
		o.depth = -mul(UNITY_MATRIX_V, wpos).z * _DepthFactor;
		
		o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
		
		UNITY_TRANSFER_FOG(o,o.pos);
		return o;
	}
	ENDCG
	
	Subshader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			sampler2D _BumpMap;
			sampler2D _ColorControl;
			
			half4 frag(v2f i) : COLOR
			{
		
				half3 bump1 = UnpackNormal(tex2D(_BumpMap, i.bumpuv[0])).rgb;
				half3 bump2 = UnpackNormal(tex2D(_BumpMap, i.bumpuv[1])).rgb;
				half3 bump = (bump1 + bump2) * 0.5 * _WaveStrength;
				

				half fresnel = saturate(dot(i.viewDir, bump));
				half4 water = tex2D(_ColorControl, float2(fresnel, fresnel));
				
				float depthFactor = saturate(i.depth * 0.1);
				half4 col;
				col.rgb = lerp(_deepColor.rgb, _horizonColor.rgb, water.a * (1 - depthFactor));
				col.a = lerp(0.6, 0.9, depthFactor);
				
				col.rgb += bump.xyz * 0.1;
				
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
	}