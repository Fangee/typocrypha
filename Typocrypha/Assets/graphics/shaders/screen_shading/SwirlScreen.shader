// should be used in post processing

Shader "Custom/SwirlScreen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Width("Width",float)=1
		_Height("Height",float)=1
		_SwirlAmount("Swirl Amount",float)=0
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
			float _Width;
			float _Height;
			float _SwirlAmount;

			fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				if (_SwirlAmount == 0) return tex2D (_MainTex, i.uv);

				float2 screenSize = float2(_Width, _Height);
				float2 pos = float2(screenPos.x,screenPos.y);
				// shift screen center to middle
				pos.x -= _Width/2;
				pos.y -= _Height/2;
				// calculate rotation amount based on distance from center
				float rot = dot(pos, pos);
				float len = dot(float2(_Width/2, _Height/2), float2(_Width/2, _Height/2));
				rot = rot/len;
				rot *= _SwirlAmount != 0 ? 1/pow(_SwirlAmount,4) : 0;
				rot = rot != 0 ? 1/rot : 0;
				// apply rotation to each pixel
				pos.x = cos(rot)*pos.x - sin(rot)*pos.y;
				pos.y = sin(rot)*pos.x + cos(rot)*pos.y;
				// shift screen back to old coordinates
				pos.x += _Width/2;
				pos.y += _Height/2;
				pos.y = _Height - pos.y;
				// set rotated pixel color
				return tex2D (_MainTex, float2(pos.x/screenSize.x, 1-pos.y/screenSize.y));
			}
			ENDCG
		}
	}
}
