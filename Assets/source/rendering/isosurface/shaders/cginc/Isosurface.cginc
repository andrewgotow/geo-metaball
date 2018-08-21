#ifndef ISOSURFACE
#define ISOSURFACE

#include "UnityCG.cginc"
#include "IsosurfaceTables.cginc"

#define ISO_LEVEL 1.0

int3 _Isosurface_VoxelResolution;

sampler3D _Isosurface_FieldTexture;
sampler3D _Isosurface_VertexTexture;

float4 _CameraFrustumCorners[4];
float3 _CameraWorldPosition;

// =================================================================================
// VOXEL UTILITIES
// =================================================================================

float3 VoxelToWorldPos ( float3 pos ) {
    float3 screenPos = float3(
		(pos.xy-1) / (_Isosurface_VoxelResolution.xy-3),
		pos.z / (_Isosurface_VoxelResolution.z-1)
	);

    // build a world space ray by taking the far corner of the frustum corresponding to
    // this vertex. The passed frustum corners should be in world space.
    // this ray is NOT normalized, and represents the total length of the shadow march.
    float3 ray = lerp(
        lerp( _CameraFrustumCorners[0], _CameraFrustumCorners[1], screenPos.y ),
        lerp( _CameraFrustumCorners[3], _CameraFrustumCorners[2], screenPos.y ),
        screenPos.x
    );

    return (ray - _CameraWorldPosition) * screenPos.z + _CameraWorldPosition;
}

float4 VoxelToClipPos ( float3 pos ) {
    /*
    // This works quite well, but doesn't set the depth value properly.
	// consider fixing this and using it to avoid the matrix multiplication.
    float3 vPos = float3( 
        (pos.xy - 1) / (_VoxelResolution.xy-2) * 2 - 1, 
        pos.z / _VoxelResolution.z
    );
	return float4(vPos, 1);
    */
    return UnityWorldToClipPos( VoxelToWorldPos(pos) );
}

float3 VoxelMinToUVZ ( float3 voxelMin ) {
    return (voxelMin+0.5) / _Isosurface_VoxelResolution;// + offset;
}

float3 UVZToVoxelMin ( float2 uv, int slice ) {
    return float3( uv * _Isosurface_VoxelResolution - 0.5, (float)slice );
}

// =================================================================================
// Isosurface Vert / Frag / Geom
// =================================================================================

#ifdef WIREFRAME_ON
	#define OUTPUT_STREAM LineStream
#else
	#define OUTPUT_STREAM TriangleStream
#endif

struct ISO_VOXEL {
	float3 voxelMin : TEXCOORD0;
	float3 voxelMax : TEXCOORD1;
};

struct ISO_VERTEX {
	float4 pos : SV_POSITION;
	float3 wPos : TEXCOORD0;
	float3 normal : TEXCOORD1;
	bool onSurface : TEXCOORD2;
};

ISO_VOXEL isosurface_vert (float4 pos : POSITION)
{
	ISO_VOXEL o;
	
	o.voxelMin = pos;//mul(unity_ObjectToWorld, pos);
	o.voxelMax = pos+1;//mul(unity_ObjectToWorld, pos+1);// + 1/_VoxelResolution);

	return o;
}

ISO_VERTEX vertexForVoxel ( float3 voxelMin, float3 voxelMax ) {
	ISO_VERTEX vertex = (ISO_VERTEX)0;

	float4 vertexData = tex3Dlod(_Isosurface_VertexTexture, float4(VoxelMinToUVZ(voxelMin), 0));
	float3 voxelPos = lerp( voxelMin, voxelMax, vertexData.xyz );

	vertex.pos = VoxelToClipPos( voxelPos );
	vertex.wPos = VoxelToWorldPos( voxelPos );
	vertex.normal = normalize( sampleField( vertex.wPos ).xyz );
	vertex.onSurface = vertexData.w;

	return vertex;
}

[MaxVertexCount(24)]
void isosurface_geom (point ISO_VOXEL voxel[1], inout OUTPUT_STREAM<ISO_VERTEX> triStream) {
	
	//
	// Voxel layout
	//
	//  y
	//  ^         z
	//  |        /
	//    7 -- 6
	//  4 |  5 |
	//  | 3 -| 2
	//  |/   |/
	//  0 -- 1  ->x
	//

	//
	// Neigbours layout
	//
	//       2  4
	//       | /
	//       |/
	//   1---c---0
	//      /|
	//     / |
	//    5  3

	ISO_VERTEX current = vertexForVoxel( voxel[0].voxelMin, voxel[0].voxelMax );
	
	if (current.onSurface) {
		
		int neighborMask = 0;
		ISO_VERTEX neighbors[6];
		for (int i = 0; i < 6; i++) {
			neighbors[i] = vertexForVoxel( voxel[0].voxelMin + voxelNeighborOffsets[i], voxel[0].voxelMax + voxelNeighborOffsets[i] );
			neighborMask |= neighbors[i].onSurface << i;
		}

		// XZ plane
		if ((neighborMask & (1<<0)) && (neighborMask & (1<<4))) {
			triStream.Append( current );
			triStream.Append( neighbors[4] );
			triStream.Append( neighbors[0] );

			triStream.RestartStrip();
		}

		if ((neighborMask & (1<<4)) && (neighborMask & (1<<1))) {
			triStream.Append( current );
			triStream.Append( neighbors[4] );
			triStream.Append( neighbors[1] );
			triStream.RestartStrip();
		}

		if ((neighborMask & (1<<1)) && (neighborMask & (1<<5))) {
			triStream.Append( current );
			triStream.Append( neighbors[5] );
			triStream.Append( neighbors[1] );
			triStream.RestartStrip();
		}

		if ((neighborMask & (1<<5)) && (neighborMask & (1<<0))) {
			triStream.Append( current );
			triStream.Append( neighbors[5] );
			triStream.Append( neighbors[0] );
			triStream.RestartStrip();
		}

		// XY plane
		if ((neighborMask & (1<<0)) && (neighborMask & (1<<2))) {
			triStream.Append( current );
			triStream.Append( neighbors[0] );
			triStream.Append( neighbors[2] );
			triStream.RestartStrip();
		}

		if ((neighborMask & (1<<2)) && (neighborMask & (1<<1)))  {
			triStream.Append( current );
			triStream.Append( neighbors[1] );
			triStream.Append( neighbors[2] );
			triStream.RestartStrip();
		}

		if ((neighborMask & (1<<1)) && (neighborMask & (1<<3)))  {
			triStream.Append( current );
			triStream.Append( neighbors[3] );
			triStream.Append( neighbors[1] );
			triStream.RestartStrip();
		}

		if ((neighborMask & (1<<3)) && (neighborMask & (1<<0)))  {
			triStream.Append( current );
			triStream.Append( neighbors[0] );
			triStream.Append( neighbors[3] );
			triStream.RestartStrip();
		}

		// YZ plane
		if ((neighborMask & (1<<4)) && (neighborMask & (1<<2)))  {
			triStream.Append( current );
			triStream.Append( neighbors[4] );
			triStream.Append( neighbors[2] );
			triStream.RestartStrip();
		}

		if ((neighborMask & (1<<2)) && (neighborMask & (1<<5)))  {
			triStream.Append( current );
			triStream.Append( neighbors[5] );
			triStream.Append( neighbors[2] );
			triStream.RestartStrip();
		}

		if ((neighborMask & (1<<5)) && (neighborMask & (1<<3)))  {
			triStream.Append( current );
			triStream.Append( neighbors[5] );
			triStream.Append( neighbors[3] );
			triStream.RestartStrip();
		}

		if ((neighborMask & (1<<3)) && (neighborMask & (1<<4)))  {
			triStream.Append( current );
			triStream.Append( neighbors[4] );
			triStream.Append( neighbors[3] );
			triStream.RestartStrip();
		}

	}

}

#endif