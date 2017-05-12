Shader "X2D/UnlitCullOff"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_Alpha ("Alpha", range(0, 1)) = 1
		_FlipX ("FlipX", range(0, 1)) = 0
		_FlipY ("FlipY", range(0, 1)) = 0
	}
	SubShader
	{
		Tags {"Queue" = "Transparent"}
		cull off
		BlendOp add
		Blend SrcAlpha OneMinusSrcAlpha
		Zwrite on
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _FlipX;
			fixed _FlipY;
			fixed _Alpha;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				if(_FlipX > 0.5)
					o.uv.x = abs(1 - o.uv.x);
				if (_FlipY > 0.5)
					o.uv.y = abs(1 - o.uv.y);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col.a *= _Alpha;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);				
				return col;
			}
			ENDCG
		}
	}
}
