Shader "Hidden/Gotow/Isosurface/Field_Prepass"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "cginc/IsosurfaceMetaballField.cginc"
			#include "cginc/Isosurface.cginc"

			int _SliceIndex;

			fixed4 frag (v2f_img i) : SV_Target {
				float3 wVoxelMin = UVZToVoxelMin(i.uv.xy, _SliceIndex);
				return sampleField( wVoxelMin );
			}
			ENDCG
		}
	}
}
