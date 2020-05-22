// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ChangeSaturation"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ChangeValue ("Saturation Change Value", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct VS_INPUT
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 diffuse : TEXC00RD1;
			};

			sampler2D _MainTex;
			float _ChangeValue;
			
			v2f vert (VS_INPUT input)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(input.vertex);
				half3 worldNormal = UnityObjectToWorldNormal(input.normal);
				o.diffuse = dot(worldNormal, _WorldSpaceLightPos0.xyz);
				o.uv = input.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col.r += col.r * _ChangeValue;
				col.g += col.g * _ChangeValue;
				col.b += col.b * _ChangeValue;
				col *= fixed4(i.diffuse, 1);
				return col;
			}
			ENDCG
		}
	}
}
