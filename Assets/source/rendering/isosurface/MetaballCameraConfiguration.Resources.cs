using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering.Isosurface {

	public partial class MetaballCameraConfiguration {

		internal delegate void DestroyTemporaryResourcesDelegate ();
		private DestroyTemporaryResourcesDelegate destroyTemporaryResources;

		internal RenderTexture _fieldTexture;
		public RenderTexture fieldTexture {
			get {
				if (_fieldTexture == null) {
					_fieldTexture = new RenderTexture( (int)sampleResolution.x / 2, (int)sampleResolution.y / 2, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear );
					_fieldTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
					_fieldTexture.volumeDepth = (int)sampleResolution.z / 2;
					_fieldTexture.filterMode = FilterMode.Trilinear;
					_fieldTexture.wrapMode = TextureWrapMode.Clamp;
					_fieldTexture.name = "Gotow - Isosurface Field Buffer";
					
					destroyTemporaryResources += () => {
						ObjectUtility.SafeDestroy( _fieldTexture );
					};
				}
				return _fieldTexture;
			}
		}

		internal RenderTexture _vertexTexture;
		public RenderTexture vertexTexture {
			get {
				if (_vertexTexture == null) {
					_vertexTexture = new RenderTexture( (int)sampleResolution.x, (int)sampleResolution.y, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear );//RenderTexture.GetTemporary( (int)sampleResolution.x, (int)sampleResolution.y );
					_vertexTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
					_vertexTexture.volumeDepth = (int)sampleResolution.z;
					_vertexTexture.filterMode = FilterMode.Point;
					_vertexTexture.wrapMode = TextureWrapMode.Clamp;
					_vertexTexture.name = "Gotow - Isosurface Vertex Buffer";

					destroyTemporaryResources += () => {
						ObjectUtility.SafeDestroy( _vertexTexture );
					};
				}
				return _vertexTexture;
			}
		}

		internal Mesh _sampleMesh;
		public Mesh sampleMesh {
			get {
				if (_sampleMesh == null) {
					int vertexCount = (int)sampleResolution.x * (int)sampleResolution.y * (int)sampleResolution.z;
					var vertices = new Vector3[ vertexCount ];
					var indices = new int[ vertexCount ];
					
					int index = 0;
					for (int x = 0; x < sampleResolution.x; x ++) {
						for (int y = 0; y < sampleResolution.y; y ++) {
							for (int z = 0; z < sampleResolution.z; z ++) {
								vertices[index] = new Vector3( x, y, z );
								indices[index] = index;
								index ++;
							}
						}
					}

					_sampleMesh = new Mesh();
					_sampleMesh.indexFormat = IndexFormat.UInt32;
					_sampleMesh.vertices = vertices;
					_sampleMesh.SetIndices( indices, MeshTopology.Points, 0 );
					_sampleMesh.RecalculateBounds();
				
					destroyTemporaryResources += () => {
						ObjectUtility.SafeDestroy( _sampleMesh );
					};
				}
				return _sampleMesh;
			}
		}

		internal CommandBuffer _fieldPrepass;
		public CommandBuffer fieldPrepass {
			get {
				if (_fieldPrepass == null) {
					_fieldPrepass = new CommandBuffer();
					_fieldPrepass.name = "Gotow - Isosurface Field Prepass";

					_fieldPrepass.Clear();
					
					_fieldPrepass.Blit3D( fieldTexture, fieldTexture, MetaballSystem.fieldPrepassMaterial );

					destroyTemporaryResources += () => {
						_fieldPrepass = null;
					};
				}
				return _fieldPrepass;
			}
		}

		internal CommandBuffer _vertexPrepass;
		public CommandBuffer vertexPrepass {
			get {
				if (_vertexPrepass == null) {
					_vertexPrepass = new CommandBuffer();
					_vertexPrepass.name = "Gotow - Isosurface Vertex Prepass";

					_vertexPrepass.Clear();
					_vertexPrepass.Blit3D( vertexTexture, vertexTexture, MetaballSystem.vertexPrepassMaterial );

					destroyTemporaryResources += () => {
						_vertexPrepass = null;
					};
				}
				return _vertexPrepass;
			}
		}
		
	}

} // namespace