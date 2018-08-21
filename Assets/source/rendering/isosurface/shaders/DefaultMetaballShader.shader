Shader "Hidden/Gotow/Isosurface/Isosurface"
{
	Properties
	{
		_ReflectionCube ("Reflection", CUBE) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull Off
			
			CGPROGRAM
			#pragma vertex isosurface_vert
			#pragma geometry isosurface_geom
			#pragma fragment frag
			
			#pragma multi_compile __ WIREFRAME_ON

			#include "UnityCG.cginc"
			#include "cginc/IsosurfaceMetaballField.cginc"
			#include "cginc/Isosurface.cginc"

			samplerCUBE _ReflectionCube;
			sampler2D _CameraBlurTex;

			fixed4 frag (ISO_VERTEX i) : SV_Target
			{
				return fixed4( normalize(i.normal.xyz)*0.5+0.5, 1 );
				
				//float3 wView = normalize(i.wPos - _CameraWorldPosition);
				//float3 wRefl = reflect(wView, i.normal);

				//fixed4 env = texCUBE( _ReflectionCube, wRefl );
				//return fixed4( env.xyz, 1 );
			}
			ENDCG
		}
	}
}
