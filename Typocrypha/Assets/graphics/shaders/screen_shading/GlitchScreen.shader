// should be used in post processing

Shader "Custom/GlitchScreen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Tint",Color)=(1,1,1,1)
		_GlitchAmount("Glitch Amount",float)=1
		_GlitchSpeed("Glitch Speed",float)=1
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
			float _GlitchAmount;
			float _GlitchSpeed;
			float _Width;
			float _Height;

			fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				float2 screenSize = float2(_Width, _Height);
				// separate screen into horizontal bands
				float bound = floor(screenPos.y - (screenPos.y % 11));
				// vary speed
				float speed = _GlitchSpeed * screenPos.y % 109;
				// vary intensity (local disturbance)
				float scale = _GlitchAmount * ((bound % 31)/17);
				// shift bands
				float mod_speed = cos(_Time.g) * 100;
				float mod_time = ceil(_Time.g * mod_speed)/mod_speed;
				i.uv.x += sin(bound + mod_time * _GlitchSpeed) * scale;
				fixed4 c = tex2D (_MainTex, i.uv);
				return c;
			}
			ENDCG
		}
	}
}
