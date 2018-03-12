Shader "Custom/OutlineSprite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Tint",Color)=(1,1,1,1)
		_OutlineSize("Outline Size", float)=0
		_OutlineColor("Outline Color", Color)=(0,0,0,1)
	}
	SubShader
	{
		Tags
		{
			"Queue"="Transparent" 
			"IgnoreProjector"="True"
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
		}

		// No culling or depth
		Cull Off 
		ZWrite Off 
		ZTest Always
		Lighting Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert (float4 vertex : POSITION, float4 color : COLOR, float2 uv : TEXCOORD0, out float4 outpos : SV_POSITION)
			{
				v2f o;
				o.uv = uv;
				o.color = color;
				outpos = UnityObjectToClipPos(vertex);
				return o;
			}
			
			sampler2D _MainTex;
			fixed4 _Color;
			float _OutlineSize;
			fixed4 _OutlineColor;

			fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				fixed4 c = tex2D (_MainTex, i.uv);
				float2 x_dist = float2 (_OutlineSize, 0);
				float2 y_dist = float2 (0, _OutlineSize);
				if (c.a < 0.1) {
					if ((tex2D (_MainTex, i.uv + x_dist).a > 0) ||
				        (tex2D (_MainTex, i.uv - x_dist).a > 0) ||
					    (tex2D (_MainTex, i.uv + y_dist).a > 0) ||
					    (tex2D (_MainTex, i.uv - y_dist).a > 0))
						c = _OutlineColor;
				}
				return c;
			}
			ENDCG
		}
	}
}
