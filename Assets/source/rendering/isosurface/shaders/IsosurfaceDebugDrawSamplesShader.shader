Shader "Hidden/Isosurface/Debug_DrawSamples"
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
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "cginc/IsosurfaceMetaballField.cginc"
			#include "cginc/Isosurface.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 uv : TEXCOORD2;
			};
	
			v2f vert (float4 pos : POSITION)
			{
				v2f o;
				o.vertex = UnityWorldToClipPos( VoxelToWorldPos(pos) );
				o.uv = pos / _Isosurface_VoxelResolution;
				return o;
			}

			[MaxVertexCount(3)]
			void geom (point v2f vert[1], inout TriangleStream<v2f> triStream) {
				triStream.Append(vert[0]);
				
				vert[0].vertex.xy += float2(-1,-1)/20;
				triStream.Append(vert[0]);

				vert[0].vertex.xy += float2( 1,-1)/20;
				triStream.Append(vert[0]);

				triStream.RestartStrip();
			}
					
			fixed4 frag (v2f i) : SV_Target
			{
				return tex3D(_Isosurface_VertexTexture, i.uv);
			}
			ENDCG
		}
	}
}
