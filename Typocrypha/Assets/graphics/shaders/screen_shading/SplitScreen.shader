// should be used in post processing

Shader "Custom/SplitScreen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Tint",Color)=(1,1,1,1)
		_Width("Width",float)=1
		_Height("Height",float)=1

		_Slope("Slope",float)=0.5
		_CutHeight("Height",float)=0
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
			float _Width;
			float _Height;
            float _Slope;
			float _CutHeight;
			float _CutSize;

			fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				float2 screenSize = float2(_Width, _Height);
				bool kill = false;
				float yp = _Slope * i.uv.x + _CutHeight;
				if (_Slope == 0) _Slope = 0.001;
				float xp = (i.uv.y - _CutHeight) / _Slope;
				if ((i.uv.x - _CutSize < xp && i.uv.x + _CutSize > xp) ||
				    (i.uv.y - _CutSize < yp && i.uv.y + _CutSize > yp)) {
					kill = true;
				} else {
					if (i.uv.y < yp) i.uv.y += _CutSize;
					else             i.uv.y -= _CutSize;
					if (i.uv.x < xp) i.uv.x += _CutSize * _Slope;
					else             i.uv.x -= _CutSize * _Slope;
				}
				fixed4 c = tex2D(_MainTex, i.uv);
				if (kill) c = fixed4(0,0,0,1);
				return c;
			}
			ENDCG
		}
	}
}
