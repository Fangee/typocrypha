Shader "Custom/SplitSprite"
{
	Properties
	{
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

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
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            float _Slope;
			float _Height;
			float _CutSize;

            v2f vert(appdata_t IN) {
            	return SpriteVert(IN);
            }

            fixed4 frag(v2f IN) : SV_Target {
				float yp = _Slope * IN.texcoord.x + _Height;
				if (_Slope == 0) _Slope = 0.001;
				float xp = (IN.texcoord.y - _Height) / _Slope;
				if ((IN.texcoord.x - _CutSize < xp && IN.texcoord.x + _CutSize > xp) ||
				    (IN.texcoord.y - _CutSize < yp && IN.texcoord.y + _CutSize > yp)) {
					IN.color.a = 0;
				} else {
					if (IN.texcoord.y < yp) IN.texcoord.y += _CutSize;
					else                    IN.texcoord.y -= _CutSize;
					if (IN.texcoord.x < xp) IN.texcoord.x += _CutSize * _Slope;
					else                    IN.texcoord.x -= _CutSize * _Slope;
				}
            	return SpriteFrag(IN);
            }
        ENDCG
        }
    }
}
