Shader "Hidden/Gotow/CommandBufferExtensions/Blit"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			Name "BLIT_FIX"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;

			v2f_img vert( appdata_img v )
			{
				v2f_img o;
				o.pos = v.vertex;
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag (v2f_img i) : SV_Target {
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
