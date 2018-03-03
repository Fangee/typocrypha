// should be used in post processing

Shader "Custom/PixelateScreen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Tint",Color)=(1,1,1,1)
		_PixelSize("Pixel Size",float)=1
		_Width("Width",float)=1
		_Height("Height",float)=1
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
			float _PixelSize;
			float _Width;
			float _Height;

			fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				float2 screenSize = float2(_Width, _Height);
				// divide screen into blocks
				screenPos.xy = screenPos.xy - (screenPos.xy % _PixelSize);
				// set pixel color to color of upper left corner of block
				if (_PixelSize > 1) 
				{
					i.uv.x = screenPos.x/screenSize.x;
					i.uv.y = 1 - screenPos.y/screenSize.y;
				}
				fixed4 c = tex2D (_MainTex, i.uv);
				return c;
			}
			ENDCG
		}
	}
}
