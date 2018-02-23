Shader "Custom/Pixelate"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PixelateAmount("Pixelate Amount",float)=1024
		_PixelSize("Pixel Size",float)=4
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//float4 vertex : SV_POSITION;
			};

			v2f vert (float4 vertex : POSITION, float2 uv : TEXCOORD0, out float4 outpos : SV_POSITION)
			{
				v2f o;
				o.uv = uv;
				outpos = UnityObjectToClipPos(vertex);
				return o;
			}
			
			sampler2D _MainTex;
			float _PixelateAmount;
			float _PixelSize;

			fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				float2 screenSize = float2(1280, 720);
				screenPos.xy = screenPos.xy - (screenPos.xy % _PixelSize);

				if (_PixelSize > 1) 
				{
					i.uv.x = screenPos.x/screenSize.x;
					i.uv.y = 1 - screenPos.y/screenSize.y;
				}
				//i.uv.y = floor(i.uv.y / aspect_rat * _PixelateAmount)/_PixelateAmount;
				fixed4 c = tex2D (_MainTex, i.uv);
				return c;
			}
			ENDCG
		}
	}
}
