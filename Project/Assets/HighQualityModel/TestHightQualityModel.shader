Shader "Custom/TestHightQualityModel" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		_Smoothness ("Smoothness", Range(0,1)) = 0.5
		_SpecMap ("Specular map", 2D) = "black" {}

		_BumpMap ("Bumpmap", 2D) = "bump" {}

		_Cube ("Cubemap", CUBE) = "" {}

		_FlowParam ("Flow Param (XY is velocity, Z is count, W is frequence)", Vector) = (1, 1, 1, 1)
		_FlowTex ("FlowTex(RGB)", 2D) = "black" {}
		_FlowMaskTex ("FlowMaskTex(RGB)", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular fullforwardshadows finalcolor:mycolor

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			fixed2 uv_MainTex;
			fixed2 uv_BumpMap;
			fixed2 uv_SpecMap;
			float3 worldRefl;
			INTERNAL_DATA
		};

		fixed4 _Color;
		sampler2D _BumpMap;
		half _Smoothness;
		sampler2D _SpecMap; 
		samplerCUBE _Cube;
		fixed4 _FlowParam;
		sampler2D _FlowTex;
		sampler2D _FlowMaskTex;

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;

			//法线贴图
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));

			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;

			//高光
			fixed4 flowTex = tex2D (_FlowMaskTex, IN.uv_MainTex);
			c = tex2D(_SpecMap, IN.uv_SpecMap);
			o.Specular = c.rgb * flowTex.r; 
			o.Smoothness = _Smoothness;

			//环境映射
			fixed4 cubeTex = texCUBE (_Cube, WorldReflectionVector (IN, o.Normal));
			if(flowTex.r > 0.1)
			{
				fixed4 cubeTex = texCUBE (_Cube, WorldReflectionVector (IN, o.Normal));
				//o.Albedo = cubeTex*0.1 + o.Albedo*0.9;
				//o.Specular = o.Specular*0.7 + cubeTex*0.7;
				//o.Emission = cubeTex*0.3f;
				//o.Alpha = 0.1;
			}

			//流光
			c = tex2D (_FlowMaskTex, IN.uv_MainTex);
			if(c.r > 0.1)
			{
				fixed2 uv_FlowTex = IN.uv_MainTex*_FlowParam.z;
				uv_FlowTex.x += _FlowParam.x * _Time.y;
				uv_FlowTex.y += _FlowParam.y * _Time.y;
				float f = uv_FlowTex.y - trunc(uv_FlowTex.y);
				if(f < 0.1)
				{
					uv_FlowTex.y = f/0.1;
					fixed3 flowTex = tex2D (_FlowTex, uv_FlowTex).rgb;
						o.Albedo *= 1 + 2*flowTex.g*c.r;
				}
			}
		}
		
		void mycolor (Input IN, SurfaceOutputStandardSpecular o, inout fixed4 color)
		{
			fixed4 cubeTex = texCUBE (_Cube, WorldReflectionVector (IN, o.Normal));
			fixed4 flowTex = tex2D (_FlowMaskTex, IN.uv_MainTex);
			if(flowTex.r > 0.05)
			{
				//color.rgb = cubeTex.rgb*flowTex.r*0.3;
				//color.rgb *= 0.7;
			}
			else
			{
				//color = tex2D (_MainTex, IN.uv_MainTex);
			}
			//color.rgb = o.Albedo + o.Specular;
			// FinalColor = Ambient * RenderSettings ambientsetting + (Light Color * Diffuse + Light Color *Specular) + Emission
		}
		ENDCG
	}
	FallBack "Diffuse"
}
