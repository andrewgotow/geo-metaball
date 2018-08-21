Shader "Hidden/Gotow/Isosurface/Vertex_Prepass"
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
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "cginc/IsosurfaceMetaballField.cginc"
			#include "cginc/Isosurface.cginc"

			float _SliceIndex;

			struct PREPASS_SAMPLE {
				float3 voxelPos;
				float4 field;
			};

			v2f_img vert( appdata_img v )
			{
				v2f_img o;
				o.pos = v.vertex;
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag (v2f_img i) : SV_Target {
				float3 voxelMin = UVZToVoxelMin(i.uv.xy, _SliceIndex);
				float3 voxelMax = voxelMin + 1;

				int cornerMask = 0;
				float4 lerpPos = 0;

				PREPASS_SAMPLE samples[8];
				for (int corner = 0; corner < 8; corner ++) {
					samples[corner].voxelPos = lerp(voxelMin, voxelMax, voxelCornerOffsets[corner]);
					samples[corner].field = sampleField( VoxelToWorldPos(samples[corner].voxelPos) );
					cornerMask |= (samples[corner].field.w < ISO_LEVEL) << corner;
				}

				for (int edge = 0; edge < 12; edge ++) {
					bool edgeIntersected = edge_table[cornerMask] & (1<<edge);

					int e1 = cube_edges[ edge<<1 ];
					int e2 = cube_edges[ (edge<<1)+1 ];

					PREPASS_SAMPLE s1 = samples[e1];
					PREPASS_SAMPLE s2 = samples[e2];
					float t = (ISO_LEVEL - s1.field.w) / (s2.field.w - s1.field.w);

					lerpPos.xyz += lerp( voxelCornerOffsets[e1], voxelCornerOffsets[e2], t ) * edgeIntersected;
					lerpPos.w += edgeIntersected;
				}

				return fixed4( 
					lerpPos.xyz / lerpPos.w, 
					(cornerMask > 0) && (cornerMask < 255)
				);
			}
			ENDCG
		}
	}
}
