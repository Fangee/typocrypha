Shader "Custom/PixelateSprite"
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

            float _PixelSize;
			float _Width;
			float _Height;

            v2f vert(appdata_t IN) {
            	return SpriteVert(IN);
            }

            fixed4 frag(v2f IN) : SV_Target {
            	float2 spriteSize = float2(_Width, _Height);
				float pixelScale = spriteSize.x/_PixelSize;
				// set pixel color to color of bottom left corner of block
				if (_PixelSize > 1) {
					IN.texcoord = floor(IN.texcoord * pixelScale)/pixelScale;
				}
            	return SpriteFrag(IN);
            }
        ENDCG
        }
    }
}
