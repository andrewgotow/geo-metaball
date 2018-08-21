using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering {

	public static class CommandBufferExtensions {

		internal static Mesh _blitMesh;
		private static Mesh blitMesh {
			get {
				if (_blitMesh == null) {
					_blitMesh = new Mesh();
					_blitMesh.vertices = new Vector3[] {
						new Vector3(-1,-1, 1),
						new Vector3( 1,-1, 1),
						new Vector3( 1, 1, 1),
						new Vector3(-1, 1, 1)
					};
					_blitMesh.uv = new Vector2[] {
						new Vector2(0,0),
						new Vector2(1,0),
						new Vector2(1,1),
						new Vector2(0,1)
					};
					_blitMesh.SetIndices( new int[] {
						0, 1, 2, 2, 3, 0
					}, MeshTopology.Triangles, 0 );
					_blitMesh.RecalculateBounds();
				}
				return _blitMesh;
			}
		}

		internal static Material _blitMaterial;
		private static Material blitMaterial {
			get {
				if (_blitMaterial == null) {
					var shader = Shader.Find("Hidden/Gotow/CommandBufferExtensions/Blit");
					if (shader == null)
						throw new System.Exception("Could not find shader \"Hidden/Gotow/CommandBufferExtensions/Blit\". Make sure it's in the \"always include\" list in the graphics settings.");

					_blitMaterial = new Material(shader);
					_blitMaterial.name = "Gotow - CommandBuffer Extension Blit Material";
					_blitMaterial.hideFlags = HideFlags.HideAndDontSave;
				}
				return _blitMaterial;
			}
		}

		public static void BlitToCurrentTarget(this CommandBuffer commandBuffer, RenderTargetIdentifier source, Material material = null, int pass = -1) {
			if (material == null)
				material = blitMaterial;
			
			commandBuffer.SetGlobalTexture("_MainTex", source);
			commandBuffer.DrawMesh( blitMesh, Matrix4x4.identity, material, 0, pass );
		}

		
		public static void Blit3D(this CommandBuffer commandBuffer, RenderTargetIdentifier source, RenderTexture dest, Material material = null, int pass = -1) {
			commandBuffer.SetGlobalTexture("_MainTex", source);

			if (dest.dimension == TextureDimension.Tex3D) {
				for (int slice = 0; slice < dest.volumeDepth; slice++) {
					commandBuffer.SetGlobalFloat( "_SliceIndex", slice );
					commandBuffer.SetRenderTarget( dest, 0, CubemapFace.Unknown, slice );

					// CommandBuffer.Blit does not work with depth-slices enabled. As a workaround, provide our own
					// blit quad-mesh, and use it instead.
					// https://fogbugz.unity3d.com/default.asp?879378_5e02ge03gc71g67u
					commandBuffer.DrawMesh( blitMesh, Matrix4x4.identity, material, 0, pass );
					//commandBuffer.Blit( source, BuiltinRenderTextureType.CurrentActive, material );
				}
			}else{
				commandBuffer.Blit( source, BuiltinRenderTextureType.CurrentActive, material );
			}
		
		}

	}

}