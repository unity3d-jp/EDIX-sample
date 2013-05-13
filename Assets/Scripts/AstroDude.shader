
Shader "AstroDude" {
	
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "grey" {}
	_SelfIllumin ("Self Illumin", 2D) = "black" {}

	_Cube ("Cube", CUBE) = "white" {}
}

// simple lighting for MQ & LQ versions

SubShader {
	Lighting on
	Tags { "RenderType"="Opaque" "Reflection" = "RenderReflectionOpaque" "Queue"="Geometry" }
	
	Pass {

		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
				
		uniform half4 _MainTex_ST;
		uniform sampler2D _MainTex;
		uniform samplerCUBE _Cube;
		uniform sampler2D _SelfIllumin;
		
		struct v2f
		{
			half4 pos : POSITION;
			half4 color : TEXCOORD0;
			half2 uv : TEXCOORD1;
			
			half3 reflectionLookup : TEXCOORD5;
		};
		
		float3 ShadeMyVertexLights (float4 vertex, float3 normal)
		{
			float3 viewpos = mul(UNITY_MATRIX_MV, vertex).xyz;
			float3 viewN = mul((float3x3) UNITY_MATRIX_IT_MV, normal);
			float3 lightColor = float3(0.0,0.0,0.0);
			
			for (int i = 0; i < 1; i++) {
				float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz;
				float lengthSq = dot(toLight, toLight);
				float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i]);
				float diff = max (0, dot (viewN, normalize(toLight)));
				lightColor += unity_LightColor[i].rgb * (diff * atten);
			}
			return (lightColor * 2.0 + 0.25);
		}
		
		void WriteTangentSpaceData (appdata_full v, out half3 ts0, out half3 ts1, out half3 ts2) {
			TANGENT_SPACE_ROTATION;
			ts0 = mul(rotation, _Object2World[0].xyz * unity_Scale.w);
			ts1 = mul(rotation, _Object2World[1].xyz * unity_Scale.w);
			ts2 = mul(rotation, _Object2World[2].xyz * unity_Scale.w);				
		}		
		
		v2f vert (appdata_full v)
		{
			v2f o;
			
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = float4 (ShadeMyVertexLights(v.vertex, v.normal), 1.0);
			o.uv.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
			
			o.reflectionLookup = reflect( -normalize(WorldSpaceViewDir (v.vertex)), normalize(mul((float3x3)_Object2World,v.normal)) );		
							
			return o; 
		}
		
		fixed4 frag (v2f i) : COLOR 
		{
			fixed4 tex = tex2D(_MainTex, i.uv.xy);
			
			fixed4 refl = texCUBE(_Cube, i.reflectionLookup ) * tex.a; 			
			fixed4 selfIllumin = tex2D(_SelfIllumin, i.uv.xy);
			
			return refl + i.color * tex + selfIllumin;
		}
		
		ENDCG
	}
}
FallBack "Diffuse"
}
