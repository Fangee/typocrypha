Shader "Custom/SplitSprite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Tint",Color)=(1,1,1,1)
		_Slope("Slope",float)=0.5
		_Height("Height",float)=0
		_CutSize("Cut Size",float)=0.1
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

			float _Slope;
			float _Height;
			float _CutSize;

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

			fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				float2 pos = float2(i.uv);
				fixed4 c = fixed4(0,0,0,0);
				float yp = _Slope * pos.x + _Height;
				if (_Slope == 0) _Slope = 0.001;
				float xp = (pos.y - _Height) / _Slope;
				if ((pos.x - _CutSize < xp && pos.x + _CutSize > xp) ||
				    (pos.y - _CutSize < yp && pos.y + _CutSize > yp)) {
					c.a = 0;
				} else {
					if (pos.y < yp) pos.y += _CutSize;
					else            pos.y -= _CutSize;
					if (pos.x < xp) pos.x += _CutSize * _Slope;
					else            pos.x -= _CutSize * _Slope;
					c = tex2D (_MainTex, pos.xy);
				}
				return c;
			}
			ENDCG
		}
	}
}
